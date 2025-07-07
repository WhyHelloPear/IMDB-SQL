using System.Text;

namespace IMDB_DB.Builders
{
    internal class RatingBuilder : BaseBuilder
    {
        public RatingBuilder( string outputDir, string inputDataFile, string fileName ) : base (outputDir, inputDataFile, fileName) {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.AppendLine( $"INSERT INTO {FileSchema.SqlTableName} ( {FileSchema.ImdbIdColName}, {FileSchema.RatingColName}, {FileSchema.NumVotesColName} )" );
            headerBuilder.Append( $"VALUES" );

            _insertHeader = headerBuilder.ToString();
        }

        public override void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new Dto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                valueBatch[sqlLineCount] = new Dto( line );
                sqlLineCount++;

                if( sqlLineCount >= batchSize ) {
                    WriteBatchFile( valueBatch, currentBatchCount );
                    currentBatchCount++;
                    sqlLineCount = 0;
                    valueBatch = new Dto[batchSize];
                }

                readerLine++;
            }

            if( valueBatch.Count() > 0 ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( Dto[] values, int currentBatchCount )
        {
           List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                var rowBuilder = new StringBuilder();
                rowBuilder.Append( "\t" );
                rowBuilder.Append( $"( '{dto.ImdbId}'" );
                rowBuilder.Append( $", '{dto.Rating}'" );
                rowBuilder.Append( $", '{dto.NumVotes}' )," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public enum Indices
            {
                ImdbId = 0,
                Rating,
                NumVotes,
            }

            public const string FileName = "title.ratings.tsv";

            public const string SqlTableName = "TitleRating";

            public const string ImdbIdColName = "ImdbId";
            public const string RatingColName = "Rating";
            public const string NumVotesColName = "NumVotes";
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( Constants.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId];
                if( decimal.TryParse( t[(int)FileSchema.Indices.Rating], out decimal rating ) ) {
                    Rating = rating;
                }

                if( int.TryParse( t[(int)FileSchema.Indices.NumVotes], out int numVotes ) ) {
                    NumVotes = numVotes;
                }
            }

            public string ImdbId { get; set; }
            public decimal Rating { get; set; } = 0;
            public int NumVotes { get; set; } = 0;
        }
    }

}
