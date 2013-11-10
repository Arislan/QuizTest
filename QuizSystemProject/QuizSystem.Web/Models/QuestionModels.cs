using Newtonsoft.Json;
using QuizSystem.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using QuizSystem.Web.Extensions.ValidationAttributes;

namespace QuizSystem.Web.Models
{
    public class QuizEditQuestionCrudModel
    {
        [JsonPropertyAttribute("id")]
        public int Id { get; set; }

        [Required(ErrorMessage="Question must have content.")]
        [MaxLength(300, ErrorMessage="Question content must be 300 characters max.")]
        [RegularExpression(".*\\w+.*", ErrorMessage="Question must not be only white space.")]
        [JsonPropertyAttribute("content")]
        public string Content { get; set; }

        [CollectionLength(2, ErrorMessage="Question must have atleast 2 answers.")]
        [JsonPropertyAttribute("answers")]
        public IEnumerable<QuizEditAnswerCrudModel> Answers { get; set; }
    }

    public class QuizEditQuestionViewModel
    {
        [JsonPropertyAttribute("id")]
        public int Id { get; set; }

        [JsonPropertyAttribute("rightId")]
        public int RightAnswerId { get; set; }

        [JsonPropertyAttribute("content")]
        public string Content { get; set; }

        [JsonPropertyAttribute("answers")]
        public IEnumerable<QuizEditAnswerViewModel> Answers { get; set; }
    }

    public class QuestionModel
    {
        public int Id { get; set; }
        public List<Answer> Answers { get; set; }

        public Answer Answer { get; set; }
    }
}