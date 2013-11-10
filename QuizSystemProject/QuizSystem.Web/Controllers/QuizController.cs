using QuizSystem.Model;
using QuizSystem.Web.Extensions.FiltersExtensions;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Controllers
{
    public class QuizController : BaseController
    {

        [HandleAjaxError]
        public ActionResult QuestionCreate(int quizId, QuestionUpdateModel data)
        {
            var q = new Question { Content = data.content, QuizId = quizId };
            Answer ans;
            Answer right = null;
            foreach (var a in data.answers)
            {
                ans = this.context.Answers.All().FirstOrDefault(x => x.Content == a.content);
                if ( ans != null)
                {
                    q.Answers.Add(ans);
                }
                else
                {
                    q.Answers.Add(this.context.Answers.Add(new Answer { Content = a.content }));
                }

                if (a.isCorrect)
                {
                    right = ans;
                }
            }

            this.context.Questions.Add(q);

            this.context.SaveChanges();

            q.RightAnswerId = right.Id;

            this.context.SaveChanges();

            return Json(q);
        }

        [HandleAjaxError]
        public ActionResult QuestionUpdate(int quizId, QuestionUpdateModel data)
        {

            return Json(new { QUIZ = quizId, DATA = data });
        }
	}
}