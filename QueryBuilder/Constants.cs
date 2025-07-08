namespace IMDB_DB
{
    public static class Constants
    {
        public enum TitlePositions
        {
            Director = 1,
            Writer = 2
        }

        public static class ImdbIdPrefix
        {
            public const string Title = "tt";
            public const string Person = "nm";
        }

        public static class DataParsing
        {
            public const char DELIMITER = '\t';
        }
    }
}
