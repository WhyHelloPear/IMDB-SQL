using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class TitleGenreBuilder : BaseBuilder
    {
        private Dictionary<string, int> GenreMappings = new Dictionary<string, int>{
            { "documentary", (int)TitleGenres.Documentary },
            { "short", (int)TitleGenres.Short },
            { "animation", (int)TitleGenres.Animation },
            { "comedy", (int)TitleGenres.Comedy },
            { "romance", (int)TitleGenres.Romance },
            { "sport", (int)TitleGenres.Sport },
            { "news", (int)TitleGenres.News },
            { "drama", (int)TitleGenres.Drama },
            { "fantasy", (int)TitleGenres.Fantasy },
            { "horror", (int)TitleGenres.Horror },
            { "biography", (int)TitleGenres.Biography },
            { "music", (int)TitleGenres.Music },
            { "war", (int)TitleGenres.War },
            { "crime", (int)TitleGenres.Crime },
            { "western", (int)TitleGenres.Western },
            { "family", (int)TitleGenres.Family },
            { "adventure", (int)TitleGenres.Adventure },
            { "action", (int)TitleGenres.Action },
            { "history", (int)TitleGenres.History },
            { "mystery", (int)TitleGenres.Mystery },
            { "sci-fi", (int)TitleGenres.SciFi },
            { "musical", (int)TitleGenres.Musical },
            { "thriller", (int)TitleGenres.Thriller },
            { "film-noir", (int)TitleGenres.FilmNoir },
            { "talk-show", (int)TitleGenres.TalkShow },
            { "game-show", (int)TitleGenres.GameShow },
            { "reality-tv", (int)TitleGenres.RealityTV },
            { "adult", (int)TitleGenres.Adult }
        };

        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public TitleGenreBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.TitleGenreId,
            };

            _insertHeader = StaticHandler.CreateInsertHeaderRow( SqlSchemaInfo.Table, columnNames );
        }

        public override void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( TargetDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new Dto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                var originalLineDto = new Dto( line );
                List<Dto> splitDtos = originalLineDto.SplitDtoByGenre( GenreMappings );

                foreach( var splitDto in splitDtos ) {
                    valueBatch[sqlLineCount] = splitDto;
                    sqlLineCount++;

                    if( sqlLineCount >= batchSize ) {
                        WriteBatchFile( valueBatch, currentBatchCount );
                        currentBatchCount++;
                        sqlLineCount = 0;
                        valueBatch = new Dto[batchSize];
                    }
                }

                readerLine++;
            }

            if( valueBatch.Count() > 0 ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( Dto[] values, int currentBatchCount )
        {
            List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                List<string> values = new List<string> {
                    dto.ImdbId.ToString(),
                    dto.TitleGenreId.ToString(),
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.basics.tsv";
            public enum Indices
            {
                ImdbId = 0,
                Genres = 8,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "TitleGenreLink";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string TitleGenreId = "TitleGenreId";
            }
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId].ParseImdbId( ImdbIdPrefix.Title );
                string genreValues = t[(int)FileSchema.Indices.Genres].CleanSqlValue();

                RelatedGenres = genreValues == "NULL" ? new List<string>() : genreValues.Split( ',' ).ToList();
            }

            public Dto( long imdbId, int genreId )
            {
                ImdbId = imdbId;
                TitleGenreId = genreId;
            }

            public long ImdbId { get; set; }
            public int TitleGenreId { get; set; }
            public List<string> RelatedGenres { get; set; }

            public List<Dto> SplitDtoByGenre( Dictionary<string, int> mapper )
            {
                var result = new List<Dto>();
                foreach( var genre in RelatedGenres ) {
                    int genreId = mapper[genre.ToLower()];
                    result.Add( new Dto( ImdbId, genreId ) );
                }
                return result;
            }
        }
    }
}