using BookApiDapper.Models;

namespace BookApiDapper.Services
{
    public interface IBookService
    {
        Task<List<GetBookInfoModel>> GetBooksInfo();

        Task<List<GetBooksModel>> GetBooks();

        Task<string> AddBookInfo(BookModel bookModel);

        Task<decimal> GetTotalPrice();

        Task<List<BookModel>> GetAllBookDetails();
        Task<string> BulkInsertBooks(List<BookModel> books);

        Task<List<BookModel>> GetBookDetail();
        string MlaFormat();

        string ChicagoFormat();
    }
}
