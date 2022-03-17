using System.Threading.Tasks;

namespace ParsingLib.Contract
{
    public interface ITextParseManager
    {
        Task<ResultWrapper<ParseResult>> ParseTextFromSource(ParseRequest request);
    }
}
