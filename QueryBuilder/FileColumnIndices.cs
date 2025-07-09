namespace IMDB_DB
{
    internal static class FileColumnIndices
    {
        public enum TitleBasicsIndices
        {
            ImdbId = 0,
            Type,
            PrimaryTitle,
            OriginalTitle,
            IsAdult,
            StartYear,
            EndYear,
            RuntimeMinutes,
            Genres,
        }

        public enum PersonIndices
        {
            PersonId = 0,
            PrimaryName,
            BirthYear,
            DeathYear,
            PrimaryProfession,
            KnownForTitles
        }
    }
}
