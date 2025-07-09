﻿using static IMDB_DB.Constants;
using static IMDB_DB.FileColumnIndices;

namespace IMDB_DB.Builders
{
    internal class TitleBuilder : BaseBuilder
    {
        private Dictionary<string, int> TitleTypeMappings = new Dictionary<string, int>{
            { "short", (int)TitleType.Short },
            { "movie", (int)TitleType.Movie },
            { "tvshort", (int)TitleType.TvShort },
            { "tvmovie", (int)TitleType.TvMovie },
            { "tvepisode", (int)TitleType.TvEpisode },
            { "tvseries", (int)TitleType.TvSeries },
            { "tvminiseries", (int)TitleType.TvMiniSeries },
            { "tvspecial", (int)TitleType.TvSpecial },
            { "video", (int)TitleType.Video },
            { "videogame", (int)TitleType.VideoGame },
            { "tvpilot", (int)TitleType.TvPilot }
        };

        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public TitleBuilder( string outputDir, string inputDataFile, string fileName ) : base( outputDir, inputDataFile, fileName )
        {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.ImdbId,
                SqlSchemaInfo.ColumnNames.Original_ImdbId,
                SqlSchemaInfo.ColumnNames.TitleTypeId,
                SqlSchemaInfo.ColumnNames.PrimaryTitle,
                SqlSchemaInfo.ColumnNames.OriginalTitle,
                SqlSchemaInfo.ColumnNames.IsAdult,
                SqlSchemaInfo.ColumnNames.StartYear,
                SqlSchemaInfo.ColumnNames.EndYear,
                SqlSchemaInfo.ColumnNames.RuntimeMinutes,
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

                valueBatch[sqlLineCount] = new Dto( line, TitleTypeMappings );
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
                    dto.ImdbId.ToString(),
                    dto.Original_ImdbId,
                    dto.MediaTypeId.ToString(),
                    dto.PrimaryTitle,
                    dto.OriginalTitle,
                    dto.IsAdult ? "1" : "0",
                    dto.StartYear,
                    dto.EndYear,
                    dto.RuntimeMinutes.ToString(),
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
                ImdbId = TitleBasicsIndices.ImdbId,
                Type = TitleBasicsIndices.Type,
                PrimaryTitle = TitleBasicsIndices.PrimaryTitle,
                OriginalTitle = TitleBasicsIndices.OriginalTitle,
                IsAdult = TitleBasicsIndices.IsAdult,
                StartYear = TitleBasicsIndices.StartYear,
                EndYear = TitleBasicsIndices.EndYear,
                RuntimeMinutes = TitleBasicsIndices.RuntimeMinutes,
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "MediaTitle";
            public static class ColumnNames
            {
                public const string ImdbId = "ImdbId";
                public const string TitleTypeId = "TitleTypeId";
                public const string PrimaryTitle = "PrimaryTitle";
                public const string OriginalTitle = "OriginalTitle";
                public const string IsAdult = "IsAdult";
                public const string StartYear = "StartYear";
                public const string EndYear = "EndYear";
                public const string RuntimeMinutes = "RuntimeMinutes";
                public const string Original_ImdbId = "Original_ImdbId";
            }
        }

        private class Dto
        {
            public Dto( string dataLine, Dictionary<string, int> typeMappings )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId].ParseImdbId( ImdbIdPrefix.Title );
                Original_ImdbId = t[(int)FileSchema.Indices.ImdbId];

                PrimaryTitle = t[(int)FileSchema.Indices.PrimaryTitle];
                OriginalTitle = t[(int)FileSchema.Indices.OriginalTitle];
                StartYear = t[(int)FileSchema.Indices.StartYear];
                EndYear = t[(int)FileSchema.Indices.EndYear];

                if( bool.TryParse( t[(int)FileSchema.Indices.IsAdult], out bool isAdult ) ) {
                    IsAdult = isAdult;
                }

                if( int.TryParse( t[(int)FileSchema.Indices.RuntimeMinutes], out int runtime ) ) {
                    RuntimeMinutes = runtime;
                }

                string mediaType = t[(int)FileSchema.Indices.Type];
                MediaTypeId = typeMappings[mediaType.ToLower()];
            }

            public long ImdbId { get; set; }
            public int MediaTypeId { get; set; }
            public string PrimaryTitle { get; set; }
            public string OriginalTitle { get; set; }
            public bool IsAdult { get; set; }
            public string StartYear { get; set; }
            public string EndYear { get; set; }
            public int RuntimeMinutes { get; set; }
            public string Original_ImdbId { get; set; }
        }
    }
}
