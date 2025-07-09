using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class PerformanceBuilder : BaseBuilder
    {
        private Dictionary<string, int> PositionMapper { get; } = new Dictionary<string, int>() {
            { "director", (int)TitlePositions.Director },
            { "writer", (int)TitlePositions.Writer },
            { "producer", (int)TitlePositions.Producer },
            { "actor", (int)TitlePositions.Actor },
            { "actress", (int)TitlePositions.Actor },
            { "cinematographer", (int)TitlePositions.Cinematographer },
            { "composer", (int)TitlePositions.Composer },
            { "editor", (int)TitlePositions.Editor },
            { "production_designer", (int)TitlePositions.ProductionDesigner },
            { "archive_footage", (int)TitlePositions.FeaturedSubject },
            { "self", (int)TitlePositions.FeaturedSubject },
            { "casting_director", (int)TitlePositions.CastingDirector },
            { "archive_sound", (int)TitlePositions.Voice },
        };

        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public PerformanceBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.PersonImdbId,
                SqlSchemaInfo.ColumnNames.PositionId,
                SqlSchemaInfo.ColumnNames.Ordering,
                SqlSchemaInfo.ColumnNames.PerformanceDescription,
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

                valueBatch[sqlLineCount] = new Dto( line, PositionMapper );
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
                    dto.PersonId.ToString(),
                    dto.PositionId.ToString(),
                    dto.Ordering.ToString(),
                    dto.PerformanceDescription,
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.principals.tsv";
            public enum Indices
            {
                ImdbId = 0,
                Ordering,
                PersonId,
                Category,
                Job,
                Characters
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "Performance";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string PersonImdbId = "PersonImdbId";
                public const string PositionId = "PositionId";
                public const string Ordering = "Ordering";
                public const string PerformanceDescription = "PerformanceDescription";
            }
        }

        private class Dto
        {
            public Dto( string dataLine, Dictionary<string, int> positionMapper )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId].ParseImdbId( ImdbIdPrefix.Title );
                PersonId = t[(int)FileSchema.Indices.PersonId].ParseImdbId( ImdbIdPrefix.Person );

                string job = t[(int)FileSchema.Indices.Job];
                string characters = t[(int)FileSchema.Indices.Characters];

                string category = t[(int)FileSchema.Indices.Category];
                int positionId = positionMapper.GetValueOrDefault( category );
                PositionId = positionId;

                string description = string.Empty;
                switch( positionId ) {
                    case (int)TitlePositions.Editor:
                    case (int)TitlePositions.Composer:
                    case (int)TitlePositions.Cinematographer:
                    case (int)TitlePositions.Writer:
                        description = job;
                        break;
                    case (int)TitlePositions.Voice:
                    case (int)TitlePositions.FeaturedSubject:
                    case (int)TitlePositions.Actor:
                        description = characters.ReadyCharacterInput();
                        break;
                    case (int)TitlePositions.CastingDirector:
                    case (int)TitlePositions.ProductionDesigner:
                    case (int)TitlePositions.Director:
                    case (int)TitlePositions.Producer:
                        description = DataParsing.NULL;
                        break;
                }

                PerformanceDescription = description;

                if( int.TryParse( t[(int)FileSchema.Indices.Ordering], out int order ) ) {
                    Ordering = order;
                }
            }

            public long ImdbId { get; set; }
            public long PersonId { get; set; }
            public int Ordering { get; set; }
            public int PositionId { get; set; }
            public string PerformanceDescription { get; set; }
        }
    }
}