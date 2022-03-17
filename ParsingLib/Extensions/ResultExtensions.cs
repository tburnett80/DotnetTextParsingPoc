using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingLib
{
    internal static class ResultExtensions
    {
        /// <summary>
        /// Extension to add a new page to existing result and returns the new page that was created.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        internal static ParsedPage AddNewPage(this ParseResult result, IEnumerable<string> lines, string pageNameOverride = null)
        {
            //ensure its not default.
            if(!result.Pages?.Any() ?? true)
                result.Pages = new List<ParsedPage>();

            //If we do not have a page name specified we default to the file name
            var pageName = pageNameOverride.HasValue()
                ? pageNameOverride.TryTrim()
                : result.FileName.TryTrim();

            var page = new ParsedPage
            {
                Lines = lines,
                PageName = pageName
            };

            //add the new page to the collection
            result.Pages = result.Pages.Concat(new[] { page });
            return page;
        }
    }
}
