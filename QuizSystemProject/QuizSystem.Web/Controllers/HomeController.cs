using QuizSystem.Model;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace QuizSystem.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.id = 1;
            return View();
        }


        public ActionResult About(QuizCreateModel model)
        {
            if (this.ModelState.IsValid)
            {
                //var q = this.context.Quizzes.Add(new Quiz { Title = model.Title, CreatorId = this.User.Identity.GetUserId()});

                var q = this.context.Quizzes.GetById(1);
                q.Title = model.Title;
                this.context.SaveChanges();
                return RedirectToAction("Index");
            }

            return Content("1");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}