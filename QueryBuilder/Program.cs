using IMDB_DB.Builders;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Text;

//string baseDataDir = @"D:\IMDB_SQL\data";
//string outputDir = @"D:\IMDB_SQL\sql_output";

string baseDataDir = @"E:\MovieLibrary\imdb_sql\data";
string outputDir = @"E:\MovieLibrary\imdb_sql\sql_output";

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
    //var ratingBuilder = new RatingBuilder( @$"{outputDir}\ratings", baseDataDir, "rating_data_insert" );
    //ratingBuilder.CreateRatingInsertFiles();

    //var titleBuilder = new TitleBuilder( @$"{outputDir}\titles", baseDataDir, "titles_data_insert" );
    //titleBuilder.CreateRatingInsertFiles();

    //var personBuilder = new PersonBuilder( @$"{outputDir}\persons", baseDataDir, "persons_data_insert" );
    //personBuilder.CreateRatingInsertFiles();

    //var performanceBuilder = new PerformanceBuilder( @$"{outputDir}\performances", baseDataDir, "performances_data_insert" );
    //performanceBuilder.CreateRatingInsertFiles();

    //var personPositionBuilder = new PersonPositionBuilder( @$"{outputDir}\personPosition", baseDataDir, "personPosition_data_insert" );
    //personPositionBuilder.CreateRatingInsertFiles();

    var titleAliasBuilder = new TitleAliasBuilder( @$"{outputDir}\titleAlias", baseDataDir, "personPosition_data_insert" );
    titleAliasBuilder.CreateRatingInsertFiles();
}

async Task InsertAllScripts()
{
    await using var connection = new MySqlConnection( IMDB_ConnectionString );
    await connection.OpenAsync();

    //List<string> titleErrors = await InsertScriptsFromDir( @$"{outputDir}\titles", connection );
    //List<string> ratingErrors = await InsertScriptsFromDir( @$"{outputDir}\ratings", connection );
    //List<string> personErrors = await InsertScriptsFromDir( @$"{outputDir}\persons", connection );
    //List<string> performanceErrors = await InsertScriptsFromDir( @$"{outputDir}\performances", connection );
    List<string> personPositionErrors = await InsertScriptsFromDir( @$"{outputDir}\personPosition", connection );

    var errorBuilder = new StringBuilder();
    errorBuilder.AppendLine( "Errors from inserts:" );

    //errorBuilder.AppendLine( "Titles:" );
    //foreach( var error in titleErrors ) {
    //    errorBuilder.AppendLine( error );
    //}

    //errorBuilder.AppendLine( "=======================================" );
    //errorBuilder.AppendLine( "\nRatings:" );
    //foreach( var error in ratingErrors ) {
    //    errorBuilder.AppendLine( error );
    //}

    //errorBuilder.AppendLine( "=======================================" );
    //errorBuilder.AppendLine( "\nPersons:" );
    //foreach( var error in personErrors ) {
    //    errorBuilder.AppendLine( error );
    //}

    //errorBuilder.AppendLine( "=======================================" );
    //errorBuilder.AppendLine( "\nPerformances:" );
    //foreach( var error in performanceErrors ) {
    //    errorBuilder.AppendLine( error );
    //}

    //errorBuilder.AppendLine( "=======================================" );
    errorBuilder.AppendLine( "\nPerson Positions:" );
    foreach( var error in personPositionErrors ) {
        errorBuilder.AppendLine( error );
    }

    Console.WriteLine( errorBuilder.ToString() );
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