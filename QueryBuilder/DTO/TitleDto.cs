using IMDB_DB.Builders;

namespace IMDB_DB.DTO
{
    public class TitleDto
    {

        public TitleDto( string dataLine, char delimiter )
        {
            string[] t = dataLine.Split( delimiter );

            ImdbId = t[(int)TitleFileSchema.Indices.ImdbId];
            MediaType = t[(int)TitleFileSchema.Indices.Type];
            PrimaryTitle = t[(int)TitleFileSchema.Indices.PrimaryTitle];
            OriginalTitle = t[(int)TitleFileSchema.Indices.OriginalTitle];
            StartYear = t[(int)TitleFileSchema.Indices.StartYear];
            EndYear = t[(int)TitleFileSchema.Indices.EndYear];
            Genres = t[(int)TitleFileSchema.Indices.Genres];

            if( bool.TryParse( t[(int)TitleFileSchema.Indices.IsAdult], out bool isAdult ) ) {
                IsAdult = isAdult;
            }
            
            if( int.TryParse( t[(int)TitleFileSchema.Indices.RuntimeMinutes], out int runtime ) ) {
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
