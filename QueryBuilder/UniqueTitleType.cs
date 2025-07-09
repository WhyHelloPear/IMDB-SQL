using static IMDB_DB.Constants;
using static IMDB_DB.FileColumnIndices;

namespace IMDB_DB
{
    internal class UniqueTitleType
    {
        private string _baseInputDir;
        private string _baseOutputDir;

        private string GenreOutputPath => @$"{_baseOutputDir}\titleTypes.csv";

        private List<string> UniqueTypes { get; set; } = new List<string>();

        public UniqueTitleType( string baseInputDir, string baseOutputDir )
        {
            _baseInputDir = baseInputDir;
            _baseOutputDir = baseOutputDir;

            Directory.CreateDirectory( _baseOutputDir );
        }

        public void FindUniqueTitleTypes()
        {
            if( File.Exists( GenreOutputPath ) ) {
                File.Delete( GenreOutputPath );
            }

            string file = $@"{_baseInputDir}\title.basics.tsv";
            using var reader = new StreamReader( file );
            Console.WriteLine( $"Checking file, {file}, for unique genres." );

            string? line;
            int readerLine = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                string titleType = line.Split( DataParsing.DELIMITER )[(int)TitleBasicsIndices.Type].CleanSqlValue();
                if( titleType == "NULL" ) {
                    continue;
                }

                Console.WriteLine( $"Checking value, {titleType}..." );
                if( !UniqueTypes.Contains( titleType ) ) {
                    Console.WriteLine( "\tAdded!" );

                    UniqueTypes.Add( titleType );

                    using var writer = new StreamWriter( GenreOutputPath, append: true );
                    writer.WriteLine( titleType );
                }

                readerLine++;
            }
        }
    }
}
