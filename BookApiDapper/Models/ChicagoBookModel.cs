namespace BookApiDapper.Models
{
    public class ChicagoBookModel
    {
        public string AuthorLastName { get; set; }
        public string AuthorFirstName { get; set; }

        public string PublicationDate { get; set; }
        public string TitleOfSource { get; set; }
        public string JournalTitle { get; set; }
        public string VolumeNo { get; set; }
        public string IssueNo { get; set; }

        public string PageRange { get; set; }

        public string URL  { get; set; }
    }
}
