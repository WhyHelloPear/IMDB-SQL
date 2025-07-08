using static IMDB_DB.Constants;

namespace IMDB_DB
{
    internal class UniquePerformance
    {
        private string _baseInputDir;
        private string _baseOutputDir;

        private string ProfessionOutputPath => @$"{_baseOutputDir}\uniqueProfessions.csv";
        private string CategoryOutputPath => @$"{_baseOutputDir}\uniqueCategories.csv";
        private string JobOutputPath => @$"{_baseOutputDir}\uniqueJobs.csv";

        private List<string> UniqueProfessions { get; set; } = new List<string>();
        private List<string> UniqueJobs { get; set; } = new List<string>();
        private List<string> UniqueCategory { get; set; } = new List<string>();

        public UniquePerformance( string baseInputDir, string baseOutputDir )
        {
            _baseInputDir = baseInputDir;
            _baseOutputDir = baseOutputDir;

            Directory.CreateDirectory( _baseOutputDir );
        }

        public void DoStuff()
        {
            if( File.Exists( ProfessionOutputPath ) ) {
                File.Delete( ProfessionOutputPath );
            }
            if( File.Exists( CategoryOutputPath ) ) {
                File.Delete( CategoryOutputPath );
            }
            if( File.Exists( JobOutputPath ) ) {
                File.Delete( JobOutputPath );
            }

            GetUniqueValuesFromPeopleFile();
            GetUniqueValuesFromPrincipalsFile();
        }

        private void GetUniqueValuesFromPeopleFile()
        {
            int professionIndex = 4;
            string file = $@"{_baseInputDir}\name.basics.tsv";
            using var reader = new StreamReader( file );
            Console.WriteLine( $"Checking file, {file}, for unique professions." );

            string? line;
            int readerLine = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                string cleanedValue = line.Split( DataParsing.DELIMITER )[professionIndex].CleanSqlValue();
                if(cleanedValue == "NULL" ) {
                    continue;
                }
                List<string> primaryProfessions = cleanedValue.Split( ',' ).ToList();

                foreach( var profession in primaryProfessions ) {
                    Console.WriteLine( $"Checking value, {profession}..." );
                    if( !UniqueProfessions.Contains( profession ) ) {
                        Console.WriteLine( "Added!" );

                        UniqueProfessions.Add( profession );

                        using var writer = new StreamWriter( ProfessionOutputPath, append: true );
                        writer.WriteLine( profession );
                    }
                }

                readerLine++;
            }
        }

        private void GetUniqueValuesFromPrincipalsFile()
        {
            int jobCategoryIndex = 3;
            int specificJobIndex = 4;
            string file = $@"{_baseInputDir}\title.principals.tsv";
            using var reader = new StreamReader( file );
            Console.WriteLine( $"Checking file, {file}, for unique professions." );

            string? line;
            int readerLine = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                string category = line.Split( DataParsing.DELIMITER )[jobCategoryIndex].CleanSqlValue();
                string job = line.Split( DataParsing.DELIMITER )[specificJobIndex].CleanSqlValue();

                if( category != "NULL" ) {
                    Console.WriteLine( $"Checking category, {category}..." );
                    if( !UniqueCategory.Contains( category ) ) {
                        Console.WriteLine( "\tAdded!" );
                        UniqueCategory.Add(category );
                        using var writer = new StreamWriter( CategoryOutputPath, append: true );
                        writer.WriteLine( category );
                    }
                }
                if( job != "NULL" ) {
                    Console.WriteLine( $"Checking job, {job}..." );
                    if( !UniqueJobs.Contains( job ) ) {
                        Console.WriteLine( "\tAdded!" );
                        UniqueJobs.Add( job );
                        using var writer = new StreamWriter( JobOutputPath, append: true );
                        writer.WriteLine( job );
                    }
                }

                readerLine++;
            }
        }
    }
}
