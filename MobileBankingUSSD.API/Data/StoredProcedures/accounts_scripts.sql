DROP FUNCTION IF EXISTS create_account(p_customer_id UUID, p_account_type INTEGER, 
                                       p_initial_balance DECIMAL(19,4), p_is_active BOOLEAN,
                                       p_account_id UUID, p_account_number VARCHAR(20), 
                                       p_success BOOLEAN, p_message VARCHAR(255));

DROP FUNCTION get_account_balance(character varying);

CREATE OR REPLACE PROCEDURE create_account(
    p_customer_id UUID,
    p_account_number VARCHAR(20),
    p_account_type INTEGER,
    p_initial_balance DECIMAL(19, 4),
    p_is_active BOOLEAN)
    LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO accounts (
        id,
        customer_id,
        account_number,
        account_type_id,
        balance,
        available_balance,
        created_at,
        is_active
    ) VALUES (
                 gen_random_uuid(),
                 p_customer_id,
                 p_account_number,
                 p_account_type,
                 p_initial_balance,
                 p_initial_balance,
                 NOW(),
                 p_is_active
             );
END;
$$;


CREATE OR REPLACE FUNCTION get_account_balance(p_account_number VARCHAR(20))
    RETURNS TABLE (
                      account_number VARCHAR(20),
                      balance DECIMAL(15, 2),
                      available_balance DECIMAL(15, 2),
                      account_type_id INTEGER,
                      last_updated TIMESTAMP
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT
            a.account_number,
            a.balance,
            a.available_balance,
            a.account_type_id,
            a.created_at AS last_updated
        FROM
            accounts a
        WHERE
            a.account_number = p_account_number
          AND a.is_active = TRUE;
END;
$$;


CREATE OR REPLACE FUNCTION account_number_exists(account_number_param VARCHAR)
    RETURNS BOOLEAN
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN EXISTS (SELECT 1 FROM accounts a WHERE a.account_number = account_number_param);
END;
$$;


CREATE OR REPLACE FUNCTION get_account_type_id(p_account_type_name VARCHAR(20))
    RETURNS INTEGER
    LANGUAGE plpgsql
AS $$
DECLARE
    v_account_type_id INTEGER;
BEGIN
    SELECT account_type_id INTO v_account_type_id
    FROM account_types
    WHERE account_type_name = p_account_type_name;

    RETURN v_account_type_id;
END;
$$;

CREATE OR REPLACE FUNCTION get_account_by_number(p_account_number VARCHAR(20))
    RETURNS TABLE (
                      id UUID,
                      account_number VARCHAR(20),
                      balance DECIMAL(15, 2),
                      available_balance DECIMAL(15, 2),
                      is_active BOOLEAN,
                      last_updated TIMESTAMP
                  )
AS $$
BEGIN
    RETURN QUERY
        SELECT
            id,
            account_number,
            balance,
            available_balance,
            is_active,
            last_updated
        FROM
            accounts
        WHERE
            account_number = p_account_number;
END;
$$ LANGUAGE plpgsql;
