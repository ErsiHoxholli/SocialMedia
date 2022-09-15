using System.ComponentModel.DataAnnotations;
namespace TestFinal.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;


public class Like{
    [Key]
    public int LikeId {get;set;}
    public int UserId {get;set;}
    public int PostId { get; set; }
    public User? UseriQePelqen {get;set;}
    public Post? PostiQePelqehet {get;set;}
}