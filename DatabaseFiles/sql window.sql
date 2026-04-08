-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema finalyearproject
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema finalyearproject
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `finalyearproject` DEFAULT CHARACTER SET utf8 ;
USE `finalyearproject` ;

-- -----------------------------------------------------
-- Table `finalyearproject`.`Group`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`Group` (
  `GroupID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  PRIMARY KEY (`GroupID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`Products`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`Products` (
  `ProductID` INT NOT NULL AUTO_INCREMENT,
  `ProductName` VARCHAR(45) NULL,
  `GTIN13` VARCHAR(45) NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `SellingPrice` DECIMAL(10,2) NULL,
  `StockCount` INT NULL,
  `ReorderLevel` INT NULL,
  `Group_GroupID` INT NOT NULL,
  PRIMARY KEY (`ProductID`),
  INDEX `fk_Products_Group_idx` (`Group_GroupID` ASC) VISIBLE,
  CONSTRAINT `fk_Products_Group`
    FOREIGN KEY (`Group_GroupID`)
    REFERENCES `finalyearproject`.`Group` (`GroupID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`Supplier`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`Supplier` (
  `SupplierID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Phone` VARCHAR(45) NULL,
  `Email` VARCHAR(45) NULL,
  `Address` VARCHAR(45) NULL,
  PRIMARY KEY (`SupplierID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`SupplierTransaction`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`SupplierTransaction` (
  `SupplierTransactionID` INT NOT NULL,
  `TotalPrice` DECIMAL(10,2) NULL,
  `TransactionDate` DATETIME NULL,
  `Invoice` INT NULL,
  PRIMARY KEY (`SupplierTransactionID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`SupplierTransactionItem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`SupplierTransactionItem` (
  `SupplierTransactionItemID` INT NOT NULL,
  `Quantity` INT NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `Total` DECIMAL(10,2) NULL,
  PRIMARY KEY (`SupplierTransactionItemID`))
ENGINE = InnoDB;

--
-- -----------------------------------------------------
-- Table `finalyearproject`.`CustomerTransaction`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`CustomerTransaction` (
  `CustomerTransactionID` INT NOT NULL,
  `Total` DECIMAL(10,2) NULL,
  `Date` DATETIME NULL,
  PRIMARY KEY (`CustomerTransactionID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`CustomerTransactionItem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`CustomerTransactionItem` (
  `CustomerTransactionItemID` INT NOT NULL,
  `Quantity` INT NULL,
  `Price` DECIMAL(10,2) NULL,
  `Total` DECIMAL(10,2) NULL,
  PRIMARY KEY (`CustomerTransactionItemID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `finalyearproject`.`ProductsTest`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `finalyearproject`.`Products` (
  `ProductID` INT NOT NULL AUTO_INCREMENT,
  `ProductName` VARCHAR(200) NULL,
  `GTIN13` VARCHAR(20) NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `SellingPrice` DECIMAL(10,2) NULL,
  `StockCount` INT NULL,
  `Availability` VARCHAR(45) NULL,
  `ProductDescription` VARCHAR(2000) Null,
  `Brand` VARCHAR(200) NULL,
  `ProductGroup1` VARCHAR(100) NULL,
  `ProductGroup2` VARCHAR(100) NULL,
  `ProductGroup3` VARCHAR(100) NULL,
  `Image` VARCHAR(1000) NULL,
  `ReorderLevel` INT NULL,
  PRIMARY KEY (`ProductID`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;



INSERT INTO productstest (productid, productname, gtin13, costprice, sellingprice, stockcount, reorderlevel)
VALUES (1, 'TestName', 019247103587, 10.00, 50, 10 ,3);

SHOW DATABASES;
USE finalyearproject;
SHOW TABLES;


LOAD DATA LOCAL INFILE 'C:\\JsonData\\DatabaseFiles\\tesco_groceries_dataset_cutdown_tab_delimited.txt'
INTO TABLE products
FIELDS TERMINATED BY '\t'
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(@name_csv, @gtin13_csv, @price_csv, @availability_csv, @description_csv, @brand_csv, @breadcrumbsgroup1_csv, @breadcrumbsgroup2_csv, @breadcrumbsgroup3_csv, @images_csv)
SET ProductName = @name_csv,
	GTIN13 = @gtin13_csv, 
    CostPrice = @price_csv,
    SellingPrice = (@price_csv * 1.2),
    StockCount = (rand() * 100),
    Availability = @availability_csv,
	ProductDescription = @description_csv,
    Brand = @brand_csv,
    ProductGroup1 = @breadcrumbsgroup1_csv,
    ProductGroup2 = @breadcrumbsgroup2_csv,
    ProductGroup3 = @breadcrumbsgroup3_csv,
	Image =  @images_csv,
	ReorderLevel = 10;
    
SET GLOBAL local_infile = 1;

truncate table Products;
SET GLOBAL local_infile = 1;
SELECT * FROM Products;
DROP TABLE Products;


LOAD DATA LOCAL INFILE 'C:\\Users\\matth\\OneDrive - University of Lincoln\\source\\repos\\MatthewPage2026\\DatabaseFiles\\tesco_groceries_dataset_cutdown_tab_delimited_updated.txt'
INTO TABLE products
FIELDS TERMINATED BY '\t'
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(@name_csv, @gtin13_csv, @price_csv, @availability_csv, @description_csv, @brand_csv, @breadcrumbsgroup1_csv, @breadcrumbsgroup2_csv, @breadcrumbsgroup3_csv, @images_csv)
SET ProductName = @name_csv,
	GTIN13 = @gtin13_csv, 
    CostPrice = @price_csv,
    SellingPrice = (@price_csv * 1.2),
    StockCount = (rand() * 100),
    Availability = @availability_csv,
	ProductDescription = @description_csv,
    Brand = @brand_csv,
    ProductGroup1 = @breadcrumbsgroup1_csv,
    ProductGroup2 = @breadcrumbsgroup2_csv,
    ProductGroup3 = @breadcrumbsgroup3_csv,
	Image =  @images_csv,
	ReorderLevel = 10;

SELECT * FROM Products;
SELECT * FROM Supplier;
SELECT * FROM suppliertransaction;

SHOW TABLES;
describe products;
describe productstest;

CREATE TABLE SupplierTransaction (
    TransactionID INT AUTO_INCREMENT PRIMARY KEY,
    Quantity INT NOT NULL,
    CostPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    TransactionDate DATE NOT NULL,
    DeliveryDate DATE NOT NULL,
    Processed BOOLEAN DEFAULT FALSE,
    CheckedIn BOOLEAN DEFAULT FALSE,
    SupplierID INT NOT NULL,
    ProductID INT NOT NULL,

    CONSTRAINT fk_supplier
        FOREIGN KEY (SupplierID)
        REFERENCES Supplier(SupplierID),

    CONSTRAINT fk_product
        FOREIGN KEY (ProductID)
        REFERENCES Products(ProductID)
);

SELECT * FROM SupplierTransaction;
DROP TABLE SupplierTransaction;
INSERT INTO suppliertransaction
(Quantity, CostPrice, TotalPrice, TransactionDate, DeliveryDate, Processed, CheckedIn, SupplierID, ProductID)
VALUES
(10, 1.54, 15.40, '2026-03-04', '2026-03-07', True, False, 1, 14);

SELECT * FROM supplier;
SELECT * FROM suppliertransaction;

Drop Table suppliertransaction;




SELECT * FROM products;

CREATE TABLE Sales (
    SaleID INT Auto_Increment PRIMARY KEY,
    Quantity INT NOT NULL,
    SellingPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    SaleDate DATETIME NOT NULL,
	ProductID INT NOT NULL,

    FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);

SELECT * FROM Sales;

SELECT SUM(TotalPrice) FROM Sales;
truncate TABLE sales;
truncate TABLE suppliertransaction;


CREATE TABLE SimulationHistory (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Day INT NOT NULL,
    Balance DECIMAL(10,2) NOT NULL,
    Revenue DECIMAL(10,2) DEFAULT 0,
    Costs DECIMAL(10,2) DEFAULT 0,
    Profit DECIMAL(10,2) GENERATED ALWAYS AS (Revenue - Costs) STORED,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE SimulationHistory
ADD UNIQUE (Day);

SELECT * FROM SimulationHistory;
Select * from SupplierTransaction;
select * from sales;

Truncate Table SimulationHistory;
Truncate Table SupplierTransaction;
Truncate Table Sales;

CREATE TABLE Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARCHAR(255) NOT NULL
);

ALTER TABLE Users
RENAME COLUMN CustomerID TO UserId;

SELECT * FROM users;

INSERT INTO Users (Username, PasswordHash)
VALUES('User1', 'Password1'),('User2', 'Password2');

ALTER TABLE Products
ADD COLUMN UserId INT NOT NULL;

ALTER TABLE Products
ADD INDEX idx_userid (UserId);

UPDATE Products
SET UserId = 1
WHERE ProductId IS NOT NULL;

select * from products order by userID;



select * from users;

describe suppliertransaction;
describe products;
describe users;

ALTER TABLE SimulationHistory
ADD COLUMN UserId INT NOT NULL;

ALTER TABLE simulationhistory
ADD INDEX idx_userid (UserId);

UPDATE SimulationHistory
SET UserId = 1
WHERE Id IS NOT NULL;

ALTER TABLE suppliertransaction
ADD COLUMN UserId INT NOT NULL;

ALTER TABLE suppliertransaction
ADD INDEX idx_userid (UserId);

UPDATE suppliertransaction
SET UserId = 1
WHERE UserId IS NOT NULL;

SELECT * FRoM suppliertransaction;
SELECT * FROM simulationhistory;


ALTER TABLE SimulationHistory
ADD CONSTRAINT unique_user_day UNIQUE (UserId, Day);

Describe simulationhistory;

SELECT * FROM SAles;

ALTER TABLE sales
ADD COLUMN UserId INT NOT NULL;

ALTER TABLE sales
ADD INDEX idx_userid (UserId);

UPDATE sales
SET UserId = 1
WHERE UserId IS NOT NULL;

ALTER TABLE stockmovements
ADD COLUMN UserId INT NOT NULL;

ALTER TABLE stockmovements
ADD INDEX idx_userid (UserId);

UPDATE stockmovements
SET UserId = 1
WHERE UserId IS NOT NULL;

SELECT * FROM stockmovements;

truncate table sales;
truncate table simulationhistory;
truncate table suppliertransaction;

SELECT * FROM simulationhistory;
SELECT * FROM sales;
SELECT * FROM suppliertransaction;

SELECT * FROM users;
ALTER TABLE Users
ADD COLUMN DisplayName VARCHAR(255);

SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE Users;

SET FOREIGN_KEY_CHECKS = 1;

INSERT INTO Users (UserId, Username, PasswordHash, DisplayName)
VALUES 
(1, 'User1', 'Password1', 'User One'),
(2, 'User2', 'Password2', 'User Two'),
(3, 'User3', 'Password3', 'User Three');

select * from users;

-- hashed password = $2a$12$Zcjrm6Do8QvoC7qG8/OZ5OxCtiOnIefXdlqdoKbLb7uuVhrnVi.6u
-- hashed password = $2a$12$bbywgHL5cVdI9i5yrVHKV.SyKatZmEcz3/s8qdZuNB9tRV1L6wzZe
-- hashed password = $2a$12$YqdMFxjbxOc6J3NlIVSR1upGSzE58Y9rEPZlXtJ8tS/RtiG1SEkCW

SELECT * FROM suppliertransaction;
SELECT * FROM simulationhistory;
SELECT * FROM SALES;

ALTER TABLE products
ADD COLUMN Promotion boolean NOT NULL;

ALTER TABLE products
ADD COLUMN OriginalSellingPrice decimal(10,2) NOT NULL;


UPDATE Products
SET OriginalSellingPrice = SellingPrice;


SELECT * FROM products;