using NotesManager.Api.Models;

namespace NotesManager.Api.Repositories
{
    public interface INotesRepository
    {
        Task<IEnumerable<Note>> GetNotesByUserAsync(string userId);
        Task<Note> GetNoteByIdAsync(int id);
        Task<Note> CreateNoteAsync(Note note);
        Task<Note> UpdateNoteAsync(Note note);
        Task<bool> DeleteNoteAsync(int id);
    }
}