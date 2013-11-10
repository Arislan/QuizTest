using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizSystem.Model
{
    public class QuizUser : User
    {
        public virtual ICollection<Score> Scores { get; set; }

        public QuizUser() 
            : base ()
        {
            this.Scores = new HashSet<Score>();
        }

        public QuizUser(string username)
            : base(username)
        {
            this.Scores = new HashSet<Score>();
        }
    }
}
