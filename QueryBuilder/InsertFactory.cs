using MySqlConnector;
using System.Data;
using System.Text;

namespace IMDB_DB
{
    public class InsertFactory
    {
        private MySqlConnection _connection;

        public InsertFactory( string connectionString )
        {
            _connection = new MySqlConnection( connectionString );
        }

        public async Task InsertScriptsFromDir( string outputDir )
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
                    if( _connection.State != ConnectionState.Open ) {
                        await _connection.OpenAsync();
                    }

                    await using var cmd = new MySqlCommand( sql, _connection );
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

            WriteErrorsToConsole( insertErrors );
        }

        private void WriteErrorsToConsole( List<string> errors )
        {
            var errorBuilder = new StringBuilder();
            errorBuilder.AppendLine( "Errors from inserts:" );

            foreach( var error in errors ) {
                errorBuilder.AppendLine( error );
            }

            Console.WriteLine( errorBuilder.ToString() );
        }
    }
}
