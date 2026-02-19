-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `mydb` DEFAULT CHARACTER SET utf8 ;
USE `mydb` ;

-- -----------------------------------------------------
-- Table `mydb`.`Group`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Group` (
  `GroupID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  PRIMARY KEY (`GroupID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Products`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Products` (
  `ProductID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Barcode` VARCHAR(45) NULL,
  `Unit` VARCHAR(45) NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `SellingPrice` DECIMAL(10,2) NULL,
  `ReorderLevel` INT NULL,
  `Group_GroupID` INT NOT NULL,
  PRIMARY KEY (`ProductID`),
  INDEX `fk_Products_Group_idx` (`Group_GroupID` ASC) VISIBLE,
  CONSTRAINT `fk_Products_Group`
    FOREIGN KEY (`Group_GroupID`)
    REFERENCES `mydb`.`Group` (`GroupID`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`Supplier`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`Supplier` (
  `SupplierID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Phone` VARCHAR(45) NULL,
  `Email` VARCHAR(45) NULL,
  `Address` VARCHAR(45) NULL,
  PRIMARY KEY (`SupplierID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`SupplierTransaction`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`SupplierTransaction` (
  `SupplierTransactionID` INT NOT NULL,
  `TotalPrice` DECIMAL(10,2) NULL,
  `TransactionDate` DATETIME NULL,
  `Invoice` INT NULL,
  PRIMARY KEY (`SupplierTransactionID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`SupplierTransactionItem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`SupplierTransactionItem` (
  `SupplierTransactionItemID` INT NOT NULL,
  `Quantity` INT NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `Total` DECIMAL(10,2) NULL,
  PRIMARY KEY (`SupplierTransactionItemID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`CustomerTransaction`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`CustomerTransaction` (
  `CustomerTransactionID` INT NOT NULL,
  `Total` DECIMAL(10,2) NULL,
  `Date` DATETIME NULL,
  PRIMARY KEY (`CustomerTransactionID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`CustomerTransactionItem`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`CustomerTransactionItem` (
  `CustomerTransactionItemID` INT NOT NULL,
  `Quantity` INT NULL,
  `Price` DECIMAL(10,2) NULL,
  `Total` DECIMAL(10,2) NULL,
  PRIMARY KEY (`CustomerTransactionItemID`))
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `mydb`.`ProductsTest`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `mydb`.`ProductsTest` (
  `ProductID` INT NOT NULL,
  `Name` VARCHAR(45) NULL,
  `Barcode` VARCHAR(45) NULL,
  `Unit` VARCHAR(45) NULL,
  `CostPrice` DECIMAL(10,2) NULL,
  `SellingPrice` DECIMAL(10,2) NULL,
  `ReorderLevel` INT NULL,
  PRIMARY KEY (`ProductID`))
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
