using IMDB_DB.DTO;
using System.Text;

namespace IMDB_DB.Builders
{
    internal class PersonPositionBuilder
    {
        private string _fileName;
        private string _outputDir;
        private string _inputDataFile;

        public PersonPositionBuilder( string outputDir, string inputDataFile, string fileName )
        {
            _outputDir = outputDir;
            _inputDataFile = inputDataFile;
            _fileName = fileName;
        }

        public void CreateRatingInsertFiles()
        {
            using var reader = new StreamReader( _inputDataFile );

            int batchSize = 1000; // Number of rows per INSERT statement
            var valueBatch = new TitlePersonPositionDto[batchSize];
            string? line;

            int readerLine = 0;
            int sqlLineCount = 0;
            int currentBatchCount = 0;
            while( ( line = reader.ReadLine() ) != null ) {
                if( readerLine == 0 ) {
                    readerLine++;
                    continue;
                }

                string[] t = line.Split( Constants.DELIMITER );

                string imdbId = t[(int)PersonPositionFileSchema.Indices.ImdbId];
                string directors = t[(int)PersonPositionFileSchema.Indices.Directors].Replace( @"\N", "" );
                string writers = t[(int)PersonPositionFileSchema.Indices.Writers].Replace( @"\N", "" );
                if( string.IsNullOrEmpty( directors ) && string.IsNullOrEmpty( writers ) ) {
                    continue;
                }

                List<TitlePersonPositionDto> titleWritersAndDirectors = [
                    .. writers.Split(',').Where(w => !string.IsNullOrWhiteSpace(w)).Select(w => new TitlePersonPositionDto(imdbId, w, (int)Constants.TitlePositions.Writer)),
                    .. directors.Split(',').Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => new TitlePersonPositionDto(imdbId, d, (int)Constants.TitlePositions.Director))
                ];

                foreach( var dto in titleWritersAndDirectors ) {
                    valueBatch[sqlLineCount] = dto;
                    sqlLineCount++;

                    if( sqlLineCount >= batchSize ) {
                        WriteBatchFile( valueBatch, currentBatchCount );
                        currentBatchCount++;
                        sqlLineCount = 0;
                        valueBatch = new TitlePersonPositionDto[batchSize];
                    }
                }
                readerLine++;
            }

            if( valueBatch.Any() ) {
                WriteBatchFile( valueBatch, currentBatchCount );
            }
        }

        private void WriteBatchFile( TitlePersonPositionDto[] values, int currentBatchCount )
        {
            var headerBuilder = new StringBuilder();
            headerBuilder.AppendLine( "USE IMDB;" );
            headerBuilder.AppendLine( $"INSERT INTO {PersonPositionFileSchema.SqlTableName} ( {PersonPositionFileSchema.ImdbIdColName}, {PersonPositionFileSchema.PersonColName}, {PersonPositionFileSchema.PositionColName} )" );
            headerBuilder.Append( $"VALUES" );

            List<string> valueRows = values.Where( v => v != null ).Select( dto => {
                var rowBuilder = new StringBuilder();
                rowBuilder.Append( "\t" );
                rowBuilder.Append( $"( '{dto.ImdbId}'" );
                rowBuilder.Append( $", '{dto.PersonImdbId}'" );
                rowBuilder.Append( $", '{dto.PositionId}' )," );
                return rowBuilder.ToString();
            } ).ToList();

            valueRows[valueRows.Count - 1] = $"{valueRows.Last().TrimEnd( ',' )};";

            StaticHandler.WriteBatchFile( _outputDir, _fileName, headerBuilder.ToString(), valueRows, currentBatchCount );
        }
    }

    public static class PersonPositionFileSchema
    {
        public enum Indices
        {
            ImdbId = 0,
            Directors,
            Writers
        }

        public const string FileName = "title.crew.tsv";

        public const string SqlTableName = "TitlePersonPosition";

        public const string ImdbIdColName = "ImdbId";
        public const string PersonColName = "PersonImdbId";
        public const string PositionColName = "PositionId";
    }
}
