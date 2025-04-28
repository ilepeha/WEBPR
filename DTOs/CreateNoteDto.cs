namespace NotesManager.Api.DTOs
{
    public class CreateNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}