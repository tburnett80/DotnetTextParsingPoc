using System.Threading.Tasks;

namespace ParsingLib.Contract
{
    public interface IPdfFileTextParsingEngine
    {
        Task<ResultWrapper<ParseResult>> ParsePdfText(ParseRequest request);
    }
}
