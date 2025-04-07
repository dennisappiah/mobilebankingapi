-- CREATE EXTENSION IF NOT EXISTS pgcrypto;

DROP FUNCTION IF EXISTS get_all_customers();
DROP FUNCTION IF EXISTS get_customer_by_msisdn(text);
DROP FUNCTION IF EXISTS get_customer_by_id(uuid);
DROP PROCEDURE IF EXISTS change_customer_pin(uuid, text, text, OUT boolean);
DROP FUNCTION hash_customer_pin(character varying);
DROP PROCEDURE create_customer(character varying,character varying,character varying);


CREATE OR REPLACE FUNCTION get_all_customers()
    RETURNS TABLE (
                      id UUID,
                      msisdn VARCHAR(15),
                      full_name VARCHAR(100),
                      is_active BOOLEAN,
                      created_at TIMESTAMP,
                      last_login TIMESTAMP
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT
            c.id,
            c.msisdn,
            c.full_name,
            c.is_active,
            c.created_at,
            c.last_login
        FROM customers c
        ORDER BY c.created_at DESC;
END;
$$;



CREATE OR REPLACE FUNCTION retrieve_customer_by_msisdn(p_msisdn VARCHAR(15))
    RETURNS TABLE (
                      id UUID,
                      msisdn VARCHAR(15),
                      full_name VARCHAR(100),
                      pin_hash VARCHAR(255),
                      is_active BOOLEAN,
                      created_at TIMESTAMP,
                      last_login TIMESTAMP
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT
            c.id,
            c.msisdn,
            c.full_name,
            c.pin_hash,
            c.is_active,
            c.created_at,
            c.last_login
        FROM customers c
        WHERE c.msisdn = p_msisdn;
END;
$$;


CREATE OR REPLACE FUNCTION get_customer_by_id(p_id UUID)
    RETURNS TABLE (
                      id UUID,
                      msisdn VARCHAR(15),
                      full_name VARCHAR(100),
                      is_active BOOLEAN,
                      created_at TIMESTAMP,
                      last_login TIMESTAMP
                  )
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
        SELECT
            c.id,
            c.msisdn,
            c.full_name,
            c.is_active,
            c.created_at,
            c.last_login
        FROM customers c
        WHERE c.id = p_id;
END;
$$;


CREATE OR REPLACE FUNCTION hash_customer_pin(p_pin VARCHAR(10))
    RETURNS VARCHAR(255)
    LANGUAGE plpgsql
AS $$
BEGIN
    RETURN crypt(p_pin, gen_salt('bf'));
END;
$$;


CREATE OR REPLACE PROCEDURE create_customer(
    p_msisdn VARCHAR(15),
    p_pin_hash VARCHAR(255),
    p_full_name VARCHAR(100),
    OUT p_customer_id UUID
) AS $$
BEGIN
    INSERT INTO customers (msisdn, pin_hash, full_name)
    VALUES (p_msisdn, p_pin_hash, p_full_name)
    RETURNING id INTO p_customer_id;
END;
$$ LANGUAGE plpgsql;



CREATE OR REPLACE FUNCTION verify_customer_pin(p_msisdn VARCHAR(15), p_pin VARCHAR(10))
    RETURNS BOOLEAN
    LANGUAGE plpgsql
AS $$
DECLARE
    v_stored_hash VARCHAR(255);
BEGIN
    SELECT pin_hash INTO v_stored_hash
    FROM customers
    WHERE msisdn = p_msisdn;

    IF v_stored_hash IS NULL THEN
        RETURN FALSE;
    END IF;

    RETURN crypt(p_pin, v_stored_hash) = v_stored_hash;
END;
$$;


CREATE OR REPLACE PROCEDURE change_customer_pin(
    p_customer_id UUID,
    p_new_pin_hash VARCHAR(255)
) AS $$
BEGIN
    UPDATE customers
    SET pin_hash = p_new_pin_hash
    WHERE id = p_customer_id;
END;
$$ LANGUAGE plpgsql;