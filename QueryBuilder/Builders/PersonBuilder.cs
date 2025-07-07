using System.Text;

namespace IMDB_DB.Builders
{
    internal class PersonBuilder
    {
        private string _fileName;
        private string _outputDir;
        private string _inputDataFile;

        public PersonBuilder( string outputDir, string inputDataFile, string fileName )
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
            headerBuilder.Append( $"{FileSchema.PersonIdColName}, {FileSchema.PrimaryNameColName}, {FileSchema.BirthYearColName}" );
            headerBuilder.AppendLine( $", {FileSchema.DeathYearColName}, {FileSchema.PrimaryProfessionColName}, {FileSchema.KnownForTitlesColName} )" );
            headerBuilder.Append( $"VALUES" );

            List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                var rowBuilder = new StringBuilder();
                rowBuilder.Append( "\t" );
                rowBuilder.Append( $"( '{dto.PersonId}'" );
                rowBuilder.Append( $", '{dto.PrimaryName.Truncate( 255 ).Replace( @"\'", "'" ).Replace( @"/'", "'" ).Replace( "'", "''" )}'" );
                rowBuilder.Append( $", '{dto.BirthYear.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.DeathYear.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.PrimaryProfession.Replace( @"\N", "NULL" )}'" );
                rowBuilder.Append( $", '{dto.KnownForTitles.Replace( @"\N", "NULL" )}' )," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, headerBuilder.ToString(), valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public enum Indices
            {
                PersonId = 0,
                PrimaryName,
                BirthYear,
                DeathYear,
                PrimaryProfession,
                KnownForTitles
            }

            public const string FileName = "name.basics.tsv";

            public const string SqlTableName = "Person";

            public const string PersonIdColName = "PersonImdbId";
            public const string PrimaryNameColName = "PrimaryName";
            public const string BirthYearColName = "BirthYear";
            public const string DeathYearColName = "DeathYear";
            public const string PrimaryProfessionColName = "PrimaryProfession";
            public const string KnownForTitlesColName = "KnownForTitles";
        }

        private class Dto
        {

            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( Constants.DELIMITER );

                PersonId = t[(int)FileSchema.Indices.PersonId];
                PrimaryName = t[(int)FileSchema.Indices.PrimaryName];
                BirthYear = t[(int)FileSchema.Indices.BirthYear];
                DeathYear = t[(int)FileSchema.Indices.DeathYear];
                PrimaryProfession = t[(int)FileSchema.Indices.PrimaryProfession];
                KnownForTitles = t[(int)FileSchema.Indices.KnownForTitles];
            }

            public string PersonId { get; set; }
            public string PrimaryName { get; set; }
            public string BirthYear { get; set; }
            public string DeathYear { get; set; }
            public string PrimaryProfession { get; set; }
            public string KnownForTitles { get; set; }
        }
    }
}
