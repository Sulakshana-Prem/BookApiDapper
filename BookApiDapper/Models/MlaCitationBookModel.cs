namespace BookApiDapper.Models
{
    public class MlaCitationBookModel
    {
        public string AuthorLastName { get; set; }
        public string AuthorFirstName { get; set; }
        public string TitleOfSource { get; set; }
        public string TitleOfContainer { get; set; }
        public string Publisher { get; set; }

        public string PublicationDate { get; set; }

        public string Location { get; set; }
    }
}
