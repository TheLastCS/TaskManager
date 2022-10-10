using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class TaskController : Controller
    {
        // GET: Task
        private readonly Task_ManagerEntities entity;
        public TaskController()
        {
            entity = new Task_ManagerEntities();
        }

        // Task Create Methods
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult AddTask_Action(FormCollection form)
        {
            Task task = new Task
            {
                Name = form["Name"],
                Description = form["Description"],
                Deadline = Convert.ToDateTime(form["Deadline"]),
                Status = form["Status"],
                UserID = (long)Session["UserId"],
            };

            if (CheckInput(task) != false)
            {
                entity.Tasks.Add(task);
                entity.SaveChanges();
            }
            TempData["Success"] = "Task Successfully Created";
            if (Session["UserType"].ToString().Contains("Admin"))
            {

                return RedirectToAction("AdminTasks");
            }

            return RedirectToAction("UserTasks");
        }
        public ActionResult AddTask() 
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

        // Task Delete Methods
        public ActionResult DeleteTask_Action(int TaskID)
        {
            Task taskToDelete = this.entity.Tasks.FirstOrDefault(x => x.TaskID == TaskID);
            
            entity.Tasks.Remove(taskToDelete);
            entity.SaveChanges();
            TempData["Info"] = "Task Has Been Deleted & Added To Recycle Bin";
            if (Session["UserType"].ToString().Contains("Admin"))
            {
                return RedirectToAction("AdminTasks");
            } 
            return RedirectToAction("UserTasks");
            
        }

        // Task Edit Methods
        public ActionResult EditTask(int id)
        {
            Task taskToEdit = this.entity.Tasks.FirstOrDefault(x => x.TaskID == id);
            TempData["Name"] = taskToEdit.Name;
            TempData["Description"] = taskToEdit.Description;
            TempData["Deadline"] = taskToEdit.Deadline;
            TempData["Status"] = taskToEdit.Status;
            TempData["UserID"] = taskToEdit.UserID;
            TempData["TaskID"] = taskToEdit.TaskID;

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
        public ActionResult EditTask_Action(FormCollection form)
        {
            var TaskID = Convert.ToInt64(form["TaskID"]);
            Task taskToEdit = this.entity.Tasks.FirstOrDefault(x => x.TaskID == TaskID);

            taskToEdit.Name = form["FirstName"];
            taskToEdit.Description = form["LastName"];
            taskToEdit.Deadline = Convert.ToDateTime(form["Deadline"]);
            taskToEdit.Status = form["Password"];
            taskToEdit.UserID = Convert.ToInt64(form["UserID"]);
            taskToEdit.TaskID = TaskID;

            if (CheckInput(taskToEdit) != false)
            {
                entity.Tasks.Where(x => x.TaskID == TaskID).FirstOrDefault();
                entity.SaveChanges();
            }

            if (Session["UserType"].ToString().Contains("Admin"))
            {
                return RedirectToAction("AdminTasks");
            }
            return RedirectToAction("UserTasks");
            
        }

       
        // Check Input Method
        public bool CheckInput(Task task)
        {
            bool retval;
            if(task.Name == null || task.Description == null || task.Status == null)
            {
                retval = false;
            } else retval = true;


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

        // Admin Display
        public ActionResult AdminFinishedTasks()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Tasks.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult AdminTasks()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Tasks.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        // User Display
        public ActionResult UserFinishedTasks()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Tasks.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult UserTasks()
        {
            if (Session["UserID"] != null)
            {
                return View(entity.Tasks.ToList());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // Activate Task
        public ActionResult ActivateTask_Action(int id)
        {
            Task taskToEdit = this.entity.Tasks.FirstOrDefault(x => x.TaskID == id);

            taskToEdit.Status = "To Be Done";

            entity.SaveChanges();
            TempData["Info"] = "Task Has Activated!";
            if (Session["UserType"].ToString().Contains("Admin"))
            {
                return RedirectToAction("AdminTasks");
            }
            return RedirectToAction("UserTasks");
            
        }

        // Deactivate Task
        public ActionResult FinishTask_Action(int id)
        {
            Task taskToEdit = this.entity.Tasks.FirstOrDefault(x => x.TaskID == id);

            taskToEdit.Status = "Finished";

            entity.SaveChanges();
            
            TempData["Info"] = "Task Accomplished!";
            if (Session["UserType"].ToString().Contains("Admin"))
            {
                return RedirectToAction("AdminTasks");
            }
            return RedirectToAction("UserTasks");
            
        }

        public ActionResult TaskDetails(int id)
        {
            using (entity)
            {
                return View(entity.Tasks.Where(x => x.TaskID == id).FirstOrDefault());
            }
                
        }

        public ActionResult RecycleBin(int id)
        {
            Task taskToDelete = this.entity.Tasks.Where(x => x.TaskID == id).FirstOrDefault();

            RecycleBin bin = new RecycleBin
            {
                TaskID = taskToDelete.TaskID,
                UserID = taskToDelete.UserID,
                Name = taskToDelete.Name,
                Description = taskToDelete.Description,
                Deadline = taskToDelete.Deadline,
            };

            entity.RecycleBins.Add(bin);
            entity.SaveChanges();

            return RedirectToAction("DeleteTask_Action", new { TaskID = taskToDelete.TaskID });
        }
       
        public ActionResult RecycledTasks()
        {
            if (Session["UserID"] != null)
            {  
                if(Session["UserType"].ToString().Contains("Admin"))
                {
                    return View(entity.RecycleBins.ToList());
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult UserProfile(int id)
        {
            return View(entity.Users.Where(x => x.UserID == id).FirstOrDefault());
        }

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

            entity.Users.Where(x => x.UserID == UserID).FirstOrDefault();
            entity.SaveChanges();

            return RedirectToAction("Users", "User");
        }
    }
}