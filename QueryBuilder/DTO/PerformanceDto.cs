using IMDB_DB.Builders;

namespace IMDB_DB.DTO
{
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
