--liquibase formatted sql

--changeset appone:005-create-orders-table
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    user_id INTEGER,
    customer_name VARCHAR(100) NOT NULL,
    customer_email VARCHAR(255),
    delivery_address VARCHAR(500),
    total_amount DECIMAL(18,2) NOT NULL,
    status VARCHAR(50) NOT NULL DEFAULT 'Pending',
    order_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    delivery_date TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_orders_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE SET NULL
);

CREATE INDEX idx_orders_user_id ON orders(user_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_order_date ON orders(order_date);

--rollback DROP TABLE orders;

