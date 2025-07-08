using System.Linq;
using System.Text;
using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class RatingBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public RatingBuilder( string outputDir, string inputDataFile, string fileName ) : base (outputDir, inputDataFile, fileName) {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.Rating,
                SqlSchemaInfo.ColumnNames.NumVotes,
            };

            _insertHeader = StaticHandler.CreateInsertHeaderRow( SqlSchemaInfo.Table, columnNames );
        }

        public override void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( TargetDataFile );

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
               List<string> values = new List<string> {
                    dto.ImdbId.ToString(),
                    dto.Rating.ToString(),
                    dto.NumVotes.ToString(),
                };

               return StaticHandler.CreateInsertRowFromValues( values );

           } ).ToList();
            
            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.ratings.tsv";
            public enum Indices
            {
                ImdbId = 0,
                Rating,
                NumVotes,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "TitleRating";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string Rating = "Rating";
                public const string NumVotes = "NumVotes";
            }
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId].ParseImdbId( ImdbIdPrefix.Title );
                if( decimal.TryParse( t[(int)FileSchema.Indices.Rating], out decimal rating ) ) {
                    Rating = rating;
                }

                if( int.TryParse( t[(int)FileSchema.Indices.NumVotes], out int numVotes ) ) {
                    NumVotes = numVotes;
                }
            }

            public long ImdbId { get; set; }
            public decimal Rating { get; set; } = 0;
            public int NumVotes { get; set; } = 0;
        }
    }
}
