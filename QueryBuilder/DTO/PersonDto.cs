using IMDB_DB.Builders;

namespace IMDB_DB.DTO
{
    public class PersonDto
    {

        public PersonDto( string dataLine, char delimiter )
        {
            string[] t = dataLine.Split( delimiter );

            PersonId = t[(int)PersonFileSchema.Indices.PersonId];
            PrimaryName = t[(int)PersonFileSchema.Indices.PrimaryName];
            BirthYear = t[(int)PersonFileSchema.Indices.BirthYear];
            DeathYear = t[(int)PersonFileSchema.Indices.DeathYear];
            PrimaryProfession = t[(int)PersonFileSchema.Indices.PrimaryProfession];
            KnownForTitles = t[(int)PersonFileSchema.Indices.KnownForTitles];
        }

        public string PersonId { get; set; }
        public string PrimaryName { get; set; }
        public string BirthYear { get; set; }
        public string DeathYear { get; set; }
        public string PrimaryProfession { get; set; }
        public string KnownForTitles { get; set; }
    }
}
