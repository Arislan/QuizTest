using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Areas.Administration.Models
{
    public class CategoryAdminModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int QuizzesCount { get; set; }
    }
}