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
            Voice = 11
        }

        public enum TitleGenres
        {
            Documentary = 1,
            Short = 2,
            Animation = 3,
            Comedy = 4,
            Romance = 5,
            Sport = 6,
            News = 7,
            Drama = 8,
            Fantasy = 9,
            Horror = 10,
            Biography = 11,
            Music = 12,
            War = 13,
            Crime = 14,
            Western = 15,
            Family = 16,
            Adventure = 17,
            Action = 18,
            History = 19,
            Mystery = 20,
            SciFi = 21,
            Musical = 22,
            Thriller = 23,
            FilmNoir = 24,
            TalkShow = 25,
            GameShow = 26,
            RealityTV = 27,
            Adult = 28
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
