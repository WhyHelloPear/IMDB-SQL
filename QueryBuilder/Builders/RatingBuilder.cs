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

            Console.WriteLine( "Done." );
        }

        private void WriteBatchFile( RatingDto[] values, int currentBatchCount )
        {
            string insertHeader = $"INSERT INTO {RatingFileSchema.SqlTableName} ({RatingFileSchema.ImdbIdColName}, {RatingFileSchema.RatingColName}, {RatingFileSchema.NumVotesColName}) VALUES";
            List<string> valueRows = values.Where( v => v != null ).Select( dto => $"('{dto.ImdbId}','{dto.Rating}','{dto.NumVotes}')," ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, insertHeader, valueRows, currentBatchCount );
        }
    }
}
