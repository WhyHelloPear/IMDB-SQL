CREATE USER 'imdb_admin'@'%' IDENTIFIED BY '{{SECRET_DB_PASSWORD}}';
GRANT ALL PRIVILEGES ON *.* TO 'imdb_admin'@'%' WITH GRANT OPTION;

CREATE DATABASE IMDB;
USE IMDB;

CREATE TABLE MediaTitle (
    ImdbId BIGINT PRIMARY KEY,
    TitleType VARCHAR(32),
    PrimaryTitle VARCHAR(255),
    OriginalTitle VARCHAR(255),
    IsAdult BOOLEAN,
    StartYear VARCHAR(4),
    EndYear VARCHAR(4) NULL,
    RuntimeMinutes BIGINT,
    Genres VARCHAR(255),
    Original_ImdbId VARCHAR(12) NOT NULL
);

CREATE TABLE TitleRating (
    RatingId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId BIGINT NOT NULL,
    Rating DECIMAL(4,2) UNSIGNED,
    NumVotes BIGINT,
    CONSTRAINT fk_TitleRating_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId)
);

CREATE TABLE Person (
    PersonImdbId BIGINT PRIMARY KEY,
    PrimaryName VARCHAR(255),
    BirthYear VARCHAR(4),
    DeathYear VARCHAR(4),
    PrimaryProfession VARCHAR(255),
    KnownForTitles VARCHAR(255),
    Original_PersonImdbId VARCHAR(12) NOT NULL
);

CREATE TABLE Performance (
    PerformanceId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId BIGINT NOT NULL,
    PersonImdbId BIGINT NOT NULL,
    Ordering INT NOT NULL,
    Category VARCHAR(64) NOT NULL,
    Job VARCHAR(255) NULL,
    Characters VARCHAR(255) NULL,
    CONSTRAINT fk_Performance_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_Performance_PersonImdbId FOREIGN KEY (PersonImdbId) REFERENCES Person(PersonImdbId)
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
    ImdbId BIGINT NOT NULL,
    PersonImdbId BIGINT NOT NULL,
    PositionId INT NOT NULL,
    CONSTRAINT uq_TitlePersonPosition UNIQUE (ImdbId, PersonImdbId, PositionId),
    CONSTRAINT fk_TitlePersonPosition_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_TitlePersonPosition_PersonImdbId FOREIGN KEY (PersonImdbId) REFERENCES Person(PersonImdbId),
    CONSTRAINT fk_TitlePersonPosition_PositionId FOREIGN KEY (PositionId) REFERENCES TitlePosition(PositionId)
);

CREATE TABLE TitleAlias (
    TitleAliasId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId BIGINT NOT NULL,
    Ordering INT NOT NULL,
    TitleAlias VARCHAR(255) NOT NULL,
    AliasRegion VARCHAR(56) NULL,
    AliasLanguage VARCHAR(128) NULL,
    AliasType VARCHAR(255) NULL,
    AliasAttributes VARCHAR(255) NULL,
    IsOriginalTitle BOOLEAN NOT NULL,
    CONSTRAINT fk_TitleAlias_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId)
);

CREATE TABLE TitleEpisode (
    TitleEpisodeId INT AUTO_INCREMENT PRIMARY KEY,
    EpisodeImdbId BIGINT NOT NULL,
    SeriesImdbId BIGINT NOT NULL,
    SeasonNumber INT NULL,
    EpisodeNumber INT NULL,
    CONSTRAINT fk_TitleEpisode_EpisodeImdbId FOREIGN KEY (EpisodeImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_TitleEpisode_SeriesImdbId FOREIGN KEY (SeriesImdbId) REFERENCES MediaTitle(ImdbId)
);

FLUSH PRIVILEGES;
