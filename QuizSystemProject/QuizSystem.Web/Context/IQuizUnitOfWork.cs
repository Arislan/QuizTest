﻿using QuizSystem.Data;
using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Context
{
    public interface IQuizUnitOfWork
    {
        IRepository<QuizUser> Users { get; }
        IRepository<Category> Categories { get; }
        IRepository<Quiz> Quizzes { get; }
        IRepository<Question> Questions { get; }
        IRepository<Answer> Answers { get; }
        IRepository<Vote> Votes { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Score> Scores { get; }
        int SaveChanges();
        void Dispose();
    }
}