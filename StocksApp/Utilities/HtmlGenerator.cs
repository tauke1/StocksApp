using Razor.Templating.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Utilities
{
    public class HtmlGenerator : IHtmlGenerator
    {
        public Task<string> GenerateHtml(string templatePath, object model)
        {
            return RazorTemplateEngine.RenderAsync(templatePath, model);
        }
    }
}
