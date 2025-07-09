CREATE USER 'imdb_admin'@'%' IDENTIFIED BY '{{SECRET_DB_PASSWORD}}';
GRANT ALL PRIVILEGES ON *.* TO 'imdb_admin'@'%' WITH GRANT OPTION;

CREATE DATABASE IMDB;
USE IMDB;

CREATE TABLE TitleGenre (
    TitleGenreId INT NOT NULL PRIMARY KEY,
    GenreName VARCHAR(24) NOT NULL,
    CONSTRAINT uq_TitleGenre UNIQUE (GenreName)
);

INSERT INTO TitleGenre (TitleGenreId, GenreName) 
VALUES
    (1, 'Documentary'),
    (2, 'Short'),
    (3, 'Animation'),
    (4, 'Comedy'),
    (5, 'Romance'),
    (6, 'Sport'),
    (7, 'News'),
    (8, 'Drama'),
    (9, 'Fantasy'),
    (10, 'Horror'),
    (11, 'Biography'),
    (12, 'Music'),
    (13, 'War'),
    (14, 'Crime'),
    (15, 'Western'),
    (16, 'Family'),
    (17, 'Adventure'),
    (18, 'Action'),
    (19, 'History'),
    (20, 'Mystery'),
    (21, 'Sci-Fi'),
    (22, 'Musical'),
    (23, 'Thriller'),
    (24, 'Film-Noir'),
    (25, 'Talk-Show'),
    (26, 'Game-Show'),
    (27, 'Reality-TV'),
    (28, 'Adult');

CREATE TABLE TitleType (
    TitleTypeId INT NOT NULL PRIMARY KEY,
    TypeName VARCHAR(24) NOT NULL,
    CONSTRAINT uq_TitleGenre UNIQUE (TypeName)
);

INSERT INTO TitleType (TitleTypeId, TypeName) 
VALUES
    (1, 'Short'),
    (2, 'Movie'),
    (3, 'TV Short'),
    (4, 'TV Movie'),
    (5, 'TV Episode'),
    (6, 'TV Series'),
    (7, 'TV Mini-Series'),
    (8, 'TV Special'),
    (9, 'Video'),
    (10, 'Video Game'),
    (11, 'TV Pilot');

CREATE TABLE MediaTitle (
    ImdbId BIGINT PRIMARY KEY,
    TitleTypeId INT NOT NULL,
    PrimaryTitle VARCHAR(255),
    OriginalTitle VARCHAR(255),
    IsAdult BOOLEAN,
    StartYear VARCHAR(4),
    EndYear VARCHAR(4) NULL,
    RuntimeMinutes BIGINT,
    Original_ImdbId VARCHAR(12) NOT NULL,
    CONSTRAINT fk_MediaTitle_TitleTypeId FOREIGN KEY (TitleTypeId) REFERENCES TitleType(TitleTypeId)
);

CREATE TABLE TitleGenreLink (
    TitleGenreLinkId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId BIGINT NOT NULL,
    TitleGenreId INT NOT NULL,
    CONSTRAINT fk_TitleGenreLink_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_TitleGenreLink_TitleGenreId FOREIGN KEY (TitleGenreId) REFERENCES TitleGenre(TitleGenreId)
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

CREATE TABLE TitlePosition (
    PositionId INT NOT NULL PRIMARY KEY,
    PositionName VARCHAR(128) NOT NULL,
    CONSTRAINT uq_TitlePosition UNIQUE (PositionName)
);

INSERT INTO TitlePosition (PositionId, PositionName)
VALUES
    (1, 'Director'),
    (2, 'Writer'),
    (3, 'Producer'),
    (4, 'Actor'),
    (5, 'Cinematographer'),
    (6, 'Composer'),
    (7, 'Editor'),
    (8, 'Production Designer'),
    (9, 'Featured Subject'),
    (10, 'Casting Director'),
    (11, 'Voice');

CREATE TABLE Performance (
    PerformanceId INT AUTO_INCREMENT PRIMARY KEY,
    ImdbId BIGINT NOT NULL,
    PersonImdbId BIGINT NOT NULL,
    PositionId INT NOT NULL,
    Ordering INT NOT NULL,
    PerformanceDescription VARCHAR(255) NULL,
    CONSTRAINT fk_Performance_ImdbId FOREIGN KEY (ImdbId) REFERENCES MediaTitle(ImdbId),
    CONSTRAINT fk_Performance_PersonImdbId FOREIGN KEY (PersonImdbId) REFERENCES Person(PersonImdbId),
    CONSTRAINT fk_Performance_PositionId FOREIGN KEY (PositionId) REFERENCES TitlePosition(PositionId)
);

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
