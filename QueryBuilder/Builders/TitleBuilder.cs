using System.Text;

namespace IMDB_DB.Builders
{
    internal class TitleBuilder : BaseBuilder
    {
        public TitleBuilder( string outputDir, string inputDataFile, string fileName ) : base (outputDir, inputDataFile, fileName) {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.Append( $"INSERT INTO {FileSchema.SqlTableName} (" );
            headerBuilder.Append( $"{FileSchema.ImdbIdColName}, {FileSchema.TitleTypeColName}, {FileSchema.PrimaryTitleColName}" );
            headerBuilder.Append( $", {FileSchema.OriginalTitleColName}, {FileSchema.IsAdultColName}, {FileSchema.StartYearColName}" );
            headerBuilder.AppendLine( $", {FileSchema.EndYearColName}, {FileSchema.RuntimeMinutesColName}, {FileSchema.GenresColName} )" );
            headerBuilder.Append( $"VALUES" );

            _insertHeader = headerBuilder.ToString();
        }

        public override void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

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
                var rowBuilder = new StringBuilder();
                rowBuilder.Append( "\t" );
                rowBuilder.Append( $"( '{dto.ImdbId}'" );
                rowBuilder.Append( $", '{dto.MediaType}'" );
                rowBuilder.Append( $", '{dto.PrimaryTitle.Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{dto.OriginalTitle.Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{( dto.IsAdult ? 1 : 0 )}'" );
                rowBuilder.Append( $", '{dto.StartYear}'" );
                rowBuilder.Append( $", '{dto.EndYear.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.RuntimeMinutes}'" );
                rowBuilder.Append( $", '{dto.Genres.Truncate( 255 )}' )," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public enum Indices
            {
                ImdbId = 0,
                Type,
                PrimaryTitle,
                OriginalTitle,
                IsAdult,
                StartYear,
                EndYear,
                RuntimeMinutes,
                Genres,
            }

            public const string FileName = "title.basics.tsv";

            public const string SqlTableName = "MediaTitle";

            public const string ImdbIdColName = "ImdbId";
            public const string TitleTypeColName = "TitleType";
            public const string PrimaryTitleColName = "PrimaryTitle";
            public const string OriginalTitleColName = "OriginalTitle";
            public const string IsAdultColName = "IsAdult";
            public const string StartYearColName = "StartYear";
            public const string EndYearColName = "EndYear";
            public const string RuntimeMinutesColName = "RuntimeMinutes";
            public const string GenresColName = "Genres";
        }

        private class Dto
        {

            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( Constants.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId];
                MediaType = t[(int)FileSchema.Indices.Type];
                PrimaryTitle = t[(int)FileSchema.Indices.PrimaryTitle];
                OriginalTitle = t[(int)FileSchema.Indices.OriginalTitle];
                StartYear = t[(int)FileSchema.Indices.StartYear];
                EndYear = t[(int)FileSchema.Indices.EndYear];
                Genres = t[(int)FileSchema.Indices.Genres];

                if( bool.TryParse( t[(int)FileSchema.Indices.IsAdult], out bool isAdult ) ) {
                    IsAdult = isAdult;
                }

                if( int.TryParse( t[(int)FileSchema.Indices.RuntimeMinutes], out int runtime ) ) {
                    RuntimeMinutes = runtime;
                }
            }

            public string ImdbId { get; set; }
            public string MediaType { get; set; }
            public string PrimaryTitle { get; set; }
            public string OriginalTitle { get; set; }
            public bool IsAdult { get; set; }
            public string StartYear { get; set; }
            public string EndYear { get; set; }
            public int RuntimeMinutes { get; set; }
            public string Genres { get; set; }
        }
    }
}
