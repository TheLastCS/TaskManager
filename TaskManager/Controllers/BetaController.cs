using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskManager.Models;

namespace TaskManager.Controllers
{
    public class BetaController : Controller
    {
        // GET: Beta
        private readonly Task_ManagerEntities entity;
        public BetaController()
        {
            entity = new Task_ManagerEntities();
        }

        public ActionResult BetaTask()
        {
            using (entity)
            {
                return View(entity.Tasks.ToList());
            }

        }

        public ActionResult BetaDetails(int id)
        {
            using (entity)
            {
                return View(entity.Tasks.Where(x => x.TaskID == id).FirstOrDefault());
            }
        }

        // GET: Beta/Create
        public ActionResult BetaAdd()
        {
            return View();
        }

        // POST: /Beta/Create
        [HttpPost]
        public ActionResult BetaAdd(Task task)
        {
            task.Deadline = DateTime.Now;
            try
            {
                using (entity)
                {
                    entity.Tasks.Add(task);
                    entity.SaveChanges();
                }

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }

        }

        // GET: Beta/Delete/5
        public ActionResult BetaDelete(int id)
        {
            return View();
        }

        // POST: Beta/Delete/5
        [HttpPost]
        public ActionResult BetaDelete(int id, Task task)
        {
            try
            {
                using(entity)
                {
                    entity.Entry(task).State = EntityState.Modified;
                }
                return RedirectToAction("BetaTask");
            }
            catch
            {
                return View();
            }
        }

        // GET: Beta/Edit/5
        public ActionResult BetaEdit(int id)
        {
            return View();

        }

        // POST: Beta/Edit/5
        [HttpPost]
        public ActionResult BetaEdit(int id, Task task)
        {
            task.Deadline = DateTime.Now;
            try
            {
                using (Task_ManagerEntities entity = new Task_ManagerEntities())
                {
                    entity.Entry(task).State = EntityState.Modified;
                    entity.SaveChanges();
                }
                return RedirectToAction("BetaTask");
            }
            catch
            {
                return View();
            }

        }
    }
}
