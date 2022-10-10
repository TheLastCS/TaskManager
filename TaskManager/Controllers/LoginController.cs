using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
         
        private readonly Task_ManagerEntities entity;
        public LoginController()
        {
            entity = new Task_ManagerEntities();
        }

        public ActionResult Index() { return View(); }

        // Login Methods
        public ActionResult Login()
        { 
            return View(entity.Users.ToList()); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult SignIn_Action(FormCollection form)
        {
            User user = new User()
            {
                Email = form["Email"],
                Password = form["Password"]
            };
            var obj = entity.Users.Where(u => u.Email.Equals(user.Email) && u.Password.Equals(user.Password) && u.Status.Equals("Active")).FirstOrDefault();
            if ( obj != null)
            {
                Session["UserID"] = obj.UserID;
                Session["UserEmail"] = obj.Email;
                Session["UserName"] = obj.FirstName + " " + obj.LastName;
                Session["UserType"] = obj.UserType;
                Session.Timeout = 60;

                if (Session["UserType"].ToString().Contains("Admin"))
                {
                    return RedirectToAction("AdminTasks", "Task");
                }

                return RedirectToAction("UserTasks", "Task");
            } else
            {
                TempData["Error"] = "Email Does Not Exist / Password Incorrect";
                return RedirectToAction("Index", "Home");
            }

        }

        // Register Methods
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult SignUp_Action(FormCollection form)
        {
            User user = new User
            {
                FirstName = form["FirstName"],
                LastName = form["LastName"],
                Email = form["Email"],
                Password = form["Password"],
                UserType = "Admin",
                Status = "Active",
                TimeCreated = DateTime.Now
            };

            if (user.Email.Equals(entity.Users.Where(u => u.Email.Equals(user.Email)).FirstOrDefault()))
            {
                if (CheckInput(user) != false)
                {
                    entity.Users.Add(user);
                    entity.SaveChanges();
                    TempData["Success"] = "User Account Successfully Created";
                    return RedirectToAction("Index", "Home");
                }

            }
            TempData["Error"] = "Email Already Exists";
            return RedirectToAction("Index", "Home");
        }
        public ActionResult SignUp()
        {
            return View();
        }

        // Check Input Method
        public bool CheckInput(User user)
        {
            bool retval;
            if (user.FirstName == null || user.LastName == null || user.Email == null || user.Password == null)
            {
                retval = false;
            }
            else retval = true;

            return retval;
        }

        // Create Newsletter Subscription
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult NewsLetter_Action(FormCollection form)
        {
            NewsLetter email = new NewsLetter
            {
                Email = form["Email"]
            };

            entity.NewsLetters.Add(email);
            entity.SaveChanges();
            TempData["Info"] = "Thank You for Subscribing to our Newsletter!";
            return RedirectToAction("Index");
        }

        // Logout Method
        public ActionResult Logout()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            TempData["Info"] = "You have Logged Out";
            return RedirectToAction("Index", "Home");
        }
    }
}