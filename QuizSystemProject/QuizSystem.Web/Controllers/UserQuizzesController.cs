using QuizSystem.Model;
using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Libs.Helpers;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Text;
using System.Threading;
using QuizSystem.Web.Libs.DataPager;

namespace QuizSystem.Web.Controllers
{
    public class UserQuizzesController : BaseController
    {
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            IEnumerable<CategoryModel> categories = 
                this.context.Categories.All().Select(ModelConvertor.CategoryToViewModel);

            List<SelectListItem> categoriesListItems = new List<SelectListItem>();
            categoriesListItems.Add(new SelectListItem { Text = "Choose Category", Value = "", Selected = true});
            categoriesListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }));

            List<SelectListItem> categoriesFilterListItems = new List<SelectListItem>();
            categoriesFilterListItems.Add(new SelectListItem { Text = "All", Value = "", Selected = true });
            categoriesFilterListItems.AddRange(categories.Select(x => new SelectListItem { Value = x.Name, Text = x.Name }));

            this.ViewData.Add("categories", categoriesListItems);
            this.ViewData.Add("categoriesFilter", categoriesFilterListItems);

            string currentUser = this.User.Identity.GetUserId();

            IQueryable<QuizAuthorViewModel> quizzes = 
                this.context.Quizzes.All().Where(x => x.CreatorId == currentUser)
                .Select(ModelConvertor.QuizToAuthorViewModel);

            SimpleDataPager<QuizAuthorViewModel> pager = 
                new SimpleDataPager<QuizAuthorViewModel>(quizzes, 3)
                .ProcessUrlParameters(this.Request.QueryString)
                .Load();

            return View(pager);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Edit(int quizId)
        {
            Quiz editedQuiz = this.context.Quizzes.GetById(quizId);

            if (this.User.Identity.GetUserId() != editedQuiz.CreatorId && !this.User.IsInRole("Admin"))
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = "/UserQuizzes/Edit/" + quizId });
            }

            QuizUpdateModel model = ModelConvertor.QuizToUpdateModel.Compile().Invoke(editedQuiz);

            var categories = 
                this.context.Categories.All()
                .ToList()
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });

            categories.Single(x => x.Value == model.CategoryId.ToString()).Selected = true;

            this.ViewData.Add("categories", categories);

            return View(model);
        }

        //Quiz Ajax Actions

        [HttpGet]
        [ActionName("UpdateGrid")]
        [Authorize]
        [HandleAjaxError]
        public ActionResult GetQuzzesGird()
        {
            string currentUser = this.User.Identity.GetUserId();

            IQueryable<QuizAuthorViewModel> quizzes =
                this.context.Quizzes.All().Where(x => x.CreatorId == currentUser)
                .Select(ModelConvertor.QuizToAuthorViewModel);

            SimpleDataPager<QuizAuthorViewModel> pager =
                new SimpleDataPager<QuizAuthorViewModel>(quizzes, 3)
                .ProcessUrlParameters(this.Request.QueryString)
                .Load();

            return PartialView("_GridView", pager);
        }

        [HttpPost]
        [Authorize]
        [HandleAjaxError]
        public ActionResult Remove(int quizId)
        {
            Thread.Sleep(2000);
            Quiz quiz = this.context.Quizzes.GetById(quizId);
            this.ValidateAjaxRequest(quiz.CreatorId);

            this.context.Quizzes.RemoveItem(quiz);
            this.context.SaveChanges();

            return RedirectToAction("UpdateGrid");
        }

        [HttpPost]
        [Authorize]
        [HandleAjaxError]
        public ActionResult Create(QuizCreateModel model)
        {
            ValidateAjaxRequest();

            if (this.context.Quizzes.All().Any(x => x.Title == model.Title))
            {
                throw new HttpException(409, String.Format("Quiz with Title '{0}' already exists.", model.Title));
            }

            Quiz newQuiz = new Quiz
            {
                Title = model.Title,
                CategoryId = model.CategoryId,
                CreatorId = this.User.Identity.GetUserId()
            };

            this.context.Quizzes.Add(newQuiz);
            this.context.SaveChanges();

            return Content(String.Format("/UserQuizzes/Edit?quizId={0}", newQuiz.Id));
        }


        [HttpPost]
        [HandleAjaxError]
        public ActionResult Edit(QuizUpdateModel model)
        {
            Quiz editedQuiz = this.context.Quizzes.GetById(model.Id);

            this.ValidateAjaxRequest(editedQuiz.CreatorId);

            if (editedQuiz.Title != model.Title)
            {
                if (this.context.Quizzes.All().Any(x => x.Title == model.Title))
                {
                    throw new HttpException(409, String.Format("Quiz with Title '{0}' already exists.", model.Title));
                }

                editedQuiz.Title = model.Title;
            }

            editedQuiz.CategoryId = model.CategoryId;

            this.context.Quizzes.Update(editedQuiz);
            this.context.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        //Questions Actions

        [HttpGet]
        [HandleAjaxError]
        public ActionResult AllQuestions(int quizId)
        {
            Thread.Sleep(2000);
            this.ValidateAjaxRequest(quizId);

            IEnumerable<QuizEditQuestionViewModel> questions =
                this.context.Questions.All().Where(x => x.QuizId == quizId)
                .Select(ModelConvertor.QuestionToQuizEditViewModel);

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(questions));
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult CreateQuestion(int quizId, QuizEditQuestionCrudModel model)
        {
            Thread.Sleep(2000);
            this.ValidateAjaxRequest(quizId);

            Question newQuestion = new Question { Content = model.Content, QuizId = quizId };

            ProcessAnswersOnCreate(newQuestion, model);
            
            newQuestion.RightAnswerId = 
                newQuestion.Answers.Single(x => x.Content == model.Answers.Single(a => a.IsCorrect).Content).Id;

            this.context.Questions.Update(newQuestion);
            this.context.SaveChanges();

            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(
                ModelConvertor.QuestionToQuizEditViewModel.Compile().Invoke(newQuestion)));
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult UpdateQuestion(int quizId, QuizEditQuestionCrudModel model)
        {
            Thread.Sleep(2000);
            this.ValidateAjaxRequest(quizId);

            Question question = this.context.Questions.GetById(model.Id);

            if (question.Content != model.Content) { question.Content = model.Content; }

            ProcessAnswersOnUpdate(question, model);

            question.RightAnswerId =
                question.Answers.Single(x => x.Content == model.Answers.Single(a => a.IsCorrect).Content).Id;

            this.context.Questions.Update(question);
            this.context.SaveChanges();

            return Content(JsonConvert.SerializeObject(
                ModelConvertor.QuestionToQuizEditViewModel.Compile().Invoke(question)));
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult RemoveQuestion(int quizId, QuizEditQuestionCrudModel model)
        {
            Thread.Sleep(2000);
            this.ValidateAjaxRequest(quizId);

            var answersToRemove = this.context.Answers.All()
                .Where(x => x.Questions.Any(q => q.Id == model.Id) && x.Questions.Count == 1);

            foreach (var answer in answersToRemove)
            {
                this.context.Answers.RemoveItem(answer);
            }

            this.context.Questions.Remove(model.Id);
            this.context.SaveChanges();

            return new HttpStatusCodeResult(200);
        }

        [NonAction]
        private void ProcessAnswersOnUpdate(Question question, QuizEditQuestionCrudModel model)
        {
            var newAnswersValues = model.Answers.Select(v => v.Content).ToList();

            HashSet<int> answerIdsToRemove = new HashSet<int>();

            var answers = question.Answers.ToList();

            foreach (var answer in answers)
            {
                if (newAnswersValues.Any(v => v == answer.Content))
                {
                    newAnswersValues.Remove(answer.Content);
                }
                else
                {
                    question.Answers.Remove(answer);
                    answerIdsToRemove.Add(answer.Id);
                }
            }

            var answersToRemove =
                this.context.Answers.All().Where(x => answerIdsToRemove.Any(a => a == x.Id) && x.Questions.Count == 1);

            foreach (var answer in answersToRemove) { this.context.Answers.RemoveItem(answer); }

            if (newAnswersValues.Count > 0)
            {
                var existingAnswers = 
                    this.context.Answers.All().Where(x => newAnswersValues.Any(v => v == x.Content)).ToList();

                foreach(var answer in existingAnswers)
                {
                    question.Answers.Add(answer);
                    newAnswersValues.Remove(answer.Content);
                }

                if (newAnswersValues.Count > 0)
                {
                    var answersToCreate = newAnswersValues.Select(x => new Answer { Content = x });

                    foreach (var answer in answersToCreate ){ question.Answers.Add(answer); }
                }
            }

            this.context.SaveChanges();
        }

        [NonAction]
        private void ProcessAnswersOnCreate(Question question, QuizEditQuestionCrudModel model)
        {
            var answersValues = model.Answers.Select(x => x.Content).ToList();

            var processedAnswers = this.context.Answers.All()
                .Where(x => answersValues.Any(v => v == x.Content))
                .ToList();

            processedAnswers.AddRange(model.Answers
                 .Where(x => !processedAnswers.Any(a => a.Content == x.Content))
                 .Select(x => new Answer { Content = x.Content }));

            this.context.Questions.Add(question);

            foreach (var answer in processedAnswers)
            {
                question.Answers.Add(answer);
            }

            this.context.SaveChanges();
        }

        [NonAction]
        private void ValidateAjaxRequest(int quizId)
        {
            string creatorId = this.context.Quizzes.GetById(quizId).CreatorId;

            this.ValidateAjaxRequest(creatorId);
        }


        [NonAction]
        private void ValidateAjaxRequest(string creatorId)
        {
            if (this.User.Identity.GetUserId() != creatorId && !this.User.IsInRole("Admin"))
            {
                throw new HttpException(400, "You are not the autor of this quiz.");
            }

            this.ValidateAjaxRequest();
        }

        [NonAction]
        private void ValidateAjaxRequest()
        {
            if (!Request.IsAjaxRequest())
            {
                throw new HttpException(400, "Operation Not Allowed.");
            }

            if (!this.ModelState.IsValid)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Validation Error(s) has occured.");
                foreach (var value in this.ModelState.Values)
                {
                    foreach(var error in value.Errors)
                    {
                        sb.AppendFormat(" {0}", error.ErrorMessage);
                    }
                }

                throw new HttpException(400, sb.ToString());
            }
        }
	}
}