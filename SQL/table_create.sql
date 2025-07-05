CREATE TABLE MediaTitle (
    ImdbId VARCHAR(12) PRIMARY KEY,
    TitleType VARCHAR(32),
    PrimaryTitle VARCHAR(255),
    OriginalTitle VARCHAR(255),
    IsAdult BOOLEAN,
    StartYear VARCHAR(4),
    EndYear VARCHAR(4) NULL,
    RuntimeMinutes SMALLINT,
    Genres VARCHAR(255)
);
