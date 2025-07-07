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
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.Append( $"INSERT INTO {FileSchema.SqlTableName} (" );
            headerBuilder.Append( $"{FileSchema.ImdbIdColName}, {FileSchema.OrderingColName}, {FileSchema.PersonIdColName}" );
            headerBuilder.AppendLine( $", {FileSchema.CategoryColName}, {FileSchema.JobColName}, {FileSchema.CharactersColName} )" );
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
        
        private static class FileSchema
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

        private class Dto
        {

            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( Constants.DELIMITER );

                ImdbId = t[(int)FileSchema.Indices.ImdbId];
                PersonId = t[(int)FileSchema.Indices.PersonId];
                Category = t[(int)FileSchema.Indices.Category];
                Job = t[(int)FileSchema.Indices.Job];
                Characters = t[(int)FileSchema.Indices.Characters];

                if( int.TryParse( t[(int)FileSchema.Indices.Ordering], out int order ) ) {
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
}