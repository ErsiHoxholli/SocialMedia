using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;


public class Comment{
    [Key]
    public int CommentId {get;set;}
    public string content { get; set; }
    public int UserId {get;set;}
    public int PostId { get; set; }
    public User? UseriQekomenton {get;set;}
    public Post? PostiQekomentohet {get;set;}
}