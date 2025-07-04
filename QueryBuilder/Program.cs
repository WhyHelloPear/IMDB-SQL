using IMDB_DB;

string inputPath = @$"D:\IMDB_SQL\data\{RatingFileSchema.FileName}";

int batchSize = 1000; // Number of rows per INSERT statement

using var reader = new StreamReader( inputPath );

var valueBatch = new RatingDto[batchSize];
string? line;

int readerLine = 0;
int sqlLineCount = 0;
int currentBatchCount = 0;
while( ( line = reader.ReadLine() ) != null ) {
    if( readerLine == 0 ) {
        readerLine++;
        continue;
    }

    valueBatch[sqlLineCount] = new RatingDto( line, '\t' );
    sqlLineCount++;

    if( sqlLineCount >= batchSize ) {
        WriteInsert( valueBatch, currentBatchCount );
        currentBatchCount++;
        sqlLineCount = 0;
        valueBatch = new RatingDto[batchSize];
    }

    readerLine++;
}

if( valueBatch.Count() > 0 ) {
    WriteInsert( valueBatch, currentBatchCount );
}

Console.WriteLine( "Done." );

static void WriteInsert( RatingDto[] values, int currentBatchCount )
{
    Directory.CreateDirectory( @"D:\IMDB_SQL\output" );

    string fileName = $"rating_sql{( currentBatchCount == 0 ? string.Empty : $"_{currentBatchCount}" )}.sql";
    string outputPath = @$"D:\IMDB_SQL\output\{fileName}";
    Console.WriteLine( $"Creating file, {fileName}..." );

    using var writer = new StreamWriter( outputPath, append: false ); // Overwrites existing file

    string insertHeader = $"INSERT INTO {RatingFileSchema.SqlTableName} ({RatingFileSchema.ImdbIdColName}, {RatingFileSchema.RatingColName}, {RatingFileSchema.NumVotesColName}) VALUES";

    int currentIndex = 0;
    int lastIndex = values.Where( v => v != null ).Count() - 1;
    writer.WriteLine( insertHeader );
    foreach( var lineValues in values ) {
        if( lineValues == null ) {
            break;
        }
        string dataInsert = $"('{lineValues.ImdbId}','{lineValues.Rating}','{lineValues.NumVotes}'){( currentIndex != lastIndex ? "," : string.Empty )}";
        writer.WriteLine( dataInsert );
        currentIndex++;
    }
}
