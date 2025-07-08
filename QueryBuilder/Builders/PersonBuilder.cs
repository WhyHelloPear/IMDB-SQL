using System.Text;
using static IMDB_DB.Constants;

namespace IMDB_DB.Builders
{
    internal class PersonBuilder : BaseBuilder
    {
        private string TargetDataFile => $@"{_inputDataBaseDir}\{FileSchema.FileName}";
        public PersonBuilder( string outputDir, string inputDataFile, string fileName ) : base (outputDir, inputDataFile, fileName) {
            List<string> columnNames = new List<string> {
                SqlSchemaInfo.ColumnNames.PersonImdbId,
                SqlSchemaInfo.ColumnNames.Original_PersonImdbId,
                SqlSchemaInfo.ColumnNames.PrimaryName,
                SqlSchemaInfo.ColumnNames.BirthYear,
                SqlSchemaInfo.ColumnNames.DeathYear,
                SqlSchemaInfo.ColumnNames.PrimaryProfession,
                SqlSchemaInfo.ColumnNames.KnownForTitles,
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
                    dto.PersonId.ToString(),
                    dto.Original_PersonId,
                    dto.PrimaryName,
                    dto.BirthYear,
                    dto.DeathYear,
                    dto.PrimaryProfession,
                    dto.KnownForTitles,
                };

                return StaticHandler.CreateInsertRowFromValues( values );
            } ).ToList();

            StaticHandler.WriteBatchFile( _outputDir, _fileName, _insertHeader, valueRows, currentBatchCount );
        }

        private static class FileSchema
        {
            public const string FileName = "name.basics.tsv";
            public enum Indices
            {
                PersonId = 0,
                PrimaryName,
                BirthYear,
                DeathYear,
                PrimaryProfession,
                KnownForTitles
            }
        }

        private static class SqlSchemaInfo
        {
            public const string Table = "Person";
            public static class ColumnNames
            {
                public const string PersonImdbId = "PersonImdbId";
                public const string PrimaryName = "PrimaryName";
                public const string BirthYear = "BirthYear";
                public const string DeathYear = "DeathYear";
                public const string PrimaryProfession = "PrimaryProfession";
                public const string KnownForTitles = "KnownForTitles";
                public const string Original_PersonImdbId = "Original_PersonImdbId";
            }
        }

        private class Dto
        {

            public Dto( string dataLine )
            {
                string[] t = dataLine.Split( DataParsing.DELIMITER );

                PersonId = t[(int)FileSchema.Indices.PersonId].ParseImdbId( ImdbIdPrefix.Person );
                Original_PersonId = t[(int)FileSchema.Indices.PersonId];

                PrimaryName = t[(int)FileSchema.Indices.PrimaryName];
                BirthYear = t[(int)FileSchema.Indices.BirthYear];
                DeathYear = t[(int)FileSchema.Indices.DeathYear];
                PrimaryProfession = t[(int)FileSchema.Indices.PrimaryProfession];
                KnownForTitles = t[(int)FileSchema.Indices.KnownForTitles];
            }

            public long PersonId { get; set; }
            public string PrimaryName { get; set; }
            public string BirthYear { get; set; }
            public string DeathYear { get; set; }
            public string PrimaryProfession { get; set; }
            public string KnownForTitles { get; set; }
            public string Original_PersonId { get; set; }
        }
    }
}
