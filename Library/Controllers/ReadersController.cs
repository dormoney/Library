using Library.DataBaseContext;
using Library.Model;
using Library.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReadersController : Controller
    {
        readonly LibraryDB _context;
        public ReadersController(LibraryDB context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("createNewReader")]
        public async Task<IActionResult> CreateNewReader([FromQuery] CreateNewReader newReader)
        {
            if (newReader == null || string.IsNullOrWhiteSpace(newReader.First_name) || string.IsNullOrWhiteSpace(newReader.Last_name))
            {
                return BadRequest("Reader data is null or invalid.");
            }

            try
            {
                var reader = new Readers()
                {
                    First_name = newReader.First_name,
                    Last_name = newReader.Last_name,
                    Birth_year = newReader.Birth_year,
                    Contact_info = newReader.Contact_info
                };

                await _context.Readers.AddAsync(reader);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(FindById), new { id_reader = reader.Id_reader }, reader);
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
        [Route("getAllReaders")]
        public async Task<IActionResult> GetAllReaders()
        {
            try
            {
                var readers = await _context.Readers.ToListAsync();
                return Ok(new { readers = readers, status = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FindById/{id_reader}")]
        public async Task<IActionResult> FindById(int id_reader)
        {
            try
            {
                var reader = await _context.Readers.FindAsync(id_reader);
                if (reader == null)
                {
                    return NotFound($"Reader with ID {id_reader} not found.");
                }

                var readerDto = new GetAllReadersId
                {
                    Id_reader = reader.Id_reader,
                    First_name = reader.First_name,
                    Last_name = reader.Last_name,
                    Birth_year = reader.Birth_year,
                    Contact_info = reader.Contact_info
                };
                return Ok(readerDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("UpdateReader/{id_reader}")]
        public async Task<IActionResult> UpdateReader(int id_reader, [FromQuery] UpdateReaders updreader)
        {
            if (updreader == null || string.IsNullOrWhiteSpace(updreader.First_name) || string.IsNullOrWhiteSpace(updreader.Last_name))
            {
                return BadRequest("Update data is null or invalid.");
            }

            try
            {
                var reader = await _context.Readers.FindAsync(id_reader);
                if (reader == null)
                {
                    return NotFound($"Reader with ID {id_reader} not found.");
                }

                reader.First_name = updreader.First_name;
                reader.Last_name = updreader.Last_name;
                reader.Birth_year = updreader.Birth_year;
                reader.Contact_info = updreader.Contact_info;

                _context.Readers.Update(reader);
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

        [HttpDelete("DeleteReader/{id_reader}")]
        public async Task<IActionResult> DeleteReader(int id_reader)
        {
            try
            {
                var reader = await _context.Readers.FindAsync(id_reader);
                if (reader == null)
                {
                    return NotFound($"Reader with ID {id_reader} not found.");
                }

                _context.Readers.Remove(reader);
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