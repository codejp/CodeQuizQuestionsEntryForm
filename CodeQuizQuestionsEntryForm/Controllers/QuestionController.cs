using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeQuizQuestionsEntryForm.Models;
using Microsoft.AspNet.Identity;

namespace CodeQuizQuestionsEntryForm.Controllers
{
    [Authorize]
    public class QuestionController : Controller
    {
        public CodeQuizQuestionsEntryFormDB DB { get; set; }

        public QuestionController()
        {
            this.DB = new CodeQuizQuestionsEntryFormDB();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var questins = this.DB.Questions
                .Where(q => q.OwnerUserId == userId)
                .OrderBy(q => q.CreateAt)
                .ToArray();
            return View(questins);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new Question());
        }

        [HttpPost]
        public ActionResult Create(Question model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }

            model.OwnerUserId = User.Identity.GetUserId();
            model.CreateAt = DateTime.UtcNow;
            this.DB.Questions.Add(model);
            this.DB.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var question = this.DB.Questions.Find(id);
            if (question.OwnerUserId != User.Identity.GetUserId())
                throw new Exception("Access Violation.");

            return View(question);
        }

        [HttpPost]
        public ActionResult Edit(int id, Question model)
        {
            var question = this.DB.Questions.Find(id);
            if (question.OwnerUserId != User.Identity.GetUserId())
                throw new Exception("Access Violation.");

            if (ModelState.IsValid == false)
            {
                return View(model);
            }

            UpdateModel(question, 
                prefix: null, 
                includeProperties: null,
                excludeProperties: new[] { "QuestionId", "OwnerUserId", "CreateAt" });
            this.DB.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var question = this.DB.Questions.Find(id);
            if (question.OwnerUserId != User.Identity.GetUserId())
                throw new Exception("Access Violation.");

            return View(question);
        }

        [HttpPost]
        public ActionResult Delete(int id, FormCollection _)
        {
            var question = this.DB.Questions.Find(id);
            if (question.OwnerUserId != User.Identity.GetUserId())
                throw new Exception("Access Violation.");

            this.DB.Questions.Remove(question);
            this.DB.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}