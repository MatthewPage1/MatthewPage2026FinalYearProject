CREATE TABLE StockMovements (
    MovementId INT AUTO_INCREMENT PRIMARY KEY,
    
    ProductId INT NOT NULL,
    
    ChangeAmount INT NOT NULL,
    
    MovementType VARCHAR(50) NOT NULL,
    
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT FK_StockMovements_Product
        FOREIGN KEY (ProductId)
        REFERENCES products(ProductId)
        ON DELETE CASCADE
);

SELECT * FROM StockMovements