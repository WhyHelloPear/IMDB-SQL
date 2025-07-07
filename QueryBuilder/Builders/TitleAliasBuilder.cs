using System.Text;

namespace IMDB_DB.Builders
{
    internal class TitleAliasBuilder : BaseBuilder
    {
        public TitleAliasBuilder( string outputDir, string inputDataFile, string fileName ) : base (outputDir, inputDataFile, fileName) {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.Append( $"INSERT INTO {SqlSchemaInfo.Table} (" );
            headerBuilder.Append( $"{SqlSchemaInfo.ColumnNames.ImdbId}, {SqlSchemaInfo.ColumnNames.Ordering}, {SqlSchemaInfo.ColumnNames.TitleAlias}" );
            headerBuilder.Append( $"{SqlSchemaInfo.ColumnNames.AliasRegion}, {SqlSchemaInfo.ColumnNames.AliasLanguage}, {SqlSchemaInfo.ColumnNames.AliasType}" );
            headerBuilder.Append( $", {SqlSchemaInfo.ColumnNames.AliasAttributes}, {SqlSchemaInfo.ColumnNames.IsOriginalTitle}" );
            headerBuilder.AppendLine( " )" );
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
                rowBuilder.Append( "\t( " );
                rowBuilder.Append( $" '{dto.ImdbId}'" );
                rowBuilder.Append( $" '{dto.Ordering}'" );
                rowBuilder.Append( $", '{dto.TitleAlias.Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{dto.AliasRegion.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.AliasLanguage.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.AliasType.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.AliasAttributes.Truncate( 255 ).Replace( @"\N", "NULL" ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $" '{dto.IsOriginalTitle}'" );
                rowBuilder.Append( ")," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "name.basics.tsv";
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
            public const string Table = "Person";
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
                string[] t = dataLine.Split( Constants.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId];
                TitleAlias = t[(int)FileSchema.Indices.TitleAlias];
                AliasRegion = t[(int)FileSchema.Indices.AliasRegion];
                AliasLanguage = t[(int)FileSchema.Indices.AliasLanguage];
                AliasType = t[(int)FileSchema.Indices.AliasType];
                AliasAttributes = t[(int)FileSchema.Indices.AliasAttributes];

                if( int.TryParse( t[(int)FileSchema.Indices.AliasAttributes], out int order ) ) {
                    Ordering = order;
                }

                if( bool.TryParse( t[(int)FileSchema.Indices.IsOriginalTitle], out bool isOriginal ) ) {
                    IsOriginalTitle = isOriginal;
                }
            }

            public string ImdbId { get; set; }
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