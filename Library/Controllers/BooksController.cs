using Library.DataBaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Library.Model;
using Library.Requests;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        readonly LibraryDB _context;
        public BooksController(LibraryDB context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("getAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _context.Books.ToListAsync();
                return Ok(new { books, status = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FindById/{id_book}")]
        public async Task<IActionResult> FindById(int id_book)
        {
            try
            {
                var book = await _context.Books.FindAsync(id_book);
                if (book == null)
                {
                    return NotFound($"Book with ID {id_book} not found.");
                }

                var bookDto = new Find.GetAllBooksId
                {
                    Id_Books = book.Id_book,
                    Title = book.Title,
                    Author = book.Author,
                    Year_public = book.Year_public,
                    Id_genre = book.Id_genre,
                    Description = book.Description,
                    Copies = book.Copies
                };
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FindByIdGenre/{id_genre}")]
        public async Task<IActionResult> FindByIdGenre(int id_genre)
        {
            try
            {
                var books = await _context.Books.Where(b => b.Id_genre == id_genre).ToListAsync();
                return Ok(books.Select(b => new Find.GetAllBooksId
                {
                    Id_Books = b.Id_book,
                    Title = b.Title,
                    Author = b.Author,
                    Year_public = b.Year_public,
                    Id_genre = b.Id_genre,
                    Description = b.Description,
                    Copies = b.Copies
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("createNewBook")]
        public async Task<IActionResult> CreateNewBook([FromQuery] CreateNewBook newBook)
        {
            if (newBook == null)
            {
                return BadRequest("New book data is null.");
            }

            try
            {
                var book = new Books()
                {
                    Title = newBook.Title,
                    Author = newBook.Author,
                    Year_public = newBook.Year_public,
                    Id_genre = newBook.Id_genre,
                    Description = newBook.Description,
                    Copies = newBook.Copies,
                };

                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(FindById), new { id_book = book.Id_book }, book);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("DeleteBook/{id_book}")]
        public async Task<IActionResult> DeleteBook(int id_book)
        {
            try
            {
                var book = await _context.Books.FindAsync(id_book);
                if (book == null)
                {
                    return NotFound($"Book with ID {id_book} not found.");
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateBook/{id_book}")]
        public async Task<IActionResult> UpdateBook(int id_book, [FromQuery] UpdateBooks Updbook)
        {
            if (Updbook == null)
            {
                return BadRequest("Update data is null.");
            }

            try
            {
                var book = await _context.Books.FindAsync(id_book);
                if (book == null)
                {
                    return NotFound($"Book with ID {id_book} not found.");
                }

                book.Title = Updbook.Title;
                book.Author = Updbook.Author;
                book.Year_public = Updbook.Year_public;
                book.Description = Updbook.Description;
                book.Copies = Updbook.Copies;
                book.Id_genre = Updbook.Id_genre;

                _context.Books.Update(book);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Database update error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FindByAuthor/{AuthorOrTitle}")]
        public async Task<IActionResult> SearchBooks(string AuthorOrTitle)
        {
            try
            {
                var books = await _context.Books
                    .Where(b => b.Title.Contains(AuthorOrTitle) || b.Author.Contains(AuthorOrTitle))
                    .ToListAsync();

                if (books.Count == 0)
                {
                    return NotFound($"No books found for query '{AuthorOrTitle}'.");
                }

                var booksDto = books.Select(b => new CreateNewBook
                {
                    Title = b.Title,
                    Author = b.Author,
                    Id_genre = b.Id_genre,
                    Copies = b.Copies,
                    Year_public = b.Year_public,
                    Description = b.Description,
                });
                return Ok(booksDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FindCopies/{Title}")]
        public async Task<IActionResult> FindCopies(string Title)
        {
            try
            {
                var books = await _context.Books.Where(b => b.Title == Title).ToListAsync();
                if (books.Count == 0)
                {
                    return NotFound($"No copies found for book title '{Title}'.");
                }

                var booksDto = books.Select(t => new Find.FindCopies
                {
                    Copies = t.Copies
                });
                return Ok(booksDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}