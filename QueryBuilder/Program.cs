using IMDB_DB.Builders;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Text;

//string baseDataDir = @"D:\IMDB_SQL\data";
//string outputDir = @"D:\IMDB_SQL\sql_output";

string baseDataDir = @"E:\MovieLibrary\imdb_sql\data";
string outputDir = @"E:\MovieLibrary\imdb_sql\sql_output";

// Load secrets.json from the current directory
var config = new ConfigurationBuilder()
    .SetBasePath( Directory.GetCurrentDirectory() )
    .AddJsonFile( "secrets.json", optional: false, reloadOnChange: true )
    .Build();

string IMDB_ConnectionString = config.GetSection( "IMDB_ConnectionString" ).Value;

bool isExitInput = false;
while( !isExitInput ) {
    var userMenuBuilder = new StringBuilder();
    userMenuBuilder.AppendLine( "Choose an option:" );
    userMenuBuilder.AppendLine( "1. Create INSERT SQL scripts" );
    userMenuBuilder.AppendLine( "2. Insert SQL scripts in order" );
    userMenuBuilder.AppendLine( "0. Exit" );
    userMenuBuilder.AppendLine( "=============================" );
    Console.WriteLine( userMenuBuilder.ToString() );

    var input = Console.ReadLine();
    switch( input ) {
        case "0":
            isExitInput = true;
            Console.WriteLine( "Exiting...Goodbye!" );
            break;
        case "1":
            PrepareScripts();
            break;
        case "2":
            await InsertAllScripts();
            break;
        default:
            Console.WriteLine( "Invalid option.\n\n" );
            break;
    }
}


void PrepareScripts()
{
    var ratingBuilder = new RatingBuilder( @$"{outputDir}\ratings", @$"{baseDataDir}\{RatingFileSchema.FileName}", "rating_data_insert" );
    ratingBuilder.CreateRatingInsertFiles();

    var titleBuilder = new TitleBuilder( @$"{outputDir}\titles", @$"{baseDataDir}\{TitleFileSchema.FileName}", "titles_data_insert" );
    titleBuilder.CreateRatingInsertFiles();

    var personBuilder = new PersonBuilder( @$"{outputDir}\persons", @$"{baseDataDir}\{PersonFileSchema.FileName}", "persons_data_insert" );
    personBuilder.CreateRatingInsertFiles();
}

async Task InsertAllScripts()
{
    await using var connection = new MySqlConnection( IMDB_ConnectionString );
    await connection.OpenAsync();

    List<string> titleErrors = await InsertScriptsFromDir( @$"{outputDir}\titles", connection );
    List<string> ratingErrors = await InsertScriptsFromDir( @$"{outputDir}\ratings", connection );
    List<string> personErrors = await InsertScriptsFromDir( @$"{outputDir}\persons", connection );

    var errorBuilder = new StringBuilder();
    errorBuilder.AppendLine( "Errors from inserts:" );

    errorBuilder.AppendLine( "Titles:" );
    foreach( var titleError in titleErrors ) {
        errorBuilder.AppendLine( titleError );
    }

    errorBuilder.AppendLine( "\nRatings:" );
    foreach( var ratingError in ratingErrors ) {
        errorBuilder.AppendLine( ratingError );
    }

    errorBuilder.AppendLine( "\nRatings:" );
    foreach( var personError in personErrors ) {
        errorBuilder.AppendLine( personError );
    }

    Console.WriteLine(errorBuilder.ToString() );
}

static async Task<List<string>> InsertScriptsFromDir( string outputDir, MySqlConnection connection )
{
    int fileCount = 0;
    List<string> insertErrors = new List<string>();
    foreach( var filePath in Directory.EnumerateFiles( outputDir, "*.sql", SearchOption.AllDirectories ) ) {
        string sql = await File.ReadAllTextAsync( filePath );
        try {
            await using var cmd = new MySqlCommand( sql, connection );
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine( $"Executed: {Path.GetFileName( filePath )}" );
            fileCount++;
        }
        catch( Exception ex ) {
            string message = $"Error in {filePath}: {ex.Message}";
            insertErrors.Add( message );
            Console.WriteLine( message );
        }
    }

    return insertErrors;
}