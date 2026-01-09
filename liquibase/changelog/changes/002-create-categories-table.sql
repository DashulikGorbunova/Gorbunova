--liquibase formatted sql

--changeset appone:002-create-flowers-table
CREATE TABLE flowers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(1000),
    price DECIMAL(18,2) NOT NULL,
    quantity INTEGER NOT NULL DEFAULT 0,
    color VARCHAR(50),
    season VARCHAR(50),
    image_url VARCHAR(500),
    category_id INTEGER,
    is_available BOOLEAN NOT NULL DEFAULT true,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP,
    CONSTRAINT fk_flowers_category FOREIGN KEY (category_id) REFERENCES flower_categories(id) ON DELETE SET NULL
);

CREATE INDEX idx_flowers_name ON flowers(name);
CREATE INDEX idx_flowers_category_id ON flowers(category_id);
CREATE INDEX idx_flowers_is_available ON flowers(is_available);
CREATE INDEX idx_flowers_color ON flowers(color);
CREATE INDEX idx_flowers_season ON flowers(season);

--rollback DROP TABLE flowers;

