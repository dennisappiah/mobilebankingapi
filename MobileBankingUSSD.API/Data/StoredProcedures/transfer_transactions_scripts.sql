
CREATE OR REPLACE FUNCTION transfer_funds(
    p_from_account_id UUID,
    p_to_account_id UUID,
    p_amount DECIMAL(15, 2),
    p_reference VARCHAR(100),
    OUT p_transaction_id UUID,
    OUT p_success BOOLEAN,
    OUT p_message VARCHAR(255)
)
    LANGUAGE plpgsql
AS $$
BEGIN
    -- Initialize outputs
    p_success := FALSE;
    p_message := '';

    -- Begin transaction
    BEGIN
        START TRANSACTION;

        -- Create transfer transaction record (debit from sender)
        INSERT INTO transactions (
            account_id,
            transaction_type,
            amount,
            reference,
            beneficiary_account,
            status,
            transaction_date
        ) VALUES (
                     p_from_account_id,
                     'TRANSFER',
                     p_amount,
                     p_reference,
                     (SELECT account_number FROM accounts WHERE id = p_to_account_id),
                     'COMPLETED',
                     CURRENT_TIMESTAMP
                 ) RETURNING id INTO p_transaction_id;

        -- Debit sender account
        UPDATE accounts
        SET balance = balance - p_amount,
            available_balance = available_balance - p_amount,
            last_updated = CURRENT_TIMESTAMP
        WHERE id = p_from_account_id;

        -- Credit recipient account
        UPDATE accounts
        SET balance = balance + p_amount,
            available_balance = available_balance + p_amount,
            last_updated = CURRENT_TIMESTAMP
        WHERE id = p_to_account_id;

        -- Record the deposit transaction for the recipient
        INSERT INTO transactions (
            account_id,
            transaction_type,
            amount,
            reference,
            status,
            transaction_date
        ) VALUES (
                     p_to_account_id,
                     'DEPOSIT',
                     p_amount,
                     p_reference || '-RCVD',
                     'COMPLETED',
                     CURRENT_TIMESTAMP
                 );

        COMMIT;

        p_success := TRUE;
        p_message := 'Transfer completed successfully';
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            p_success := FALSE;
            p_message := 'Error processing transfer: ' || SQLERRM;
    END;
END;
$$;



CREATE OR REPLACE FUNCTION reverse_transaction(
    p_transaction_id UUID,
    p_reversal_reference VARCHAR(100),
    OUT p_reversal_id UUID,
    OUT p_success BOOLEAN,
    OUT p_message VARCHAR(255)
)
    LANGUAGE plpgsql
AS $$
DECLARE
    v_transaction RECORD;
    v_account_id UUID;
    v_amount DECIMAL(15, 2);
    v_transaction_type VARCHAR(20);
BEGIN
    -- Initialize outputs
    p_success := FALSE;
    p_message := '';
    p_reversal_id := NULL;

    -- Get transaction details
    SELECT
        t.account_id,
        t.amount,
        t.transaction_type
    INTO v_transaction
    FROM transactions t
    WHERE t.id = p_transaction_id;

    IF NOT FOUND THEN
        p_message := 'Transaction not found';
        RETURN;
    END IF;

    v_account_id := v_transaction.account_id;
    v_amount := v_transaction.amount;
    v_transaction_type := v_transaction.transaction_type;

    -- Begin transaction
    BEGIN
        START TRANSACTION;

        -- Handle different transaction types
        CASE v_transaction_type
            WHEN 'TRANSFER' THEN
                -- Find beneficiary account ID
                DECLARE
                    v_benef_account_id UUID;
                BEGIN
                    SELECT id INTO v_benef_account_id
                    FROM accounts
                    WHERE account_number = (SELECT beneficiary_account FROM transactions WHERE id = p_transaction_id) AND is_active = TRUE;

                    -- Reverse the transfer
                    -- 1. Debit beneficiary
                    UPDATE accounts
                    SET balance = balance - v_amount,
                        available_balance = available_balance - v_amount,
                        last_updated = CURRENT_TIMESTAMP
                    WHERE id = v_benef_account_id;

                    -- 2. Credit original account
                    UPDATE accounts
                    SET balance = balance + v_amount,
                        available_balance = available_balance + v_amount,
                        last_updated = CURRENT_TIMESTAMP
                    WHERE id = v_account_id;

                    -- Create reversal transaction record
                    INSERT INTO transactions (
                        account_id,
                        transaction_type,
                        amount,
                        reference,
                        beneficiary_account,
                        status,
                        transaction_date,
                        reversal_reference
                    ) VALUES (
                                 v_benef_account_id,
                                 'TRANSFER',
                                 v_amount,
                                 p_reversal_reference,
                                 (SELECT account_number FROM accounts WHERE id = v_account_id),
                                 'COMPLETED',
                                 CURRENT_TIMESTAMP,
                                 p_transaction_id
                             ) RETURNING id INTO p_reversal_id;

                    -- Create deposit record for the reversal
                    INSERT INTO transactions (
                        account_id,
                        transaction_type,
                        amount,
                        reference,
                        status,
                        transaction_date,
                        reversal_reference
                    ) VALUES (
                                 v_account_id,
                                 'DEPOSIT',
                                 v_amount,
                                 p_reversal_reference || '-RCVD',
                                 'COMPLETED',
                                 CURRENT_TIMESTAMP,
                                 p_transaction_id
                             );
                END;

            WHEN 'DEPOSIT' THEN
                -- Reverse deposit
                UPDATE accounts
                SET balance = balance - v_amount,
                    available_balance = available_balance - v_amount,
                    last_updated = CURRENT_TIMESTAMP
                WHERE id = v_account_id;

                -- Create reversal transaction record
                INSERT INTO transactions (
                    account_id,
                    transaction_type,
                    amount,
                    reference,
                    status,
                    transaction_date,
                    reversal_reference
                ) VALUES (
                             v_account_id,
                             'WITHDRAWAL',
                             v_amount,
                             p_reversal_reference,
                             'COMPLETED',
                             CURRENT_TIMESTAMP,
                             p_transaction_id
                         ) RETURNING id INTO p_reversal_id;

            WHEN 'WITHDRAWAL' THEN
                -- Reverse withdrawal
                UPDATE accounts
                SET balance = balance + v_amount,
                    available_balance = available_balance + v_amount,
                    last_updated = CURRENT_TIMESTAMP
                WHERE id = v_account_id;

                -- Create reversal transaction record
                INSERT INTO transactions (
                    account_id,
                    transaction_type,
                    amount,
                    reference,
                    status,
                    transaction_date,
                    reversal_reference
                ) VALUES (
                             v_account_id,
                             'DEPOSIT',
                             v_amount,
                             p_reversal_reference,
                             'COMPLETED',
                             CURRENT_TIMESTAMP,
                             p_transaction_id
                         ) RETURNING id INTO p_reversal_id;

            ELSE
                RAISE EXCEPTION 'Reversal not supported for transaction type: %', v_transaction_type;
            END CASE;

        -- Update original transaction status
        UPDATE transactions
        SET status = 'REVERSED'
        WHERE id = p_transaction_id;

        COMMIT;

        p_success := TRUE;
        p_message := 'Transaction reversed successfully';
    EXCEPTION
        WHEN OTHERS THEN
            ROLLBACK;
            p_success := FALSE;
            p_message := 'Error reversing transaction: ' || SQLERRM;
    END;
END;
$$;