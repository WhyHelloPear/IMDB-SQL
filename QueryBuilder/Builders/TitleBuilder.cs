using IMDB_DB.DTO;

namespace IMDB_DB.Builders
{
    internal class TitleBuilder
    {
        private string _fileName;
        private string _outputDir;
        private string _inputDataFile;

        public TitleBuilder( string outputDir, string inputDataFile, string fileName )
        {
            _outputDir = outputDir;
            _inputDataFile = inputDataFile;
            _fileName = fileName;
        }

        public void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new TitleDto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                valueBatch[sqlLineCount] = new TitleDto( line, '\t' );
                sqlLineCount++;

                if( sqlLineCount >= batchSize ) {
                    WriteBatchFile( valueBatch, currentBatchCount );
                    currentBatchCount++;
                    sqlLineCount = 0;
                    valueBatch = new TitleDto[batchSize];
                }

                readerLine++;
            }

            if( valueBatch.Count() > 0 ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( TitleDto[] values, int currentBatchCount )
        {
            string insertHeader = @$"
                INSERT INTO {TitleFileSchema.SqlTableName} 
                (
                    {TitleFileSchema.ImdbIdColName}, {TitleFileSchema.TitleTypeColName}, {TitleFileSchema.PrimaryTitleColName},
                    {TitleFileSchema.OriginalTitleColName}, {TitleFileSchema.IsAdultColName}, {TitleFileSchema.StartYearColName},
                    {TitleFileSchema.EndYearColName}, {TitleFileSchema.RuntimeMinutesColName}, {TitleFileSchema.GenresColName}
                ) 
                VALUES";

            List<string> valueRows = values.Where( v => v != null ).Select( dto =>
                $"('{dto.ImdbId}','{dto.MediaType}','{dto.PrimaryTitle.Replace( "'", "''" )}','{dto.OriginalTitle.Replace( "'", "''" )}','{(dto.IsAdult ? 1 : 0)}','{dto.StartYear}','{dto.EndYear.Replace( @"\N", "NULL" )}','{dto.RuntimeMinutes}','{dto.Genres}'),"
            ).ToList();

            valueRows[valueRows.Count - 1] = valueRows.Last().TrimEnd( ',' );

            StaticHandler.WriteBatchFile( _outputDir, _fileName, insertHeader, valueRows, currentBatchCount );
        }
    }

    public static class TitleFileSchema
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

        public const string SqlTableName = "dbo.MediaTitle";

        public const string ImdbIdColName = "ImdbId";
        public const string TitleTypeColName = "TitleType";
        public const string PrimaryTitleColName = "PrimaryTitle";
        public const string OriginalTitleColName = "OriginalTitle";
        public const string IsAdultColName = "IsAdult";
        public const string StartYearColName = "StartYear";
        public const string EndYearColName = "EndYear";
        public const string RuntimeMinutesColName = "RuntimeMinutes";
        public const string GenresColName = "Genres";

        public const string ImdbIdColType = "VARCHAR(16)";
        public const string RatingColType = "DECIMAL UNSIGNED ZEROFILL";
        public const string NumVotesColType = "INT";
    }
}
