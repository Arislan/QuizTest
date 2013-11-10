using QuizSystem.Model;
using QuizSystem.Web.Areas.Administration.Models;
using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var mainCategories =
                this.context.Categories.All()
                .Select(x => new CategoryAdminModel { Id = x.Id, Name = x.Name, QuizzesCount = x.Quizzes.Count })
                .ToList();

            return View(mainCategories);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult Update(CategoryModel category)
        {
            if (!User.IsInRole("Admin"))
            {
                throw new HttpException(404, "ERROR");
            }

            if (this.ModelState.IsValid)
            {
                Category newCategory;

                if (category.Id <= 0)
                {
                    newCategory = this.context.Categories.Add(new Category { Name = category.Name });
                }
                else
                {
                    newCategory = this.context.Categories.Update(new Category { Id = category.Id, Name = category.Name });
                }

                this.context.SaveChanges();

                return PartialView("_EditCategory", new CategoryModel { Id = newCategory.Id, Name = newCategory.Name });
            }
            else
            {
                throw new HttpException(404, "Category is invalid. Opration FAILED !");
            }
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult Remove(CategoryModel model)
        {
            if (!User.IsInRole("Admin"))
            {
                throw new HttpException(404, "ERROR");
            }

            Thread.Sleep(3000);
            this.context.Categories.Remove(model.Id);
            int result = this.context.SaveChanges();

            if (result == 0)
            {
                throw new HttpException("Operation FAILED !");
            }

            return new HttpStatusCodeResult(200);
        }
	}
}