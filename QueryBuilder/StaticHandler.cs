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

            writer.WriteLine( headerRow );
            foreach( var lineValues in valueRows ) {
                writer.WriteLine( lineValues );
            }
        }
    }
}
