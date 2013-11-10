using QuizSystem.Data;
using QuizSystem.Web.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizSystem.Web.Areas.Administration.Controllers
{
    public class BaseController : Controller
    {
        protected QuizUnitOfWork context;

        public BaseController()
        {
            this.context = new QuizUnitOfWork(new QuizContext());
        }
    }
}