using System.Text;

namespace IMDB_DB.Builders
{
    internal class PerformanceBuilder
    {
        private string _fileName;
        private string _outputDir;
        private string _inputDataFile;

        public PerformanceBuilder( string outputDir, string inputDataFile, string fileName )
        {
            _outputDir = outputDir;
            _inputDataFile = inputDataFile;
            _fileName = fileName;
        }

        public void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new PerformanceDto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                valueBatch[sqlLineCount] = new PerformanceDto( line );
                sqlLineCount++;

                if( sqlLineCount >= batchSize ) {
                    WriteBatchFile( valueBatch, currentBatchCount );
                    currentBatchCount++;
                    sqlLineCount = 0;
                    valueBatch = new PerformanceDto[batchSize];
                }

                readerLine++;
            }

            if( valueBatch.Count() > 0 ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( PerformanceDto[] values, int currentBatchCount )
        {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.Append( $"INSERT INTO {PerformanceFileSchema.SqlTableName} (" );
            headerBuilder.Append( $"{PerformanceFileSchema.ImdbIdColName}, {PerformanceFileSchema.OrderingColName}, {PerformanceFileSchema.PersonIdColName}" );
            headerBuilder.AppendLine( $", {PerformanceFileSchema.CategoryColName}, {PerformanceFileSchema.JobColName}, {PerformanceFileSchema.CharactersColName} )" );
            headerBuilder.Append( $"VALUES" );

            List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                var rowBuilder = new StringBuilder();
                rowBuilder.Append( "\t" );
                rowBuilder.Append( $"( '{dto.ImdbId}'" );
                rowBuilder.Append( $", '{dto.Ordering}'" );
                rowBuilder.Append( $", '{dto.PersonId}'" );
                rowBuilder.Append( $", '{dto.Category.Replace( @"\N", "NULL" ).Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{dto.Job.Replace( @"\N", "NULL" ).Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{dto.Characters.Replace( @"\N", "NULL" ).Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}' )," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, headerBuilder.ToString(), valueRows, currentBatchCount );
        }
    }

    public static class PerformanceFileSchema
    {
        public enum Indices
        {
            ImdbId = 0,
            Ordering,
            PersonId,
            Category,
            Job,
            Characters
        }

        public const string FileName = "title.principals.tsv";
        public const string SqlTableName = "Performance";

        public const string ImdbIdColName = "ImdbId";
        public const string OrderingColName = "Ordering";
        public const string PersonIdColName = "PersonImdbId";
        public const string CategoryColName = "Category";
        public const string JobColName = "Job";
        public const string CharactersColName = "Characters";
    }

    public class PerformanceDto
    {

        public PerformanceDto( string dataLine )
        {
            string[] t = dataLine.Split( Constants.DELIMITER );

            ImdbId = t[(int)PerformanceFileSchema.Indices.ImdbId];
            PersonId = t[(int)PerformanceFileSchema.Indices.PersonId];
            Category = t[(int)PerformanceFileSchema.Indices.Category];
            Job = t[(int)PerformanceFileSchema.Indices.Job];
            Characters = t[(int)PerformanceFileSchema.Indices.Characters];

            if( int.TryParse( t[(int)PerformanceFileSchema.Indices.Ordering], out int order ) ) {
                Ordering = order;
            }
        }

        public string ImdbId { get; set; }
        public int Ordering { get; set; }
        public string PersonId { get; set; }
        public string Category { get; set; }
        public string Job { get; set; }
        public string Characters { get; set; }
    }
}
