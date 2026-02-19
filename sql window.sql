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
CREATE TABLE IF NOT EXISTS `finalyearproject`.`ProductsTest` (
  `ProductID` INT NOT NULL AUTO_INCREMENT,
  `ProductName` VARCHAR(45) NULL,
  `GTIN13` VARCHAR(45) NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `SellingPrice` DECIMAL(10,2) NULL,
  `StockCount` INT NULL,
  `Availability` VARCHAR(45) NULL,
  `ProductDescription` VARCHAR(256) Null,
  `Brand` VARCHAR(45) NULL,
  `ProductGroup1` VARCHAR(45) NULL,
  `ProductGroup2` VARCHAR(45) NULL,
  `ProductGroup3` VARCHAR(45) NULL,
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


LOAD DATA LOCAL INFILE 'C:\\JsonData\\tesco_groceries_dataset_cutdown.csv'
INTO TABLE productstest
FIELDS TERMINATED BY ','
LINES TERMINATED BY '\n'
IGNORE 1 ROWS
(@name_csv, @gtin13_csv, @price_csv, @availability_csv, @description_csv, @brand_csv, @breadcrumbsgroup1_csv, @breadcrumbsgroup2_csv, @breadcrumbsgroup3_csv)
SET ProductName = @name_csv,
	GTIN13 = @gtin13_csv, 
    CostPrice = @price_csv,
    SellingPrice = (@price_csv * 0.8),
    StockCount = (rand() * 100),
    Availability = @availability_csv,
	ProductDescription = @description_csv,
    Brand = @brand_csv,
    ProductGroup1 = @breadcrumbsgroup1_csv,
    ProductGroup2 = @breadcrumbsgroup2_csv,
    ProductGroup3 = @breadcrumbsgroup3_csv,
    ReorderLevel = 10;

SET GLOBAL local_infile = 1;

truncate table ProductsTest
DROP TABLE productstest;
SELECT * FROM productstest;