using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Libs.DataPager;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QuizSystem.Web.Libs.Helpers;
using QuizSystem.Web.Areas.Administration.Models;


namespace QuizSystem.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class QuizzesController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            IEnumerable<CategoryModel> categories =
                this.context.Categories.All().Select(ModelConvertor.CategoryToViewModel);

            List<SelectListItem> categoriesListItems = new List<SelectListItem>();
            categoriesListItems.Add(new SelectListItem { Text = "Choose Category", Value = "", Selected = true });
            categoriesListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));

            List<SelectListItem> categoriesFilterListItems = new List<SelectListItem>();
            categoriesFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            categoriesFilterListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }));

            this.ViewData.Add("categories", categoriesListItems);
            this.ViewData.Add("categoriesFilter", categoriesFilterListItems);

            IQueryable<QuizAdminViewModel> quizzes =
                this.context.Quizzes.All()
                .Select(ModelConvertor.QuizToAdminViewModel);

            var pager =
                new SimpleDataPager<QuizAdminViewModel>(quizzes, 3)
                .ProcessUrlParameters(this.Request.QueryString)
                .Load();

            return View(pager);
        }

        [HttpPost]
        public ActionResult Remove(int quizId)
        {
            if (this.User.IsInRole("Admin"))
            {
                this.context.Quizzes.Remove(quizId);
                this.context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
	}
}