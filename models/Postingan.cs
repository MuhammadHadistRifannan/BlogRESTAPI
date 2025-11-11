using System.ComponentModel.DataAnnotations;

namespace PersonalBlog;

public class Postingan
{
    [Key]
    public int Id { get; set; }
    [MaxLength(100)]
    public string? title { get; set; }
    [MaxLength(100)]    
    public string? content { get; set; }
    [MaxLength(100)]
    public string? category { get; set; }
    public List<Tag>? tags { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
}
