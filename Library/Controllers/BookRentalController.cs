using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Library.Requests;
using Library.DataBaseContext;
using Library.Model;
using LessonApiBiblioteka.Requests;

[ApiController]
[Route("api/[controller]")]
public class BookRentalController : ControllerBase
{
    private readonly LibraryDB _context;

    public BookRentalController(LibraryDB context)
    {
        _context = context;
    }

    [HttpPost("rent")]
    public async Task<IActionResult> RentBook([FromQuery] RentBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Данные запроса неверные.");
        }

        try
        {
            var book = await _context.Books.FindAsync(request.Id_book);
            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            if (book.Copies <= 0)
            {
                return BadRequest("Нет доступных копий для аренды.");
            }

            var reader = await _context.Readers.FindAsync(request.Id_reader);
            if (reader == null)
            {
                return NotFound("Читатель не найден.");
            }

            var rental = new Rent_story
            {
                Id_book = request.Id_book,
                Id_reader = reader.Id_reader,
                RentalDate = DateTime.UtcNow,
                DueDate = request.DueDate
            };

            book.Copies--;

            _context.Rent_story.Add(rental);
            await _context.SaveChangesAsync();

            return Ok("Книга арендована.");
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Ошибка базы данных: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }

    [HttpPost("return")]
    public async Task<IActionResult> ReturnBook([FromQuery] ReturnBookRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Данные запроса неверные.");
        }

        try
        {
            var rental = await _context.Rent_story.FindAsync(request.RentalId);
            if (rental == null)
            {
                return NotFound("Аренда не найдена.");
            }

            var book = await _context.Books.FindAsync(rental.Id_book);
            if (book == null)
            {
                return NotFound("Книга не найдена.");
            }

            rental.ReturnDate = DateTime.UtcNow;
            book.Copies++;

            await _context.SaveChangesAsync();

            return Ok("Книга возвращена.");
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, $"Ошибка базы данных: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }

    [HttpGet("user/{id_reader}/history")]
    public async Task<IActionResult> GetRentalHistoryByUser(int id_reader)
    {
        try
        {
            var rentals = await _context.Rent_story
                .Where(r => r.Id_reader == id_reader)
                .Include(r => r.Books)
                .Include(r => r.Readers)
                .Select(r => new
                {
                    BookTitle = r.Books.Title,
                    UserName = r.Readers.First_name + " " + r.Readers.Last_name,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate,
                    ReturnDate = r.ReturnDate
                })
                .ToListAsync();

            if (rentals == null || rentals.Count == 0)
            {
                return NotFound("История аренды не найдена.");
            }

            return Ok(rentals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRentals()
    {
        try
        {
            var rentals = await _context.Rent_story
                .Where(r => r.ReturnDate == null)
                .Include(r => r.Books)
                .Include(r => r.Readers)
                .Select(r => new
                {
                    BookTitle = r.Books.Title,
                    UserName = r.Readers.First_name + " " + r.Readers.Last_name,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate
                })
                .ToListAsync();

            return Ok(rentals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }

    [HttpGet("book/{id_book}/history")]
    public async Task<IActionResult> GetRentalHistoryByBook(int id_book)
    {
        try
        {
            var rentals = await _context.Rent_story
                .Where(r => r.Id_book == id_book)
                .Include(r => r.Books)
                .Include(r => r.Readers)
                .Select(r => new
                {
                    BookTitle = r.Books.Title,
                    UserName = r.Readers.First_name + " " + r.Readers.Last_name,
                    RentalDate = r.RentalDate,
                    DueDate = r.DueDate,
                    ReturnDate = r.ReturnDate
                })
                .ToListAsync();

            if (rentals == null || rentals.Count == 0)
            {
                return NotFound("История аренды для книги не найдена.");
            }

            return Ok(rentals);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Ошибка сервера: {ex.Message}");
        }
    }
}