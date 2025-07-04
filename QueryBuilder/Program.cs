using IMDB_DB;
using IMDB_DB.Builders;


//string baseDataDir = @"D:\IMDB_SQL\data";
//string outputDir = @"E:\MovieLibrary\imdb_sql";
string baseDataDir = @"E:\MovieLibrary\imdb_sql";
string outputDir = @"E:\MovieLibrary\imdb_sql\sql_output\ratings";
string fileName = "rating_data_insert";
string inputPath = @$"{baseDataDir}\data\{RatingFileSchema.FileName}";

var ratingBuilder = new RatingBuilder( outputDir, inputPath, fileName );
ratingBuilder.CreateRatingInsertFiles();
