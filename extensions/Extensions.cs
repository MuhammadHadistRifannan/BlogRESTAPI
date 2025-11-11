using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PersonalBlog;

public static class Extensions
{
    public static PostinganDTO ToDto(this Postingan _postingan)
    {
        var tags = _postingan.tags!.Select(e => e.name).ToList();
        return new PostinganDTO
        {
            title = _postingan.title,
            content = _postingan.content,
            category = _postingan.category,
            tags = tags!
        };
    }

    public static bool IsValid(this PostinganDTO dTO)
    {
        if (dTO.title != null && dTO.category != null && dTO.content != null && dTO.tags != null) return true;
        else return false;
    }
}
