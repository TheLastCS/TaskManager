using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly Task_ManagerEntities entity;

        public HomeController()
        {
            entity = new Task_ManagerEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult About()
        {

            return View();
        }
        public ActionResult Contact()
        {

            return View();
        }
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

    }
}