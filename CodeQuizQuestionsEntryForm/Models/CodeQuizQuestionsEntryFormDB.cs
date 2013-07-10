﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CodeQuizQuestionsEntryForm.Models
{
    public class CodeQuizQuestionsEntryFormDB : DbContext
    {
        public CodeQuizQuestionsEntryFormDB()
            : base("DefaultConnection")
        {
        }

        public DbSet<Question> Questions { get; set; }
    }
}