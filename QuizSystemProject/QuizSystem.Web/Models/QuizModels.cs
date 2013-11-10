using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizSystem.Web.Models
{
    public class QuizCreateModel
    {
        [Required(ErrorMessage="Quiz must have category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage="Quiz title is required.")]
        [MaxLength(100, ErrorMessage="Quiz title can be 100 characters max.")]
        [MinLength(5,ErrorMessage="Quiz title can be 5 characters min.")]
        public string Title { get; set; }
    }

    public class QuizUpdateModel : QuizCreateModel
    {
        public int Id { get; set; }
    }

    public class QuizAuthorViewModel
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public string Title { get; set; }

        public int Questions { get; set; }
    }
}