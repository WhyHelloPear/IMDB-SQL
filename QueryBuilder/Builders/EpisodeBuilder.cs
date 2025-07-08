using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class EpisodeBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public EpisodeBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.EpisodeImdbId,
                SqlSchemaInfo.ColumnNames.SeriesImdbId,
                SqlSchemaInfo.ColumnNames.SeasonNumber,
                SqlSchemaInfo.ColumnNames.EpisodeNumber,
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

                valueBatch[sqlLineCount] = new Dto( line );
                sqlLineCount++;

                if( sqlLineCount >= batchSize ) {
                    WriteBatchFile( valueBatch, currentBatchCount );
                    currentBatchCount++;
                    sqlLineCount = 0;
                    valueBatch = new Dto[batchSize];
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
                    dto.EpisodeImdbId.ToString(),
                    dto.SeriesImdbId.ToString(),
                    dto.SeasonNumber,
                    dto.EpisodeNumber,
                };

                return StaticHandler.CreateInsertRowFromValues( values );

            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "title.episode.tsv";
            public enum Indices
            {
                EpisodeImdbId = 0,
                SeriesImdbId,
                SeasonNumber,
                EpisodeNumber,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "TitleEpisode";
            public static class ColumnNames
            {
                public const string EpisodeImdbId = "EpisodeImdbId";
                public const string SeriesImdbId = "SeriesImdbId";
                public const string SeasonNumber = "SeasonNumber";
                public const string EpisodeNumber = "EpisodeNumber";
            }
        }

        private class Dto
        {
            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                EpisodeImdbId = t[(int)FileSchema.Indices.EpisodeImdbId].ParseImdbId( ImdbIdPrefix.Title );
                SeriesImdbId = t[(int)FileSchema.Indices.SeriesImdbId].ParseImdbId( ImdbIdPrefix.Title );
                SeasonNumber = t[(int)FileSchema.Indices.SeasonNumber];
                EpisodeNumber = t[(int)FileSchema.Indices.EpisodeNumber];
            }

            public long EpisodeImdbId { get; set; }
            public long SeriesImdbId { get; set; }
            public string SeasonNumber { get; set; }
            public string EpisodeNumber { get; set; }
        }
    }
}