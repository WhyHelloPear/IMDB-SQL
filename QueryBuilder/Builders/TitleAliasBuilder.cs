using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class TitleAliasBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public TitleAliasBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.Ordering,
                SqlSchemaInfo.ColumnNames.TitleAlias,
                SqlSchemaInfo.ColumnNames.AliasRegion,
                SqlSchemaInfo.ColumnNames.AliasLanguage,
                SqlSchemaInfo.ColumnNames.AliasType,
                SqlSchemaInfo.ColumnNames.AliasAttributes,
                SqlSchemaInfo.ColumnNames.IsOriginalTitle,
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
                    dto.Ordering.ToString(),
                    dto.TitleAlias,
                    dto.AliasRegion,
                    dto.AliasLanguage,
                    dto.AliasType,
                    dto.AliasAttributes,
                    dto.IsOriginalTitle ? "1" : "0"
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.akas.tsv";
            public enum Indices
            {
                ImdbId = 0,
                Ordering,
                TitleAlias,
                AliasRegion,
                AliasLanguage,
                AliasType,
                AliasAttributes,
                IsOriginalTitle,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "TitleAlias";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string Ordering = "Ordering";
                public const string TitleAlias = "TitleAlias";
                public const string AliasRegion = "AliasRegion";
                public const string AliasLanguage = "AliasLanguage";
                public const string AliasType = "AliasType";
                public const string AliasAttributes = "AliasAttributes";
                public const string IsOriginalTitle = "IsOriginalTitle";
            }
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId].ParseImdbId( ImdbIdPrefix.Title );
                TitleAlias = t[(int)FileSchema.Indices.TitleAlias];
                AliasRegion = t[(int)FileSchema.Indices.AliasRegion];
                AliasLanguage = t[(int)FileSchema.Indices.AliasLanguage];
                AliasType = t[(int)FileSchema.Indices.AliasType];
                AliasAttributes = t[(int)FileSchema.Indices.AliasAttributes];

                IsOriginalTitle = t[(int)FileSchema.Indices.IsOriginalTitle] == "1";

                if( int.TryParse( t[(int)FileSchema.Indices.Ordering], out int order ) ) {
                    Ordering = order;
                }
            }

            public long ImdbId { get; set; }
            public int Ordering { get; set; }
            public string TitleAlias { get; set; }
            public string AliasRegion { get; set; }
            public string AliasLanguage { get; set; }
            public string AliasType { get; set; }
            public string AliasAttributes { get; set; }
            public bool IsOriginalTitle { get; set; }
        }
    }
}