-- Create the database
CREATE DATABASE IF NOT EXISTS librarydb;
USE librarydb;

-- Drop existing tables if needed (for re-creation during dev)
DROP TABLE IF EXISTS Books;
DROP TABLE IF EXISTS Authors;

-- Create Authors table
CREATE TABLE Authors (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Nationality VARCHAR(100)
);

-- Create Books table
CREATE TABLE Books (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Genre VARCHAR(100),
    PublishedOn DATE,
    AuthorId INT,
    FOREIGN KEY (AuthorId) REFERENCES Authors(Id)
        ON DELETE CASCADE
        ON UPDATE CASCADE
);
