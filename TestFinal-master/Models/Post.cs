using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

public class Post
{
    [Key]
    public int PostId { get; set; }
    [Required]
    public string Description { get; set; }
    
    [Required]
    public int UserId { get; set; }
    // Navigation property for related User object
    public User? Creator { get; set; }
    // public List<Like> Likers { get; set; } = new List<Like>(); 
    // public List<Fans> Fansat { get; set; } = new List<Fans>(); 
    public List<Like> Likes {get;set;} = new List<Like>();
    public List<Comment> Comments {get;set;} = new List<Comment>();


    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;


}