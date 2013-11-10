using QuizSystem.Model;
using QuizSystem.Web.Areas.Administration.Models;
using QuizSystem.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace QuizSystem.Web.Libs.Helpers
{
    public static class ModelConvertor
    {
        public static Expression<Func<Category, CategoryModel>> CategoryToViewModel
        {
            get
            {
                return x =>
                    new CategoryModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    };
            }
        }

        public static  Expression<Func<Quiz, QuizUpdateModel>> QuizToUpdateModel
        {
            get
            {
                return x =>
                    new QuizUpdateModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        CategoryId = x.CategoryId
                    };
            }
        }

        public static Expression<Func<Quiz, QuizAuthorViewModel>> QuizToAuthorViewModel
        {
            get
            {
                return x =>
                    new QuizAuthorViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Category = x.Category.Name,
                        Questions = x.Questions.Count
                    };
            }
        }

        public static Expression<Func<Quiz, QuizAdminViewModel>> QuizToAdminViewModel
        {
            get
            {
                return x =>
                    new QuizAdminViewModel
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Category = x.Category.Name,
                        Questions = x.Questions.Count,
                        Creator = x.Creator.UserName
                    };
            }
        }

        public static Expression<Func<Question, QuizEditQuestionViewModel>> QuestionToQuizEditViewModel
        {
            get
            {
                return x =>
                    new QuizEditQuestionViewModel
                    {
                        Id = x.Id,
                        Content = x.Content,
                        RightAnswerId = x.RightAnswerId,
                        Answers = x.Answers.Select(a => new QuizEditAnswerViewModel { Id = a.Id, Content = a.Content })
                    };
            }
        }

        public static Expression<Func<Answer, AnswerEditModel>> AnswerToEditModel
        {
            get
            {
                return x =>
                    new AnswerEditModel
                    {
                       Answer = x,
                       QuestionsCount = x.Questions.Count
                    };
            }
        }
    }
}