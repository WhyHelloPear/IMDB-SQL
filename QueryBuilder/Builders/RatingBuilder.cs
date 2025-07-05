using IMDB_DB.DTO;

namespace IMDB_DB.Builders
{
    internal class RatingBuilder
    {
        private string _fileName;
        private string _outputDir;
        private string _inputDataFile;

        public RatingBuilder( string outputDir, string inputDataFile, string fileName )
        {
            _outputDir = outputDir;
            _inputDataFile = inputDataFile;
            _fileName = fileName;
        }

        public void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new RatingDto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                valueBatch[sqlLineCount] = new RatingDto( line, '\t' );
                sqlLineCount++;

                if( sqlLineCount >= batchSize ) {
                    WriteBatchFile( valueBatch, currentBatchCount );
                    currentBatchCount++;
                    sqlLineCount = 0;
                    valueBatch = new RatingDto[batchSize];
                }

                readerLine++;
            }

            if( valueBatch.Count() > 0 ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( RatingDto[] values, int currentBatchCount )
        {
            string insertHeader = $"INSERT INTO {RatingFileSchema.SqlTableName} ({RatingFileSchema.ImdbIdColName}, {RatingFileSchema.RatingColName}, {RatingFileSchema.NumVotesColName}) VALUES";
            List<string> valueRows = values.Where( v => v != null ).Select( dto => $"('{dto.ImdbId}','{dto.Rating}','{dto.NumVotes}')," ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, insertHeader, valueRows, currentBatchCount );
        }
    }

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
