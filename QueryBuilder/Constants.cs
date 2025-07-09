namespace IMDB_DB
{
    public static class Constants
    {
        public enum TitlePositions
        {
            Director = 1,
            Writer = 2,
            Producer = 3,
            Actor = 4,
            Cinematographer = 5,
            Composer = 6,
            Editor = 7,
            ProductionDesigner = 8,
            FeaturedSubject = 9,
            CastingDirector = 10,
            Voice = 11,
        }

        public static class ImdbIdPrefix
        {
            public const string Title = "tt";
            public const string Person = "nm";
        }

        public static class DataParsing
        {
            public const char DELIMITER = '\t';
            public const string NULL = @"\N";
        }
    }
}
