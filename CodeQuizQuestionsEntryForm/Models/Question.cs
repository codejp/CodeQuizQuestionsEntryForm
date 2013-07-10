using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeQuizQuestionsEntryForm.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string OwnerUserId { get; set; }

        [Display(Name = "問題本文"), Required, AllowHtml]
        public string Body { get; set; }

        [Display(Name = "問題本文の書式"), Required]
        public TextFormatType BodyFormat { get; set; }

        [Display(Name = "回答選択肢1"), Required, AllowHtml]
        public string Option1 { get; set; }
        [Display(Name = "回答選択肢2"), Required, AllowHtml]
        public string Option2 { get; set; }
        [Display(Name = "回答選択肢3"), AllowHtml]
        public string Option3 { get; set; }
        [Display(Name = "回答選択肢4"), AllowHtml]
        public string Option4 { get; set; }
        [Display(Name = "回答選択肢5"), AllowHtml]
        public string Option5 { get; set; }
        [Display(Name = "回答選択肢6"), AllowHtml]
        public string Option6 { get; set; }

        public string[] GetOptions()
        {
            return new[] { 
                Option1,
                Option2,
                Option3,
                Option4,
                Option5,
                Option6
            }.Where(s => string.IsNullOrEmpty(s) == false).ToArray();
        }

        [Display(Name = "正解の選択肢の番号")]
        public int IndexOfCorrectOption { get; set; }

        [Display(Name = "解説"), AllowHtml]
        public string Comment { get; set; }

        [Display(Name = "問題本文の書式"), Required]
        public TextFormatType CommentFormat { get; set; }

        //public string Category { get; set; }

        [Display(Name = "投稿日時")]
        public DateTime CreateAt { get; set; }

        public Question()
        {
            this.CreateAt = DateTime.UtcNow;
        }
    }
}