using static IMDB_DB.Constants;
using static IMDB_DB.FileColumnIndices;

namespace IMDB_DB.Builders
{
    internal class KnownTitlesBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public KnownTitlesBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.PersonImdbId,
                SqlSchemaInfo.ColumnNames.ImdbId,
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

                var originalLineDto = new Dto( line );
                List<Dto> splitDtos = originalLineDto.SplitDtoByGenre();
                if( !splitDtos.Any() ) {
                    continue;
                }

                foreach( var splitDto in splitDtos ) {
                    valueBatch[sqlLineCount] = splitDto;
                    sqlLineCount++;

                    if( sqlLineCount >= batchSize ) {
                        WriteBatchFile( valueBatch, currentBatchCount );
                        currentBatchCount++;
                        sqlLineCount = 0;
                        valueBatch = new Dto[batchSize];
                    }
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
                    dto.PersonId.ToString(),
                    dto.ImdbId.ToString(),
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "name.basics.tsv";
            public enum Indices
            {
                PersonId = PersonIndices.PersonId,
                KnownForTitles = PersonIndices.KnownForTitles,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "PersonKnownForTitles";
            public static class ColumnNames
            {
                public const string PersonImdbId = "PersonImdbId";
                public const string ImdbId = "ImdbId";
            }
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                PersonId = t[(int)FileSchema.Indices.PersonId].ParseImdbId( ImdbIdPrefix.Person );
                string knownForTitles = t[(int)FileSchema.Indices.KnownForTitles].CleanSqlValue();

                KnowForTitleIds = ( knownForTitles == "NULL" || string.IsNullOrEmpty( knownForTitles ) ) ? new List<string>() : knownForTitles.Split( ',' ).ToList();
            }

            public Dto( long personId, long imdbId )
            {
                PersonId = personId;
                ImdbId = imdbId;
            }

            public long PersonId { get; set; }
            public long ImdbId { get; set; }
            public List<string> KnowForTitleIds { get; set; }

            public List<Dto> SplitDtoByGenre()
            {
                var result = new List<Dto>();
                foreach( var imdbId in KnowForTitleIds ) {
                    result.Add( new Dto( PersonId, imdbId.ParseImdbId( ImdbIdPrefix.Title ) ) );
                }
                return result;
            }
        }
    }
}