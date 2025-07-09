using static IMDB_DB.Constants;

namespace IMDB_DB
{
    internal class UniqueGenres
    {
        private string _baseInputDir;
        private string _baseOutputDir;

        private string GenreOutputPath => @$"{_baseOutputDir}\genres.csv";

        private List<string> UniqueGenreNames { get; set; } = new List<string>();

        public UniqueGenres( string baseInputDir, string baseOutputDir )
        {
            _baseInputDir = baseInputDir;
            _baseOutputDir = baseOutputDir;

            Directory.CreateDirectory( _baseOutputDir );
        }

        public void FindUniqueGenres()
        {
            if( File.Exists( GenreOutputPath ) ) {
                File.Delete( GenreOutputPath );
            }

            int professionIndex = 8;
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

                string cleanedValue = line.Split( DataParsing.DELIMITER )[professionIndex].CleanSqlValue();
                if( cleanedValue == "NULL" ) {
                    continue;
                }
                List<string> genres = cleanedValue.Split( ',' ).ToList();

                foreach( var genre in genres ) {
                    Console.WriteLine( $"Checking value, {genre}..." );
                    if( !UniqueGenreNames.Contains( genre ) ) {
                        Console.WriteLine( "\tAdded!" );

                        UniqueGenreNames.Add( genre );

                        using var writer = new StreamWriter( GenreOutputPath, append: true );
                        writer.WriteLine( genre );
                    }
                }

                readerLine++;
            }
        }
    }
}
