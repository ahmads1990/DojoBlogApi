namespace DojoBlogApi.Models;

class Blog
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public Author Author { get; set; } = default!;
}
