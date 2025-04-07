DROP TABLE IF EXISTS bill_payments CASCADE ;
DROP TABLE IF EXISTS airtime_purchases CASCADE ;
DROP TABLE IF EXISTS transactions CASCADE ;
DROP TABLE IF EXISTS accounts CASCADE ;
DROP TABLE IF EXISTS customers CASCADE ;

CREATE TABLE customers (
       id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
       msisdn VARCHAR(15) UNIQUE NOT NULL,
       pin_hash VARCHAR(255) NOT NULL,
       full_name VARCHAR(100) NOT NULL,
       is_active BOOLEAN DEFAULT TRUE,
       created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
       last_login TIMESTAMP,
       CONSTRAINT valid_msisdn CHECK (msisdn ~ '^233[0-9]{9}$') 
    );


CREATE TABLE account_types (
   account_type_id SERIAL PRIMARY KEY,
   account_type_name VARCHAR(20) UNIQUE NOT NULL
);

INSERT INTO account_types (account_type_name) VALUES
  ('SAVINGS'),
  ('CURRENT'),
  ('FIXED_DEPOSIT');

CREATE TABLE accounts (
      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
      customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
      account_number VARCHAR(20) UNIQUE NOT NULL,
      account_type_id INTEGER NOT NULL REFERENCES account_types(account_type_id),
      balance DECIMAL(15, 2) DEFAULT 0.00 CHECK (balance >= 0),
      available_balance DECIMAL(15, 2) DEFAULT 0.00 CHECK (available_balance >= 0 AND available_balance <= balance),
      created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      is_active BOOLEAN DEFAULT TRUE,
      last_updated TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      CONSTRAINT valid_account_number CHECK (account_number ~ '^[0-9]{10}$')
);

CREATE TABLE transactions (
      id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
      account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
      transaction_type VARCHAR(20) NOT NULL CHECK (transaction_type IN (
        'DEPOSIT', 'WITHDRAWAL', 'TRANSFER', 'BILL_PAYMENT', 'AIRTIME_PURCHASE', 'FEE'
          )),
      amount DECIMAL(15, 2) NOT NULL CHECK (amount > 0),
      reference VARCHAR(100),
      beneficiary_account VARCHAR(20),
      status VARCHAR(20) NOT NULL CHECK (status IN (
                                                    'PENDING', 'COMPLETED', 'FAILED', 'REVERSED'
          )),
      transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
      reversal_reference UUID REFERENCES transactions(id)
      
);


CREATE TABLE airtime_purchases (
   id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
   customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
   account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
   msisdn VARCHAR(15) NOT NULL,
   network_provider VARCHAR(20) NOT NULL CHECK (network_provider IN (
        'MTN', 'VODAFONE', 'AIRTELTIGO', 'GLO'
    )),
   amount DECIMAL(10, 2) NOT NULL CHECK (amount > 0),
   transaction_id UUID REFERENCES transactions(id),
   transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE bill_payments (
   id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
   customer_id UUID NOT NULL REFERENCES customers(id) ON DELETE CASCADE,
   account_id UUID NOT NULL REFERENCES accounts(id) ON DELETE CASCADE,
   bill_type VARCHAR(30) NOT NULL CHECK (bill_type IN (
       'ELECTRICITY', 'WATER', 'INTERNET', 'TV_SUBSCRIPTION', 'RENT'
   )),
   account_number VARCHAR(50) NOT NULL,
   amount DECIMAL(10, 2) NOT NULL CHECK (amount > 0),
   transaction_id UUID REFERENCES transactions(id),
   transaction_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);