using System.Data;
using Dapper;
using Npgsql;

namespace MobileBankingUSSD.API.Data.DbService;

public class DbTransactionFactory(string connectionString, ILogger<DbTransactionFactory>? logger = null)
    : IDisposable, IAsyncDisposable
{
    private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    private NpgsqlConnection? _connection;
    private NpgsqlTransaction? _transaction;

    private NpgsqlConnection GetOrCreateConnection()
    {
        if (_connection == null)
        {
            _connection = new NpgsqlConnection(_connectionString);
        }
        return _connection;
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = GetOrCreateConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
        return connection;
    }

    public async Task<NpgsqlTransaction> BeginTransactionAsync(
        IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already active");
        }

        var connection = await OpenConnectionAsync(cancellationToken);
        _transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _transaction;
    }

    public void CommitTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to commit");
        }

        try
        {
            _transaction.Commit();
            logger?.LogDebug("Transaction committed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to commit transaction");
            throw new DataAccessException("Failed to commit transaction", ex);
        }
        finally
        {
            CleanupTransaction();
        }
    }

    public void RollbackTransaction()
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No active transaction to rollback");
        }

        try
        {
            _transaction.Rollback();
            logger?.LogDebug("Transaction rolled back successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Failed to rollback transaction");
            throw new DataAccessException("Failed to rollback transaction", ex);
        }
        finally
        {
            CleanupTransaction();
        }
    }

    public async Task<List<T>> QueryListAsync<T>(
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default) where T : class
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        return await QueryListAsync<T>(connection, schema, functionName, parameters, cancellationToken);
    }

    public async Task<List<T>> QueryListAsync<T>(
        IDbConnection dbConnection,
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default) where T : class
    {
        ValidateParameters(dbConnection, schema, functionName);

        try
        {
            string query = BuildFunctionCallQuery(schema, functionName, parameters);
            
            var command = new CommandDefinition(
                commandText: query,
                parameters: parameters,
                commandType: CommandType.Text,
                transaction: _transaction,
                cancellationToken: cancellationToken);

            var result = await dbConnection.QueryAsync<T>(command);
            return result.AsList();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error executing function {Schema}.{Function}", schema, functionName);
            throw new DataAccessException(
                $"Error executing function {schema}.{functionName}", ex);
        }
    }

    public async Task<T?> QuerySingleAsync<T>(
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default) 
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        return await QuerySingleAsync<T>(connection, schema, functionName, parameters, cancellationToken);
    }

    public async Task<T?> QuerySingleAsync<T>(
        IDbConnection dbConnection,
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default) 
    {
        ValidateParameters(dbConnection, schema, functionName);

        try
        {
            string query = BuildFunctionCallQuery(schema, functionName, parameters);

            var command = new CommandDefinition(
                commandText: query,
                parameters: parameters,
                commandType: CommandType.Text,
                transaction: _transaction,
                cancellationToken: cancellationToken);

            return await dbConnection.QueryFirstOrDefaultAsync<T>(command);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error executing function {Schema}.{Function}", schema, functionName);
            throw new DataAccessException(
                $"Error executing function {schema}.{functionName}", ex);
        }
    }

    public async Task<int> ExecuteAsync(
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default,
        bool isStoredProcedure = false)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        return await ExecuteAsync(connection, schema, functionName, parameters, cancellationToken, isStoredProcedure);
    }
    
    public async Task<T> ExecuteStoredProcedureWithOutputAsync<T>(
        string schema,
        string procedureName,
        DynamicParameters parameters,
        string outputParameterName,
        CancellationToken cancellationToken = default)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        
        try
        {
            string fullProcedureName = $"{schema}.\"{procedureName}\"";
            
            await connection.ExecuteAsync(
                new CommandDefinition(
                    commandText: fullProcedureName,
                    parameters: parameters,
                    transaction: _transaction,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken
                ));
            
            return parameters.Get<T>(outputParameterName);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error executing procedure {Schema}.{Procedure}", schema, procedureName);
            throw new DataAccessException($"Error executing procedure {schema}.{procedureName}", ex);
        }
    }

    public async Task<int> ExecuteAsync(
        IDbConnection dbConnection,
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default,
        bool isStoredProcedure = false)
    {
        ValidateParameters(dbConnection, schema, functionName);
        
        try
        {
            if (isStoredProcedure)
            {
                string fullProcedureName = $"{schema}.\"{functionName}\"";
                
                var command = new CommandDefinition(
                    commandText: fullProcedureName,
                    parameters: parameters,
                    commandType: CommandType.StoredProcedure,
                    transaction: _transaction,
                    cancellationToken: cancellationToken);
                
                return await dbConnection.ExecuteAsync(command);
            }
            else
            {
                string query = BuildFunctionCallQuery(schema, functionName, parameters, true);
                
                var command = new CommandDefinition(
                    commandText: query,
                    parameters: parameters,
                    commandType: CommandType.Text,
                    transaction: _transaction,
                    cancellationToken: cancellationToken);
                
                return await dbConnection.ExecuteAsync(command);
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error executing {Type} {Schema}.{Function}",
                isStoredProcedure ? "procedure" : "function",
                schema, functionName);
            throw new DataAccessException(
                $"Error executing {(isStoredProcedure ? "procedure" : "function")} {schema}.{functionName}", ex);
        }
    }

    
    public async Task<T> ExecuteScalarAsync<T>(
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var connection = await OpenConnectionAsync(cancellationToken);
        return await ExecuteScalarAsync<T>(connection, schema, functionName, parameters, cancellationToken);
    }

    public async Task<T> ExecuteScalarAsync<T>(
        IDbConnection dbConnection,
        string schema,
        string functionName,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        ValidateParameters(dbConnection, schema, functionName);

        try
        {
            string query = BuildFunctionCallQuery(schema, functionName, parameters, true);

            var command = new CommandDefinition(
                commandText: query,
                parameters: parameters,
                commandType: CommandType.Text,
                transaction: _transaction,
                cancellationToken: cancellationToken);

            return await dbConnection.ExecuteScalarAsync<T>(command);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error executing function {Schema}.{Function}", schema, functionName);
            throw new DataAccessException(
                $"Error executing function {schema}.{functionName}", ex);
        }
    }

    private string BuildFunctionCallQuery(string schema, string functionName, object? parameters, bool forScalar = false)
    {
        string prefix = forScalar ? "SELECT" : "SELECT * FROM";
        
        if (parameters == null || !HasParameters(parameters))
        {
            return $"{prefix} {schema}.\"{functionName}\"()";
        }

        var paramList = GetParameterNames(parameters);
        return $"{prefix} {schema}.\"{functionName}\"({string.Join(", ", paramList)})";
    }

   
    private string BuildStoredProcedureCallQuery(string schema, string functionName, object? parameters)
    {
        if (parameters == null || !HasParameters(parameters))
        {
            return $"CALL {schema}.\"{functionName}\"()";
        }

        var paramList = new List<string>();
    
        if (parameters is DynamicParameters dynamicParams)
        {
            foreach (var paramName in dynamicParams.ParameterNames)
            {
                paramList.Add($"@{paramName}");
            }
        }
        else
        {
            paramList.AddRange(GetParameterNames(parameters));
        }

        return $"CALL {schema}.\"{functionName}\"({string.Join(", ", paramList)})";
    }
    
    private bool HasParameters(object? parameters)
    {
        if (parameters == null) return false;
        
        if (parameters is DynamicParameters dynamicParams)
        {
            return dynamicParams.ParameterNames.Any();
        }
        
        return parameters.GetType().GetProperties().Length > 0;
    }

    private IEnumerable<string> GetParameterNames(object parameters)
    {
        if (parameters is DynamicParameters dynamicParams)
        {
            return dynamicParams.ParameterNames.Select(p => $"@{p}");
        }
        
        return parameters.GetType().GetProperties().Select(p => $"@{p.Name}");
    }

    private void ValidateParameters(IDbConnection dbConnection, string schema, string functionName)
    {
        if (dbConnection == null)
            throw new ArgumentNullException(nameof(dbConnection));
        if (string.IsNullOrWhiteSpace(schema))
            throw new ArgumentException("Schema cannot be empty", nameof(schema));
        if (string.IsNullOrWhiteSpace(functionName))
            throw new ArgumentException("Function name cannot be empty", nameof(functionName));
    }

    private void CleanupTransaction()
    {
        try
        {
            _transaction?.Dispose();
        }
        finally
        {
            _transaction = null;
        }
    }

    public void Dispose()
    {
        CleanupTransaction();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await CleanupTransactionAsync();
        if (_connection != null)
        {
            await _connection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }

    private async ValueTask CleanupTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}

public class DataAccessException : Exception
{
    public DataAccessException(string message) : base(message) { }
    public DataAccessException(string message, Exception innerException) : base(message, innerException) { }
}