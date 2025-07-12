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
    userMenuBuilder.AppendLine( "=============================" );
    userMenuBuilder.AppendLine( "==========Main Menu==========" );
    userMenuBuilder.AppendLine( "=============================" );
    userMenuBuilder.AppendLine( "Choose an option:\n" );
    userMenuBuilder.AppendLine( "1. Create INSERT SQL scripts" );
    userMenuBuilder.AppendLine( "2. Insert SQL scripts" );
    userMenuBuilder.AppendLine( "3. Get Unique Values" );
    userMenuBuilder.AppendLine( "0. Exit" );
    userMenuBuilder.AppendLine( "\n=============================\n" );
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
    string baseOutputDir = $@"{outputDir}\info";
    bool isExitInput = false;
    while( !isExitInput ) {
        var userMenuBuilder = new StringBuilder();
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "======Unique Value Menu======" );
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "Choose an option:\n" );
        userMenuBuilder.AppendLine( "1. Professions" );
        userMenuBuilder.AppendLine( "2. Genres" );
        userMenuBuilder.AppendLine( "3. Title Types" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "\n=============================\n" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                var performance = new UniquePerformance( baseDataDir, baseOutputDir );
                performance.FindUniqueProfessions();
                break;
            case "2":
                var genres = new UniqueGenres( baseDataDir, baseOutputDir );
                genres.FindUniqueGenres();
                break;
            case "3":
                var titleTypes = new UniqueTitleType( baseDataDir, baseOutputDir );
                titleTypes.FindUniqueTitleTypes();
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
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "======Prepare SQL Menu=======" );
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "Choose an option:\n" );
        userMenuBuilder.AppendLine( "1. Titles" );
        userMenuBuilder.AppendLine( "2. Persons" );
        userMenuBuilder.AppendLine( "3. Performance" );
        userMenuBuilder.AppendLine( "4. PersonPosition (director/writer)" );
        userMenuBuilder.AppendLine( "5. Title Alias" );
        userMenuBuilder.AppendLine( "6. Title Ratings" );
        userMenuBuilder.AppendLine( "7. Episodes" );
        userMenuBuilder.AppendLine( "8. Genre Links" );
        userMenuBuilder.AppendLine( "9. Known for Titles" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "\n=============================\n" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                factory.BuildTitlesSql();
                break;
            case "2":
                factory.BuildPersonsSql();
                break;
            case "3":
                factory.BuildPerformancesSql();
                break;
            case "4":
                factory.BuildPersonPositionSql();
                break;
            case "5":
                factory.BuildAliasSql();
                break;
            case "6":
                factory.BuildRatingsSql();
                break;
            case "7":
                factory.BuildEpisodesSql();
                break;
            case "8":
                factory.BuildGenreLinksSql();
                break;
            case "9":
                factory.BuildKnownForTitlesSql();
                break;
            default:
                Console.WriteLine( "Invalid option.\n\n" );
                break;
        }
    }
}

async Task HandleInsertScriptsChoice()
{
    var factory = new InsertFactory( IMDB_ConnectionString );

    bool isExitInput = false;
    while( !isExitInput ) {
        var userMenuBuilder = new StringBuilder();
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "=======Insert SQL Menu=======" );
        userMenuBuilder.AppendLine( "=============================" );
        userMenuBuilder.AppendLine( "Choose an option:\n" );
        userMenuBuilder.AppendLine( "1. Titles" );
        userMenuBuilder.AppendLine( "2. Persons" );
        userMenuBuilder.AppendLine( "3. Performance" );
        userMenuBuilder.AppendLine( "4. PersonPosition (director/writer)" );
        userMenuBuilder.AppendLine( "5. Title Alias" );
        userMenuBuilder.AppendLine( "6. Title Ratings" );
        userMenuBuilder.AppendLine( "7. Episodes" );
        userMenuBuilder.AppendLine( "8. Genre Links" );
        userMenuBuilder.AppendLine( "9. Known for Titles" );
        userMenuBuilder.AppendLine( "0. Back" );
        userMenuBuilder.AppendLine( "\n=============================\n" );
        Console.WriteLine( userMenuBuilder.ToString() );

        var errors = new List<string>();

        var input = Console.ReadLine();
        switch( input ) {
            case "0":
                isExitInput = true;
                break;
            case "1":
                await factory.InsertScriptsFromDir( @$"{outputDir}\titles" );
                break;
            case "2":
                await factory.InsertScriptsFromDir( @$"{outputDir}\persons" );
                break;
            case "3":
                await factory.InsertScriptsFromDir( @$"{outputDir}\performances" );
                break;
            case "4":
                await factory.InsertScriptsFromDir( @$"{outputDir}\personPosition" );
                break;
            case "5":
                await factory.InsertScriptsFromDir( @$"{outputDir}\titleAlias" );
                break;
            case "6":
                await factory.InsertScriptsFromDir( @$"{outputDir}\ratings" );
                break;
            case "7":
                await factory.InsertScriptsFromDir( @$"{outputDir}\episodes" );
                break;
            case "8":
                await factory.InsertScriptsFromDir( @$"{outputDir}\genreLinks" );
                break;
            case "9":
                await factory.InsertScriptsFromDir( @$"{outputDir}\knownForTitles" );
                break;
            default:
                Console.WriteLine( "Invalid option.\n\n" );
                break;
        }
    }
}
