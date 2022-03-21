using System.Threading.Tasks;

namespace ParsingLib.Contract
{
    public interface IWordDocTextParsingEngine
    {
        Task<ResultWrapper<ParseResult>> ParseDocTextLines(ParseRequest request);
    }
}
