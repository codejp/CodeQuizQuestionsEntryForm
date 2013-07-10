using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CodeQuizQuestionsEntryForm.Models
{
    public class Question
    {
        public int QuestionId { get; set; }

        public string OwnerUserId { get; set; }

        [Display(Name="問題本文"), Required]
        public string Body { get; set; }

        [Display(Name = "回答選択肢1"), Required]
        public string Option1 { get; set; }
        [Display(Name = "回答選択肢2"), Required]
        public string Option2 { get; set; }
        [Display(Name = "回答選択肢3")]
        public string Option3 { get; set; }
        [Display(Name = "回答選択肢4")]
        public string Option4 { get; set; }
        [Display(Name = "回答選択肢5")]
        public string Option5 { get; set; }
        [Display(Name = "回答選択肢6")]
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

        [Display(Name = "解説")]
        public string Comment { get; set; }

        //public string Category { get; set; }

        [Display(Name = "投稿日時")]
        public DateTime CreateAt { get; set; }

        public Question()
        {
            this.CreateAt = DateTime.UtcNow;
        }
    }
}