using NotesManager.Api.Models;

namespace NotesManager.Api.Services
{
    public interface IAuthService
    {
        Task<string> GenerateTokenAsync(ApplicationUser user);
    }
}