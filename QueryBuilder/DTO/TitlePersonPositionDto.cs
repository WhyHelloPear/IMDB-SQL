namespace IMDB_DB.DTO
{
    public class TitlePersonPositionDto
    {
        public TitlePersonPositionDto( string imdbId, string personImdbId, int positionId )
        {
            ImdbId = imdbId;
            PersonImdbId = personImdbId;
            PositionId = positionId;
        }

        public string ImdbId { get; set; }
        public string PersonImdbId { get; set; }
        public int PositionId { get; set; }
    }
}
