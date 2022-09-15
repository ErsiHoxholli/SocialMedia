using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestFinal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TestFinal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }
    public IActionResult Index()

    {

        if (HttpContext.Session.GetInt32("userId") == null)
        {
            return RedirectToAction("Register");
        }
        int id = (int)HttpContext.Session.GetInt32("userId");


        ViewBag.iLoguari = _context.Users.FirstOrDefault(e => e.UserId == id);

    // ViewBag.Allusers = _context.Users.Include(e=>e.RequestsReciver).Include(e=>e.RequestsSender)
    //                                     .Where(e => e.UserId != id)
    //                                     .Where(e=>(e.RequestsReciver.Any(f => f.ReciverId == id) == false) 
    //                                             && (e.RequestsSender.Any(f => f.SenderId == id) == false))
    //                                     .ToList();

        ViewBag.Allusers = _context.Users
                            .Include(e => e.RequestsReciver)
                            .Include(e => e.RequestsSender)
                            .Where(e => e.UserId != id)
                            .Where(e =>
                                        (e.RequestsSender.Any(f => f.ReciverId == id) == false)
                                        && (e.RequestsReciver.Any(f => f.SenderId == id) == false)
                            )
                            .ToList();

                    ViewBag.request = _context.Requests.Include(e => e.Reciver).Include(e => e.Sender)
                                        .Where(e => e.ReciverId == id)
                                        .Where(e => e.Accepted == false)
                                        .ToList();

                    ViewBag.friends = _context.Requests.Include(e => e.Reciver).Include(e => e.Sender)
                                        .Where(e => (e.ReciverId == id ) || (e.SenderId == id ))
                                        .Where(e => e.Accepted == true)
                                        .ToList();

                    ViewBag.posts = _context.Posts.Include(e => e.Creator).Include(e=>e.Likes).Include(e=>e.Comments).ThenInclude(e=>e.UseriQekomenton).ThenInclude(e=>e.RequestsReciver)
                                            // .Where(e=>e.UserId != id)
                                            // .Where(e=>(e.Creator.RequestsReciver.Where(e=>e.ReciverId == id)) )
                                            .ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet("Register")]
    public IActionResult Register()
    {


        if (HttpContext.Session.GetInt32("userId") == null)
        {

            return View();
        }

        return RedirectToAction("Index");

    }
    [HttpPost("Register")]
    public IActionResult Register(User user)
    {
        // Check initial ModelState
        if (ModelState.IsValid)
        {
            // If a User exists with provided email
            if (_context.Users.Any(u => u.UserName == user.UserName))
            {
                // Manually add a ModelState error to the Email field, with provided
                // error message
                ModelState.AddModelError("UserName", "UserName already in use!");

                return View();
                // You may consider returning to the View at this point
            }
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            user.Password = Hasher.HashPassword(user, user.Password);
            _context.Users.Add(user);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("userId", user.UserId);

            return RedirectToAction("Index");
        }
        return View();
    }

    [HttpPost("Login")]
    public IActionResult LoginSubmit(LoginUser userSubmission)
    {
        if (ModelState.IsValid)
        {
            // If initial ModelState is valid, query for a user with provided email
            var userInDb = _context.Users.FirstOrDefault(u => u.UserName == userSubmission.UserName);
            // If no user exists with provided email
            if (userInDb == null)
            {
                ModelState.AddModelError("User", "Invalid UserName/Password");
                return View("Register");
            }

            var hasher = new PasswordHasher<LoginUser>();
            var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
            if (result == 0)
            {
                ModelState.AddModelError("Password", "Invalid Password");
                return View("Register");
            }
            HttpContext.Session.SetInt32("userId", userInDb.UserId);

            return RedirectToAction("Index");
        }

        return View("Register");
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {

        HttpContext.Session.Clear();
        return RedirectToAction("register");
    }

    // Pjesa me request


    [HttpGet("SendR/{id}")]
    public IActionResult SendR(int id)
    {
        int idFromSession = (int)HttpContext.Session.GetInt32("userId");
        if (_context.Requests.Any(u => (u.SenderId == idFromSession) && (u.ReciverId == id)))
        {

            return RedirectToAction("index");
        }
        else
        {

            Request newRequest = new Models.Request()
            {
                SenderId = idFromSession,
                ReciverId = id,

            };
            _context.Requests.Add(newRequest);
            _context.SaveChanges();
            return RedirectToAction("index");
        }


    }
    [HttpGet("AcceptR/{id}")]
    public IActionResult AcceptR(int id)
    {

        Request requestii = _context.Requests.First(e => e.RequestId == id);
        requestii.Accepted = true;

        _context.SaveChanges();
        return RedirectToAction("index");
    }
    [HttpGet("DeclineR/{id}")]
    public IActionResult Decline(int id)
    {

        Request requestii = _context.Requests.First(e => e.RequestId == id);
        _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }
    [HttpGet("RemoveF/{id}")]
    public IActionResult RemoveF(int id)
    {

        Request requestii = _context.Requests.First(e => e.RequestId == id);
        _context.Remove(requestii);
        _context.SaveChanges();
        return RedirectToAction("index");
    }

    public IActionResult PostAdd(int id)
    {
        ViewBag.id = id;
        return View();
    }

    [HttpPost]
    public IActionResult PostCreate(Post marrNgaView)
    {
        if (ModelState.IsValid)
        {
            int id = (int)HttpContext.Session.GetInt32("userId");


            marrNgaView.UserId = id;
            _context.Posts.Add(marrNgaView);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return View("PostAdd");
    }

    public IActionResult Like(int id, int id2)
    {

        if (_context.Likes.Any(u => u.UserId == id && u.PostId == id2))
        {
            return RedirectToAction("Index");
        }
        else
        {
            Like mylike = new Like()
            {
                UserId = id,
                PostId = id2
            };
            _context.Add(mylike);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }

    public IActionResult CommentCreate(int id, int id2, string content)
    {


        Comment mylike = new Comment()
        {
            UserId = id,
            PostId = id2,
            content = content
        };
        _context.Add(mylike);
        _context.SaveChanges();
        return RedirectToAction("Index");


    }






    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
