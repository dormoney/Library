using Library.DataBaseContext;
using Library.Model;
using Library.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : Controller
    {
        readonly LibraryDB _context;
        public GenresController(LibraryDB context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("createNewGenre")]
        public async Task<IActionResult> CreateNewGenre([FromQuery] CreateNewGenre newGenre)
        {
            if (newGenre == null || string.IsNullOrEmpty(newGenre.Name_genre))
            {
                return BadRequest("Genre data is null or invalid.");
            }

            try
            {
                var genre = new Genres()
                {
                    Name_genre = newGenre.Name_genre
                };

                await _context.Genres.AddAsync(genre);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllGenres), new { id_genre = genre.Id_genre }, genre);
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

        [HttpGet]
        [Route("getAllGenres")]
        public async Task<IActionResult> GetAllGenres()
        {
            try
            {
                var genres = await _context.Genres.ToListAsync();
                return Ok(new
                {
                    genres = genres,
                    status = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateGenre/{id_genre}")]
        public async Task<IActionResult> UpdateGenre(int id_genre, [FromQuery] UpdateGenres updateGenres)
        {
            if (updateGenres == null || string.IsNullOrEmpty(updateGenres.Name_genre))
            {
                return BadRequest("Update data is null or invalid.");
            }

            try
            {
                var genre = await _context.Genres.FindAsync(id_genre);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id_genre} not found.");
                }

                genre.Name_genre = updateGenres.Name_genre;
                _context.Genres.Update(genre);
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

        [HttpDelete("DeleteGenre/{id_genre}")]
        public async Task<IActionResult> DeleteGenre(int id_genre)
        {
            try
            {
                var genre = await _context.Genres.FindAsync(id_genre);
                if (genre == null)
                {
                    return NotFound($"Genre with ID {id_genre} not found.");
                }

                _context.Genres.Remove(genre);
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
    }
}