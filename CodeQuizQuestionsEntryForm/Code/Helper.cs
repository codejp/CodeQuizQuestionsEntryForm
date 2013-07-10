using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeQuizQuestionsEntryForm.Models;
using MarkdownSharp;

namespace CodeQuizQuestionsEntryForm.Code
{
    public static class Helper
    {
        public static IHtmlString FormatToHtml(this HtmlHelper helper, string text, TextFormatType format)
        {
            switch (format)
            {
                case TextFormatType.PlainText:
                    return helper.Raw(HttpUtility.HtmlEncode(text ?? "").Replace("\n", "<br />"));
                case TextFormatType.MarkDown:
                    return helper.Raw(new Markdown(loadOptionsFromConfigFile: true).Transform(HttpUtility.HtmlEncode(text ?? "")));
                default: throw new NotImplementedException();
            }
        }
    }
}