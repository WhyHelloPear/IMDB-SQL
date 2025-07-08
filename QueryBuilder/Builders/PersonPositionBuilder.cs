using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class PersonPositionBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public PersonPositionBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.PersonImdbId,
                SqlSchemaInfo.ColumnNames.PositionId,
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

                string[] t = line.Split( DataParsing.DELIMITER );

                string imdbId = t[(int)FileSchema.Indices.ImdbId];
                string directors = t[(int)FileSchema.Indices.Directors].Replace( @"\N", "" );
                string writers = t[(int)FileSchema.Indices.Writers].Replace( @"\N", "" );
                if( string.IsNullOrEmpty( directors ) && string.IsNullOrEmpty( writers ) ) {
                    continue;
                }

                List<Dto> titleWritersAndDirectors = [
                    .. writers.Split(',').Where(w => !string.IsNullOrWhiteSpace(w)).Select(w => new Dto(imdbId, w, (int)Constants.TitlePositions.Writer)),
                    .. directors.Split(',').Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => new Dto(imdbId, d, (int)Constants.TitlePositions.Director))
                ];

                foreach( var dto in titleWritersAndDirectors ) {
                    valueBatch[sqlLineCount] = dto;
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

            if( valueBatch.Any() ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( Dto[] values, int currentBatchCount )
        {
            List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                List<string> values = new List<string> {
                    dto.ImdbId.ToString(),
                    dto.PersonImdbId.ToString(),
                    dto.PositionId.ToString(),
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.crew.tsv";
            public enum Indices
            {
                ImdbId = 0,
                Directors,
                Writers
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "TitlePersonPosition";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string PersonImdbId = "PersonImdbId";
                public const string PositionId = "PositionId";
            }
        }

        private class Dto
        {
            public Dto( string imdbId, string personImdbId, int positionId )
            {
                ImdbId = imdbId.ParseImdbId( ImdbIdPrefix.Title );
                PersonImdbId = personImdbId.ParseImdbId( ImdbIdPrefix.Person );
                PositionId = positionId;
            }

            public long ImdbId { get; set; }
            public long PersonImdbId { get; set; }
            public int PositionId { get; set; }
        }
    }
}
