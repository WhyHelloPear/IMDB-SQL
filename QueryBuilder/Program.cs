using IMDB_DB;
using IMDB_DB.Builders;


string baseDataDir = @"D:\IMDB_SQL\data";
string outputDir = @"D:\IMDB_SQL\sql_output";

//string baseDataDir = @"E:\MovieLibrary\imdb_sql\data";
//string outputDir = @"E:\MovieLibrary\imdb_sql\sql_output\ratings";

var ratingBuilder = new RatingBuilder( @$"{outputDir}\ratings", @$"{baseDataDir}\{RatingFileSchema.FileName}", "rating_data_insert" );
ratingBuilder.CreateRatingInsertFiles();

var titleBuilder = new TitleBuilder( @$"{outputDir}\titles", @$"{baseDataDir}\{TitleFileSchema.FileName}", "titles_data_insert" );
titleBuilder.CreateRatingInsertFiles();
