namespace BookApiDapper.Models
{
    public class GetBookInfoModel
    {
        public string Publisher { get; set; }
        public string AuthorLastName { get; set; }
        public string AuthorFirstName { get; set; }
        public string Title { get; set; }
    }
}
