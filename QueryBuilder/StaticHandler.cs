using System.Text;

namespace IMDB_DB
{
    internal static class StaticHandler
    {
        public static void WriteBatchFile( string outputDir, string baseFileName, string headerRow, List<string> valueRows, int currentBatchCount )
        {

            Directory.CreateDirectory( outputDir );

            string fileName = $"{baseFileName}{( currentBatchCount == 0 ? string.Empty : $"_{currentBatchCount}" )}.sql";
            string outputPath = @$"{outputDir}\{fileName}";
            Console.WriteLine( $"Creating file, {fileName}..." );

            using var writer = new StreamWriter( outputPath, append: false );

            //cleanup last line before writing
            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            writer.WriteLine( headerRow );
            foreach( var lineValues in valueRows ) {
                writer.WriteLine( lineValues );
            }
        }

        public static string Truncate( this string value, int maxLength )
        {
            if( string.IsNullOrEmpty( value ) ) {
                return value;
            }

            return value.Length <= maxLength ? value : value.Substring( 0, maxLength );
        }

        public static long ParseImdbId( this string imdbId, string expectedPrefix )
        {
            if( string.IsNullOrWhiteSpace( imdbId ) )
                throw new ArgumentException( "IMDB ID cannot be null or empty." );

            if( !imdbId.StartsWith( expectedPrefix ) )
                throw new ArgumentException( $"IMDB ID must start with '{expectedPrefix}'." );

            string numericPart = imdbId.Substring( expectedPrefix.Length );

            if( !long.TryParse( numericPart, out long id ) )
                throw new FormatException( "Invalid numeric part in IMDB ID." );

            return id;
        }

        public static string CreateInsertRowFromValues( List<string> values )
        {
            var rowBuilder = new StringBuilder();
            rowBuilder.Append( "\t(" );
            foreach( var value in values ) {
                string cleanedValue = $"'{value.Truncate( 255 ).Replace( @"\N", "NULL" ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'";
                cleanedValue = cleanedValue.Replace( @"'NULL'", "NULL" );
                rowBuilder.Append( $" {cleanedValue}," );
            }
            var row = rowBuilder.ToString().TrimEnd( ',' );
            return $"{row} ),";
        }

        public static string CreateInsertHeaderRow( string tableName, List<string> values )
        {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.Append( $"INSERT INTO {tableName} (" );
            foreach( var value in values ) {
                headerBuilder.Append( $" {value}," );
            }
            var row = headerBuilder.ToString().TrimEnd( ',' );
            return $"{row} )\nVALUES";
        }
    }
}
