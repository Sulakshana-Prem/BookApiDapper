using BookApiDapper.Models;
using BookApiDapper.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookApiDapper.Controllers
{
    public class BooksController : ControllerBase
    {
        private readonly IBookService bookService;
        public BooksController(IBookService _bookService) 
        { 
            this.bookService = _bookService;
        }

        [Route("/api/books/sorted-by-publisher-author-title")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBookInfoModel>>> GetBooksInfo()
        {
            List<GetBookInfoModel> books = new List<GetBookInfoModel>();
            books = await bookService.GetBooksInfo();
            return Ok(books);
        }

        [Route("/api/books/sorted-by-author-title")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetBooksModel>>> GetBooks() 
        {
            List<GetBooksModel> bookModels = new List<GetBooksModel>();
            bookModels = await bookService.GetBooks();
            return Ok(bookModels);
        }

        [Route("/api/books/save")]
        [HttpPost]
        public async Task<IActionResult> AddBookInfo(BookModel bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var values = await bookService.AddBookInfo(bookModel);
            return Ok(values);
        }

        [Route("/api/books/total-price")]
        [HttpGet]
        public async Task<IActionResult> GetTotalPrice()
        {
            var values = await bookService.GetTotalPrice();
            return Ok(values);
        }

        [Route("/api/books/bulkInsertBooks")]
        [HttpGet]
        public async Task<IActionResult> BulkInsertBooks()
        {
            string values = string.Empty;
            List<BookModel> books = new List<BookModel>();
            books = await bookService.GetAllBookDetails();
            if (books == null || !books.Any())
            {
                return BadRequest("No books provided for insertion.");
            }

            values = await bookService.BulkInsertBooks(books);
            return Ok(values);
        }

        [Route("/api/books/getall-books-detail")]
        [HttpGet]

        public async Task<IActionResult> GetBookDetail()
        {
            List<BookModel> books = new List<BookModel>();
            books = await bookService.GetBookDetail();
            return Ok(books);
        }

        [Route("/api/books/style-citation-mla")]
        [HttpGet]

        public IActionResult MlaFormat()
        {
            var values = bookService.MlaFormat();
            return Ok(values);
        }

        [Route("/api/books/style-citation-chicago")]
        [HttpGet]

        public IActionResult ChicagoFormat()
        {
            var values = bookService.ChicagoFormat();
            return Ok(values);
        }
    }
}
