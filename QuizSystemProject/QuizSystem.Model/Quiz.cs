using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class Quiz
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        public string CreatorId { get; set; }
        public virtual QuizUser Creator { get; set; }

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<Score> Scores { get; set; }

        public Quiz()
        {
            this.Questions = new HashSet<Question>();
            this.Votes = new HashSet<Vote>();
            this.Comments = new HashSet<Comment>();
            this.Scores = new HashSet<Score>();
        }
    }
}
