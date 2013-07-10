using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeQuizQuestionsEntryForm.Code
{
    public static class Helper
    {
        public static IHtmlString FormatToHtml(this HtmlHelper helper, string text)
        {
            return helper.Raw(HttpUtility.HtmlEncode(text ?? "").Replace("\n", "<br />"));
        }
    }
}