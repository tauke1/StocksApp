using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocksApp.Utilities
{
    public class HtmlToPdfConverter : IHtmlToPdfConverter
    {
        public byte[] ConvertHtmlToPdf(string html) 
        {
            if (html == null)
                throw new ArgumentNullException(nameof(html));

            HtmlToPdf renderer = new HtmlToPdf();
            PdfDocument document = renderer.ConvertHtmlString(html);
            return document.Save();
        }
    }
}
