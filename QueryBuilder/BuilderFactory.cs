using IMDB_DB.Builders;

namespace IMDB_DB
{
    public class BuilderFactory
    {
        private string _baseDataInputDir;
        private string _baseOutputDir;

        public BuilderFactory( string baseDataInputDir, string baseOutputDir )
        {
            _baseDataInputDir = baseDataInputDir;
            _baseOutputDir = baseOutputDir;
        }

        public void BuildAll()
        {
            BuildRatingsSql();
            BuildTitlesSql();
            BuildPersonsSql();
            BuildPerformancesSql();
            BuildPersonPositionSql();
            BuildAliasSql();
            BuildEpisodesSql();
        }

        public void BuildAliasSql()
        {
            var titleAliasBuilder = new TitleAliasBuilder( @$"{_baseOutputDir}\titleAlias", _baseDataInputDir, "titleAlias_data_insert" );
            titleAliasBuilder.CreateRatingInsertFiles();
        }

        public void BuildPersonPositionSql()
        {
            var personPositionBuilder = new PersonPositionBuilder( @$"{_baseOutputDir}\personPosition", _baseDataInputDir, "personPosition_data_insert" );
            personPositionBuilder.CreateRatingInsertFiles();
        }

        public void BuildPerformancesSql()
        {
            var performanceBuilder = new PerformanceBuilder( @$"{_baseOutputDir}\performances", _baseDataInputDir, "performances_data_insert" );
            performanceBuilder.CreateRatingInsertFiles();
        }

        public void BuildTitlesSql()
        {
            var titleBuilder = new TitleBuilder( @$"{_baseOutputDir}\titles", _baseDataInputDir, "titles_data_insert" );
            titleBuilder.CreateRatingInsertFiles();
        }

        public void BuildRatingsSql()
        {
            var ratingBuilder = new RatingBuilder( @$"{_baseOutputDir}\ratings", _baseDataInputDir, "rating_data_insert" );
            ratingBuilder.CreateRatingInsertFiles();
        }

        public void BuildPersonsSql()
        {
            var personBuilder = new PersonBuilder( @$"{_baseOutputDir}\persons", _baseDataInputDir, "persons_data_insert" );
            personBuilder.CreateRatingInsertFiles();
        }
        
        public void BuildEpisodesSql()
        {
            var episodeBuilder = new EpisodeBuilder( @$"{_baseOutputDir}\episodes", _baseDataInputDir, "episodes_data_insert" );
            episodeBuilder.CreateRatingInsertFiles();
        }
        
        public void BuildGenreLinksSql()
        {
            var episodeBuilder = new TitleGenreBuilder( @$"{_baseOutputDir}\genreLinks", _baseDataInputDir, "genreLinks_data_insert" );
            episodeBuilder.CreateRatingInsertFiles();
        }
    }
}
