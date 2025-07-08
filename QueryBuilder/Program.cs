using IMDB_DB;
using Microsoft.Extensions.Configuration;
using System.Text;

var config = new ConfigurationBuilder()
    .SetBasePath( Directory.GetCurrentDirectory() )
    .AddJsonFile( "secrets.json", optional: false, reloadOnChange: true )
    .Build();

string IMDB_ConnectionString = config.GetSection( "IMDB_ConnectionString" ).Value;

//string outputDir = @"D:\IMDB_SQL\sql_output";
string outputDir = @"E:\MovieLibrary\imdb_sql\sql_output";

//string baseDataDir = @"D:\IMDB_SQL\data";
string baseDataDir = @"E:\MovieLibrary\imdb_sql\data";

bool isExitInput = false;
while( !isExitInput ) {
    var userMenuBuilder = new StringBuilder();
    userMenuBuilder.AppendLine( "Choose an option:" );
    userMenuBuilder.AppendLine( "1. Create INSERT SQL scripts" );
    userMenuBuilder.AppendLine( "2. Insert SQL scripts" );
    userMenuBuilder.AppendLine( "3. Get Unique Values" );
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
            HandlePrepareScriptsChoice();
            break;
        case "2":
            await HandleInsertScriptsChoice();
            break;
        case "3":
            await HandleUniqueValueChoice();
            break;
        default:
            Console.WriteLine( "Invalid option.\n\n" );
            break;
    }
}

async Task HandleUniqueValueChoice()
{
    var factory = new UniquePerformance( baseDataDir, $@"{outputDir}\info" );

    bool isExitInput = false;
    while( !isExitInput ) {
        var userMenuBuilder = new StringBuilder();
        userMenuBuilder.AppendLine( "\n=============================" );
        userMenuBuilder.AppendLine( "Choose an option:" );
        userMenuBuilder.AppendLine( "1. Professions" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "=============================" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                factory.DoStuff();
                break;
            default:
                Console.WriteLine( "Invalid option.\n\n" );
                break;
        }
    }
}

void HandlePrepareScriptsChoice()
{
    var factory = new BuilderFactory( baseDataDir, outputDir );

    bool isExitInput = false;
    while( !isExitInput ) {
        var userMenuBuilder = new StringBuilder();
        userMenuBuilder.AppendLine( "\n=============================" );
        userMenuBuilder.AppendLine( "Choose an option:" );
        userMenuBuilder.AppendLine( "1. All" );
        userMenuBuilder.AppendLine( "2. Titles" );
        userMenuBuilder.AppendLine( "3. Persons" );
        userMenuBuilder.AppendLine( "4. Performance" );
        userMenuBuilder.AppendLine( "5. PersonPosition (director/writer)" );
        userMenuBuilder.AppendLine( "6. Title Alias" );
        userMenuBuilder.AppendLine( "7. Title Ratings" );
        userMenuBuilder.AppendLine( "8. Episodes" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "=============================" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                factory.BuildAll();
                break;
            case "2": //titles
                factory.BuildTitlesSql();
                break;
            case "3": //persons
                factory.BuildPersonsSql();
                break;
            case "4": //performances
                factory.BuildPerformancesSql();
                break;
            case "5": //person positions
                factory.BuildPersonPositionSql();
                break;
            case "6": //aliases
                factory.BuildAliasSql();
                break;
            case "7": //ratings
                factory.BuildRatingsSql();
                break;
            case "8": //episodes
                factory.BuildEpisodesSql();
                break;
            default:
                Console.WriteLine( "Invalid option.\n\n" );
                break;
        }
    }
}

async Task HandleInsertScriptsChoice()
{
    var factory = new InsertFactory( IMDB_ConnectionString, outputDir );

    bool isExitInput = false;
    while( !isExitInput ) {
        var userMenuBuilder = new StringBuilder();
        userMenuBuilder.AppendLine( "\n=============================" );
        userMenuBuilder.AppendLine( "Choose an option:" );
        userMenuBuilder.AppendLine( "1. All" );
        userMenuBuilder.AppendLine( "2. Titles" );
        userMenuBuilder.AppendLine( "3. Persons" );
        userMenuBuilder.AppendLine( "4. Performance" );
        userMenuBuilder.AppendLine( "5. PersonPosition (director/writer)" );
        userMenuBuilder.AppendLine( "6. Title Alias" );
        userMenuBuilder.AppendLine( "7. Title Ratings" );
        userMenuBuilder.AppendLine( "8. Episodes" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "=============================" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var errors = new List<string>();

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                errors = await factory.InsertAll();
                WriteErrorsToConsole( errors );
                break;
            case "2": //titles
                errors = await factory.InsertTitlesSql();
                WriteErrorsToConsole( errors );
                break;
            case "3": //persons
                errors = await factory.InsertPersonsSql();
                WriteErrorsToConsole( errors );
                break;
            case "4": //performances
                errors = await factory.InsertPerformancesSql();
                WriteErrorsToConsole( errors );
                break;
            case "5": //person positions
                errors = await factory.InsertPersonPositionSql();
                WriteErrorsToConsole( errors );
                break;
            case "6": //aliases
                errors = await factory.InsertAliasSql();
                WriteErrorsToConsole( errors );
                break;
            case "7": //ratings
                errors = await factory.InsertRatingsSql();
                WriteErrorsToConsole( errors );
                break;
            case "8": //episodes
                errors = await factory.InsertEpisodesSql();
                WriteErrorsToConsole( errors );
                break;
            default:
                Console.WriteLine( "Invalid option.\n\n" );
                break;
        }
    }
}

void WriteErrorsToConsole( List<string> errors )
{
    var errorBuilder = new StringBuilder();
    errorBuilder.AppendLine( "Errors from inserts:" );

    foreach( var error in errors ) {
        errorBuilder.AppendLine( error );
    }

    Console.WriteLine( errorBuilder.ToString() );
}
