using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace QuizSystem.Data
{
    public class QuizContextConfigurator : DbMigrationsConfiguration<QuizContext>
    {
        public QuizContextConfigurator()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(QuizContext context)
        {
            context.Database.ExecuteSqlCommand(
               "IF NOT EXISTS (SELECT name from sys.indexes WHERE name = N'uc_AnswerContent')  " +
               "ALTER TABLE Answers ADD CONSTRAINT uc_AnswerContent UNIQUE (Content)");
        }
    }
}
