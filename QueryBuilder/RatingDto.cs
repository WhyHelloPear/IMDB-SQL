namespace IMDB_DB
{
    public class RatingDto
    {
        public RatingDto( string dataLine, char delimiter )
        {
            string[] t = dataLine.Split( delimiter );

            ImdbId = t[RatingFileSchema.ImdbIdIndex];
            if(decimal.TryParse(t[RatingFileSchema.RatingIndex], out decimal rating ) ) {
                Rating = rating;
            }

            if(int.TryParse(t[RatingFileSchema.NumVotesIndex], out int numVotes ) ) {
                NumVotes = numVotes;
            }
        }

        public string ImdbId { get; set; }
        public decimal Rating { get; set; } = 0;
        public int NumVotes { get; set; } = 0;
    }
}
