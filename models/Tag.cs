using System.ComponentModel.DataAnnotations;

namespace PersonalBlog;

public class Tag
{
    [Key]
    public int id { get; set; }
    public string? name { get; set; }
    public List<Postingan>? posts { get; set; }
}
