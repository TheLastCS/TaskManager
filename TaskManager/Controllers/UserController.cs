using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Security;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private readonly Task_ManagerEntities entity;
        public UserController()
        {
            entity = new Task_ManagerEntities();
        }

        // User Display
        public ActionResult Users()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Users.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // User Create Methods
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddUser_Action(FormCollection form)
        {
            User user = new User
            {
                FirstName = form["FirstName"],
                LastName = form["LastName"],
                Email = form["Email"],
                Password = form["Password"],
                UserType = form["UserType"],
                TimeCreated = DateTime.Now,
                Status = "Active"
            };

            entity.Users.Add(user);
            entity.SaveChanges();

            return RedirectToAction("AddUser");
        }
        public ActionResult AddUser()
        {
            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // User Delete Methods
        public ActionResult DeactivatedUsers()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Users.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult SoftDeleteUser(int id)
        {
            User userToDelete = this.entity.Users.FirstOrDefault(x => x.UserID == id);

            userToDelete.Status = "Deleted";

            entity.SaveChanges();

            return RedirectToAction("Users");
        }
        public ActionResult ActivateUser(int id)
        {
            User userToDelete = this.entity.Users.FirstOrDefault(x => x.UserID == id);

            userToDelete.Status = "Active";

            entity.SaveChanges();

            return RedirectToAction("Users");
        }
        public ActionResult DeleteUser_Action(int id)
        {
            User userToDelete = this.entity.Users.FirstOrDefault(x => x.UserID == id);

            entity.Users.Remove(userToDelete);
            entity.SaveChanges();

            return RedirectToAction("DeletedUsers");
        }

        // User Edit Methods
        public ActionResult EditUser(int id)
        {
            User userToEdit = this.entity.Users.FirstOrDefault(x => x.UserID == id);
            TempData["FirstName"] = userToEdit.FirstName;
            TempData["LastName"] = userToEdit.LastName;
            TempData["Email"] = userToEdit.Email;
            TempData["Password"] = userToEdit.Password;
            TempData["UserType"] = userToEdit.UserType;
            TempData["Status"] = userToEdit.Status;
            TempData["UserID"] = userToEdit.UserID;

            if (Session["UserID"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult EditUser_Action(FormCollection form)
        {
            var UserID = Convert.ToInt64(form["UserID"]);
            User userToEdit = this.entity.Users.FirstOrDefault(x => x.UserID == UserID);

            userToEdit.FirstName = form["FirstName"];
            userToEdit.LastName = form["LastName"];
            userToEdit.Email = form["Email"];
            userToEdit.Password = form["Password"];
            userToEdit.UserType = form["UserType"];
            userToEdit.TimeCreated = DateTime.Now;

            if (CheckInput(userToEdit) != false)
            {
                entity.Users.Where(x => x.UserID == UserID).FirstOrDefault();
                entity.SaveChanges();
            }

            if (Session["UserType"].ToString().Contains("Admin"))
            {
                return RedirectToAction("Users", "User");
            }
            else
            {
                return RedirectToAction("UserTasks","Task");
            }
            
        }

        // Check Input Method
        public bool CheckInput(User user)
        {
            bool retval;
            if (user.FirstName == null || user.LastName == null || user.Email == null || user.Password == null || user.UserType == null)
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

        public ActionResult UserDetails(int id)
        {
            using (entity)
            {
                return View(entity.Users.Where(x => x.UserID == id).FirstOrDefault());
            }
        }

        
    }
}