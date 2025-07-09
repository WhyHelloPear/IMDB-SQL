using MySqlConnector;
using System.Text;

namespace IMDB_DB
{
    public class InsertFactory
    {
        private string _connectionString;
        private string _baseOutputDir;

        public InsertFactory( string connectionString, string baseOutputDir )
        {
            _connectionString = connectionString;
            _baseOutputDir = baseOutputDir;
        }

        public async Task<List<string>> InsertAll()
        {
            List<string> titleErrors = await InsertTitlesSql();
            List<string> personPositionErrors = await InsertPersonsSql();
            List<string> performanceErrors = await InsertPerformancesSql();
            List<string> personErrors = await InsertPersonPositionSql();
            List<string> aliasErrors = await InsertAliasSql();
            List<string> ratingErrors = await InsertRatingsSql();
            List<string> episodeErrors = await InsertEpisodesSql();

            return [ 
                .. titleErrors,
                .. personPositionErrors,
                .. performanceErrors,
                .. personErrors,
                .. aliasErrors,
                .. ratingErrors,
                .. episodeErrors,
            ];
        }

        public async Task<List<string>> InsertAliasSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\titleAlias", connection );
        }

        public async Task<List<string>> InsertPersonPositionSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\personPosition", connection );
        }

        public async Task<List<string>> InsertPerformancesSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\performances", connection );
        }

        public async Task<List<string>> InsertTitlesSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\titles", connection );
        }

        public async Task<List<string>> InsertRatingsSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\ratings", connection );
        }

        public async Task<List<string>> InsertPersonsSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\persons", connection );
        }
        
        public async Task<List<string>> InsertEpisodesSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\episodes", connection );
        }
        
        public async Task<List<string>> InsertGenreLinksSql()
        {
            await using var connection = new MySqlConnection( _connectionString );
            await connection.OpenAsync();

            return await InsertScriptsFromDir( @$"{_baseOutputDir}\genreLinks", connection );
        }

        static async Task<List<string>> InsertScriptsFromDir( string outputDir, MySqlConnection connection )
        {
            string errorDir = $@"{outputDir}\error";
            string completedDir = $@"{outputDir}\inserted";

            Directory.CreateDirectory( errorDir );
            Directory.CreateDirectory( completedDir );

            int fileCount = 0;
            List<string> insertErrors = new List<string>();
            List<string> targetFiles = Directory.EnumerateFiles( outputDir, "*.sql", SearchOption.TopDirectoryOnly ).ToList();
            foreach( var filePath in targetFiles ) {
                var file = Path.GetFileName( filePath );
                string sql = await File.ReadAllTextAsync( filePath );
                try {
                    await using var cmd = new MySqlCommand( sql, connection );
                    await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine( $"Executed: {Path.GetFileName( filePath )}" );
                    fileCount++;
                    File.Move( filePath, $@"{completedDir}\{file}" );
                }
                catch( Exception ex ) {
                    string message = $"Error in {filePath}: {ex.Message}";
                    insertErrors.Add( message );
                    Console.WriteLine( message );
                    File.Move( filePath, $@"{errorDir}\{file}" );
                }
            }

            return insertErrors;
        }
    }
}
