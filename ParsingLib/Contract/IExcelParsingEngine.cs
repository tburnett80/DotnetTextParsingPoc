using System.Threading.Tasks;

namespace ParsingLib.Contract
{
    public interface IExcelParsingEngine
    {
        Task<ResultWrapper<ParseResult>> ParseExcelLines(ParseRequest request);
    }
}
