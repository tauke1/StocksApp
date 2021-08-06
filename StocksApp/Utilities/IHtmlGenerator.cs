using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Utilities
{
    public interface IHtmlGenerator
    {
        Task<string> GenerateHtml(string templatePath, object model);
    }
}
