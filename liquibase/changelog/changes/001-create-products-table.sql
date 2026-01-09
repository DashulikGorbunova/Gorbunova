--liquibase formatted sql

--changeset appone:001-create-categories-table
CREATE TABLE flower_categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(1000),
    is_active BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP
);

CREATE INDEX idx_flower_categories_name ON flower_categories(name);
CREATE INDEX idx_flower_categories_is_active ON flower_categories(is_active);

--rollback DROP TABLE flower_categories;

