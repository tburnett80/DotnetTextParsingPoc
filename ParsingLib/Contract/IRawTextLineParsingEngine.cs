using System.Threading.Tasks;

namespace ParsingLib.Contract
{
    public interface IRawTextLineParsingEngine
    {
        Task<ResultWrapper<ParseResult>> ParseRawTextLines(ParseRequest request);
    }
}
