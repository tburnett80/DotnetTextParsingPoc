using ParsingLib.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib.Engines
{
    internal sealed class WordDocTextParsingEngine : IWordDocTextParsingEngine
    {
        public Task<ResultWrapper<ParseResult>> ParseDocTextLines(ParseRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
