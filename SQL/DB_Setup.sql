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

CREATE TABLE Performance (
    PerformanceId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId VARCHAR(12) NOT NULL,
    Ordering INT NOT NULL,
    PersonImdbId VARCHAR(12) NOT NULL,
    Category VARCHAR(64) NOT NULL,
    Job VARCHAR(255) NULL,
    Characters VARCHAR(255) NULL,
    CONSTRAINT fk_Performance_MediaTitle_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_Performance_Person_PersonImdbId FOREIGN KEY (PersonImdbId) REFERENCES Person(PersonImdbId)
);

CREATE TABLE TitlePosition (
    PositionId INT NOT NULL PRIMARY KEY,
    PositionName VARCHAR(128) NOT NULL,
    CONSTRAINT uq_TitlePosition UNIQUE (PositionName)
);

INSERT INTO TitlePosition (PositionId, PositionName)
VALUES
    (1, 'Director'),
    (2, 'Writer');

CREATE TABLE TitlePersonPosition (
    TitlePersonPositionId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId VARCHAR(12) NOT NULL,
    PersonImdbId VARCHAR(12) NOT NULL,
    PositionId INT NOT NULL,
    CONSTRAINT uq_TitlePersonPosition UNIQUE (ImdbId, PersonImdbId, PositionId),
    CONSTRAINT fk_TitlePersonPosition_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_TitlePersonPosition_PersonImdbId FOREIGN KEY (PersonImdbId) REFERENCES Person(PersonImdbId),
    CONSTRAINT fk_TitlePersonPosition_PositionId FOREIGN KEY (PositionId) REFERENCES TitlePosition(PositionId)
);

FLUSH PRIVILEGES;
