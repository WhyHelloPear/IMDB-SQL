CREATE USER 'imdb_admin'@'%' IDENTIFIED BY '{{SECRET_DB_PASSWORD}}';
GRANT ALL PRIVILEGES ON *.* TO 'imdb_admin'@'%' WITH GRANT OPTION;

CREATE DATABASE IMDB;
USE IMDB;

CREATE TABLE MediaTitle (
    ImdbId VARCHAR(12) PRIMARY KEY,
    TitleType VARCHAR(32),
    PrimaryTitle VARCHAR(255),
    OriginalTitle VARCHAR(255),
    IsAdult BOOLEAN,
    StartYear VARCHAR(4),
    EndYear VARCHAR(4) NULL,
    RuntimeMinutes BIGINT,
    Genres VARCHAR(255)
);

CREATE TABLE TitleRating (
    RatingId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId VARCHAR(12) NOT NULL,
    Rating DECIMAL(4,2) UNSIGNED,
    NumVotes BIGINT,
    CONSTRAINT fk_TitleRating_MediaTitle_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId)
);

CREATE TABLE Person (
    PersonImdbId VARCHAR(12) PRIMARY KEY,
    PrimaryName VARCHAR(255),
    BirthYear VARCHAR(4),
    DeathYear VARCHAR(4),
    PrimaryProfession VARCHAR(255),
    KnownForTitles VARCHAR(255)
);

FLUSH PRIVILEGES;
