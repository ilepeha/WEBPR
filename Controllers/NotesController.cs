using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesManager.Api.Data;
using NotesManager.Api.Models;
using NotesManager.Api.DTOs;
using System.Security.Claims;

namespace NotesManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetNotes(
            [FromQuery] string? title,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Notes.Where(n => n.UserId == userId);

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(n => n.Title.Contains(title));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= fromDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt < endDate.Value.Date.AddDays(1));
            }

            var notes = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateNote([FromBody] CreateNoteDto createNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var note = new Note
            {
                Title = createNoteDto.Title,
                Description = createNoteDto.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return Ok(note);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] CreateNoteDto updateNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingNote = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (existingNote == null)
            {
                return NotFound();
            }

            existingNote.Title = updateNoteDto.Title;
            existingNote.Description = updateNoteDto.Description;

            await _context.SaveChangesAsync();

            return Ok(existingNote);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var note = await _context.Notes
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}