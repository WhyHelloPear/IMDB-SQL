namespace IMDB_DB
{
    public static class RatingFileSchema
    {
        public const int ImdbIdIndex = 0;
        public const int RatingIndex = 1;
        public const int NumVotesIndex = 2;

        public const string FileName = "title.ratings.tsv";

        public const string SqlTableName = "dbo.Ratings";

        public const string ImdbIdColName = "ImdbId";
        public const string RatingColName = "Rating";
        public const string NumVotesColName = "NumVotes";

        public const string ImdbIdColType = "VARCHAR(16)";
        public const string RatingColType = "DECIMAL UNSIGNED ZEROFILL";
        public const string NumVotesColType = "INT";
    }
}
