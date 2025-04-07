

CREATE OR REPLACE FUNCTION purchase_airtime(
    p_account_number VARCHAR(20),
    p_phone_number VARCHAR(15),
    p_network VARCHAR(20),
    p_amount DECIMAL(10, 2),
    p_reference VARCHAR(100),
    OUT p_transaction_id UUID,
    OUT p_airtime_purchase_id UUID,
    OUT p_success BOOLEAN,
    OUT p_message VARCHAR(255)
)
    LANGUAGE plpgsql
AS $$
DECLARE
    v_account_id UUID;
    v_customer_id UUID;
BEGIN
    -- Initialize outputs
    p_success := FALSE;
    p_message := '';

    -- Get account details
    SELECT a.id, a.customer_id
    INTO v_account_id, v_customer_id
    FROM accounts a
    WHERE a.account_number = p_account_number AND a.is_active = TRUE;

    IF NOT FOUND THEN
        p_message := 'Account not found or inactive';
        RETURN;
    END IF;

    -- Begin transaction
    BEGIN
        START TRANSACTION;

        -- Create transaction record
        INSERT INTO transactions (
            account_id,
            transaction_type,
            amount,
            reference,
            beneficiary_account,
            status
        ) VALUES (
                     v_account_id,
                     'AIRTIME_PURCHASE',
                     p_amount,
                     p_reference,
                     p_phone_number,
                     'PENDING'
                 ) RETURNING id INTO p_transaction_id;

        -- Create airtime purchase record
        INSERT INTO airtime_purchases (
            customer_id,
            account_id,
            msisdn,
            network_provider,
            amount,
            transaction_id
        ) VALUES (
                     v_customer_id,
                     v_account_id,
                     p_phone_number,
                     p_network,
                     p_amount,
                     p_transaction_id
                 ) RETURNING id INTO p_airtime_purchase_id;

        -- Debit user account
        UPDATE accounts
        SET balance = balance - p_amount,
            available_balance = available_balance - p_amount,
            last_updated = CURRENT_TIMESTAMP
        WHERE id = v_account_id;

        COMMIT;
        p_success := TRUE;
        p_message := 'Airtime purchase initiated successfully';

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            p_success := FALSE;
            p_message := 'Error initiating airtime purchase: ' || SQLERRM;
    END;
END;
$$;



CREATE OR REPLACE FUNCTION handle_airtime_purchase_failure(
    p_transaction_id UUID,
    p_failure_reason VARCHAR(255),
    OUT p_success BOOLEAN,
    OUT p_message VARCHAR(255)
)
    LANGUAGE plpgsql
AS $$
DECLARE
    v_transaction RECORD;
    v_account_id UUID;
    v_amount DECIMAL(10, 2);
BEGIN
    -- Initialize outputs
    p_success := FALSE;
    p_message := '';

    -- Get transaction details
    SELECT t.account_id, t.amount, t.status
    INTO v_transaction
    FROM transactions t
    WHERE t.id = p_transaction_id;

    IF NOT FOUND THEN
        p_message := 'Transaction not found';
        RETURN;
    END IF;

    -- Begin transaction
    BEGIN
        START TRANSACTION;

        -- Update transaction status
        UPDATE transactions
        SET status = 'FAILED'
        WHERE id = p_transaction_id;

        -- Refund the amount to the account
        UPDATE accounts
        SET balance = balance + v_transaction.amount,
            available_balance = available_balance + v_transaction.amount,
            last_updated = CURRENT_TIMESTAMP
        WHERE id = v_transaction.account_id;

        COMMIT;
        p_success := TRUE;
        p_message := 'Airtime purchase failure handled successfully';

    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            p_success := FALSE;
            p_message := 'Error handling airtime purchase failure: ' || SQLERRM;
    END;
END;
$$;