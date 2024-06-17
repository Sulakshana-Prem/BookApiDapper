using BookApiDapper.Models;
using BookApiDapper.Services;
using Dapper;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookApiDapper.Repos
{
    public class BookRepo:IBookService
    {

        private readonly DbContext dbContext;

        public BookRepo(DbContext _dbContext) 
        { 
            this.dbContext = _dbContext;
        }

        public async Task<List<GetBookInfoModel>> GetBooksInfo()
        {
            IEnumerable<GetBookInfoModel> books;

            string storedProcedureName = "GetBooksSortedByPublisherAuthorTitle";

            using (var dbConnection = dbContext.CreateSQLServerConnection())
            {
                 //books = await dbConnection.QueryAsync<GetBookInfoModel>(storedProcedureName, null, commandType: CommandType.StoredProcedure);
                books = await dbConnection.QueryAsync<GetBookInfoModel>(storedProcedureName, commandType: CommandType.StoredProcedure);
            }

            return books.ToList();
        }


        public async Task<List<GetBooksModel>> GetBooks()
        {
            IEnumerable<GetBooksModel> books;

            string StoredProcedureName = "GetBooksSortedByAuthorTitle";

            using (var dbConnection = dbContext.CreateSQLServerConnection())
            {
                books = await dbConnection.QueryAsync<GetBooksModel>(StoredProcedureName,null, commandType: CommandType.StoredProcedure);

            }

            return books.ToList();
        }

        public async Task<string> AddBookInfo(BookModel bookModel)
        {
            int books = 0; 
            string query = "insert into Book (Publisher,Title,AuthorLastName,AuthorFirstName,Price) values (@Publisher,@Title,@AuthorLastName,@AuthorFirstName,@Price)";
            
            //Data inserted in Sqlite
            using (var dbConnection = dbContext.CreateSQLiteConnection())
            {
                books = await dbConnection.ExecuteAsync(query,bookModel);
                string values = "Inserted Successfully";
                return values;
            }

        }


        public async Task<decimal> GetTotalPrice()
        {
            decimal price;
            string storedProcedureName = "GetTotalPrice";
            using (var dbConnection = dbContext.CreateSQLServerConnection())
            {
                object result = await dbConnection.ExecuteScalarAsync(
                    storedProcedureName,
                    null,
                    commandType: CommandType.StoredProcedure
                );

                price = (result != null) ? Convert.ToDecimal(result) : 0;

                return price;
            }
        }


        public async Task<List<BookModel>> GetAllBookDetails()
        {
            List<BookModel> books = new List<BookModel>(); // Initialize the variable
            List<int> sqlIds;
            List<int> serverIds;

            string queryBooks = @"SELECT * FROM Book";

            // Retrieve Ids from BooksTable in SQLite
            using (var dbConnectionSQLite = dbContext.CreateSQLiteConnection())
            {
                sqlIds = (await dbConnectionSQLite.QueryAsync<int>("SELECT Id FROM Book")).ToList();
            }

            // Retrieve Ids from BooksTable in SQL Server
            using (var dbConnectionSQLServer = dbContext.CreateSQLServerConnection())
            {
                serverIds = (await dbConnectionSQLServer.QueryAsync<int>("SELECT Id FROM BooksTable")).ToList();
            }

            // Filter books based on matching Ids
            if (sqlIds.Except(serverIds).Any())
            {
                // Execute queryBooks to fetch books
                using (var dbConnection = dbContext.CreateSQLiteConnection())
                {
                    books = (await dbConnection.QueryAsync<BookModel>(queryBooks)).ToList();
                }
            }
            else
            {
                // Fetch books from SQLite if either list is empty
               
            }

            return books;
        }

        public async Task<string> BulkInsertBooks(List<BookModel> books)
        {
            string returnMessage = "";
            
            using (var sqlServerConnection = dbContext.CreateSQLServerConnection() as SqlConnection)
            {
                if (sqlServerConnection == null)
                    throw new InvalidOperationException("Expected SQL Server connection.");

                await sqlServerConnection.OpenAsync(); // Open the connection asynchronously

                using (var transaction = sqlServerConnection.BeginTransaction())
                {
                    try
                    {
                        // Convert list of books to DataTable
                        var dataTable = new DataTable();
                        dataTable.Columns.Add("Id", typeof(int));
                        dataTable.Columns.Add("Publisher", typeof(string));
                        dataTable.Columns.Add("AuthorLastName", typeof(string));
                        dataTable.Columns.Add("AuthorFirstName", typeof(string));
                        dataTable.Columns.Add("Title", typeof(string));
                        dataTable.Columns.Add("Price", typeof(decimal));
                        
                        foreach (var book in books)
                        {
                            dataTable.Rows.Add(book.Id, book.Publisher, book.AuthorLastName, book.AuthorFirstName, book.Title, book.Price);
                        }

                        // Use DynamicParameters to handle parameters
                        var parameters = new DynamicParameters();
                        parameters.Add("@BulkData", dataTable.AsTableValuedParameter("dbo.BooksTable"));
                        parameters.Add("@UserId", (int)BookModel.Role.Admin);

                        // Execute stored procedure with Dapper
                        returnMessage = await sqlServerConnection.QueryFirstOrDefaultAsync<string>(
                            "usp_BulkInsertWithHistory",
                            parameters,
                            commandType: CommandType.StoredProcedure,
                            transaction: transaction);

                        // Commit transaction
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        // Handle exceptions, rollback transaction if necessary
                        await transaction.RollbackAsync();
                        throw new Exception("Error occurred during bulk insert operation.", ex);
                    }
                }
            }

            return returnMessage;
        }

        public async Task<List<BookModel>> GetBookDetail()
        {

            List<BookModel> books;
            string query = "SELECT Id, Publisher, Title, AuthorLastName, AuthorFirstName, Price FROM BooksTable";

            using (var dbConnection = dbContext.CreateSQLServerConnection())
            {
                books = (await dbConnection.QueryAsync<BookModel>(query)).ToList();
                
            }

            return books;
        }


        public string MlaFormat()
        {
            string MLACitation = string.Empty;
            MlaCitationBookModel bookModel = new MlaCitationBookModel();
            bookModel.AuthorLastName = "Barzun, ";
            bookModel.AuthorFirstName = "Jacques. ";
            bookModel.TitleOfSource = "\"Behind the Blue Pencil: Censorship or Creeping Creativity?\"";
            bookModel.TitleOfContainer = "\n  On Writing, Editing, and Publishing, ";
            bookModel.Publisher = "U of Chicago P, ";
            bookModel.PublicationDate = "1986, ";
            bookModel.Location = "pp. 120-126.";
            MLACitation = $"{bookModel.AuthorLastName}{bookModel.AuthorFirstName}{bookModel.TitleOfSource}{bookModel.TitleOfContainer}{bookModel.Publisher}{bookModel.PublicationDate}{bookModel.Location}";
            return MLACitation;
        }

        public string ChicagoFormat()
        {
            string Chicago = string.Empty;
            ChicagoBookModel bookModel = new ChicagoBookModel();
            bookModel.AuthorLastName = "Schmidt, ";
            bookModel.AuthorFirstName = "John. ";
            bookModel.PublicationDate = "1935. ";
            bookModel.TitleOfSource = "\"Mystery of \n   a Talking Wombat.\" ";
            bookModel.JournalTitle = "Bulletin \n   of the Illinois Society for \n   Physical Research 217, no. ";
            bookModel.VolumeNo = "2 ";
            bookModel.IssueNo = "\n   (February 1935): ";
            bookModel.PageRange = "275-278. ";
            bookModel.URL = "\n   https://doi.org/10.xxxx/xxxxxx.";
            Chicago = $"{bookModel.AuthorLastName}{bookModel.AuthorFirstName}{bookModel.PublicationDate}{bookModel.TitleOfSource}{bookModel.JournalTitle}{bookModel.VolumeNo}{bookModel.IssueNo}{bookModel.PageRange}{bookModel.URL}";
            return Chicago;
        }

    }
}
