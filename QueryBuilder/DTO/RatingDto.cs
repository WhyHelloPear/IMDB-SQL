using IMDB_DB.Builders;

namespace IMDB_DB.DTO
{
    public class RatingDto
    {
        public RatingDto( string dataLine )
        {
            string[] t = dataLine.Split( Constants.DELIMITER );

            ImdbId = t[(int)RatingFileSchema.Indices.ImdbId];
            if( decimal.TryParse( t[(int)RatingFileSchema.Indices.Rating], out decimal rating ) ) {
                Rating = rating;
            }

            if( int.TryParse( t[(int)RatingFileSchema.Indices.NumVotes], out int numVotes ) ) {
                NumVotes = numVotes;
            }
        }

        public string ImdbId { get; set; }
        public decimal Rating { get; set; } = 0;
        public int NumVotes { get; set; } = 0;
    }
}
