using MobileBankingUSSD.API.Configuration;
using MobileBankingUSSD.API.Data.DbService;
using MobileBankingUSSD.API.Middleware;
using MobileBankingUSSD.API.Repositories;
using MobileBankingUSSD.API.Repositories.interfaces;
using MobileBankingUSSD.API.Services;
using MobileBankingUSSD.API.Services.interfaces;
using Npgsql;
using Serilog;

namespace MobileBankingUSSD.API;

public class Program
{
    public static void Main(string[] args)
    {

        // Add services to the IOC container.
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        //  set up logging service
        builder.Host.UseSerilog((ctx, lc) => 
            lc.WriteTo.Console()
                .ReadFrom.Configuration(ctx.Configuration));
        
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        builder.Services.AddCors((option) =>
        {
            option.AddPolicy("AllowAll", opt =>
            {
                opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
        });
        
        // Setup Database connection
        var databaseConnectionString = builder.Configuration.GetConnectionStringOrThrow("Database"); 
        
        var npgsqlDataSourceBuilder = new NpgsqlDataSourceBuilder(databaseConnectionString);
        if (builder.Environment.IsDevelopment())
        {
            npgsqlDataSourceBuilder.EnableParameterLogging();
        }
        
        var npgsqlDataSource = npgsqlDataSourceBuilder.Build();
        
        builder.Services.AddSingleton(npgsqlDataSource);
        
        
        // Register Redis cache service
        builder.Services.AddSingleton<ICacheService, RedisCacheService>();

        // the in-memory session repository with Redis-based one
        builder.Services.AddScoped<IUssdSessionRepository, RedisUssdSessionRepository>();


        // set up Redis
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionStringOrThrow("Redis");
            options.InstanceName = "MobileBanking_"; // Prefix for all keys
        });
                
        // Add services to the container
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IAccountRepository, AccountRepository>();  
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<IAccountService, AccountService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
        builder.Services.AddScoped<ITransferFundService, TransferFundService>();
        builder.Services.AddScoped<ITransferFundRepository, TransferFundRepository>();
                
                
        builder.Services.AddScoped<DbTransactionFactory>(provider => 
                    new DbTransactionFactory(databaseConnectionString));
        
        
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseExceptionHandler();

        // middleware that logs requests using serilog
        app.UseSerilogRequestLogging();  


        app.UseCors("AllowAll");
        
        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
