using System;
using System.Collections.Generic;
using System.Linq;

namespace ParsingLib
{
    public sealed class ParseResult
    {
        public string FileName { get; set; }

        public int PageCount 
            => Pages?.Count() ?? 0;

        public int TotalLineCount
            => Pages?.Select(p => p.LineCount)?.Sum() ?? 0;

        public IEnumerable<ParsedPage> Pages { get; set; } = Array.Empty<ParsedPage>();
    }

    public sealed class ParsedPage
    {
        public int LineCount
            => Lines?.Count() ?? 0;

        public string PageName { get; set; }

        public IEnumerable<string> Lines { get; set; } = Array.Empty<string>();
    }
}
