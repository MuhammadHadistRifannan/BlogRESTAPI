using System.Collections.Immutable;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PersonalBlog;

namespace MyApp.Namespace
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        public PostController(ApplicationDbContext _context)
        {
            dbContext = _context;
        }
        [HttpPost("new-post")]
        public IActionResult NewPost([FromBody] PostinganDTO postingan)
        {
            try
            {
                if (!postingan.IsValid()) return BadRequest(new
                {
                    message = "Validation Error"
                });

                //Create a new post 
                var newPost = new Postingan
                {
                    title = postingan.title,
                    content = postingan.content,
                    category = postingan.category,
                    tags = new List<Tag>(),
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now
                };
                var tags = new List<Tag>();

                //Check the tags if exist
                if (postingan.tags != null && postingan.tags.Count > 0)
                {
                    tags = dbContext.tags.Where(e => postingan.tags.Contains(e.name!)).ToList();
                    foreach (var tag in postingan.tags)
                    {
                        var tagExisted = tags.Where(e => e.name == tag).FirstOrDefault();
                        if (tagExisted != null)
                        {
                            newPost.tags!.Add(tagExisted);
                        }
                        else
                        {
                            //Update data tag di database 
                            dbContext.tags.Add(new Tag { name = tag });
                            newPost.tags!.Add(new Tag { name = tag, posts = new List<Postingan> { { newPost } } });
                        }
                    }
                }


                dbContext.posts.Add(newPost);
                dbContext.SaveChanges();

                return StatusCode(201, new
                {
                    message = "Data successfully created",
                    data = postingan
                });
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdatePost(int id, [FromBody] PostinganDTO postingan)
        {
            if (!postingan.IsValid()) return BadRequest(new
            {
                message = "Validation Error"
            });

            var post = dbContext.posts.Where(e => e.Id == id).FirstOrDefault();
            if (post == null) return NotFound(new
            {
                message = "blog not found"
            });

            var tags = new List<Tag>();
            if (postingan.tags != null && postingan.tags.Count > 0)
                foreach (var tag in postingan.tags)
                {
                    var isexist = dbContext.tags.Where(e => e.name == tag).FirstOrDefault();
                    if (isexist != null)
                    {
                        tags.Add(new Tag { name = tag });
                        post.tags = tags;
                    }
                    else
                    {
                        post.tags!.Add(new Tag { name = tag });
                    }
                }

            post.title = postingan.title;
            post.content = postingan.content;
            post.category = postingan.category;
            post.updated_at = DateTime.Now;

            dbContext.posts.Update(post);
            dbContext.SaveChanges();

            return Ok(new
            {
                message = "updated data blog successfully",
                data = postingan
            });
        }
        [HttpDelete("delete/{id}")]
        public IActionResult DeletePost(int id)
        {
            var post = dbContext.posts.Where(e => e.Id == id).FirstOrDefault();
            if (post == null) return NotFound(new
            {
                message = "Posts Not Found"
            });

            dbContext.posts.Remove(post!);
            dbContext.SaveChanges();
            return StatusCode(204);
        }
        [HttpGet("{id}")]
        public IActionResult GetPost(int id)
        {
            var post = dbContext.posts.Where(e => e.Id == id).FirstOrDefault();
            if (post == null) return NotFound(new
            {
                message = "Posts Not Found"
            });

            return Ok(post);
        }

        [HttpGet]
        public IActionResult GetAllPosts([FromQuery] string? title, [FromQuery] string? tags)
        {
            if (string.IsNullOrWhiteSpace(title) && (tags == null || tags.Split(',').Length == 0))
            {
                return Ok(dbContext.posts.ToList());
            }

            var query = dbContext.posts.Include(p => p.tags).AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(e => EF.Functions.Like(e.title, $"%{title}%"));
            }

            if (tags != null && tags.Length > 0)
            {
                var split = tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

                query = query.Where(p =>
                    p.tags!.Any(t =>
                        split.Any(tag =>
                            EF.Functions.Like(t.name!, "%" + tag + "%"))));
            }
            var result = query.Select(e => new { e.Id, e.title, e.content, e.category , tag = e.tags!.Select(t => t.name).ToList()}).ToList();
            return Ok(result);
        }


    }
}
