using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using System;
using System.Threading.Tasks;

namespace ParsingLib.Managers
{
    public sealed class TextParseManager : ITextParseManager
    {
        #region Private members and Constructor
        private readonly ILogger<TextParseManager> _log;
        private readonly IRawTextLineParsingEngine _rawTextEngine; 
        private readonly IExcelParsingEngine _excelTextEngine;
        private readonly IPdfFileTextParsingEngine _pdfTextEngine;
        private readonly IWordDocTextParsingEngine _wordTextEngine;

        public TextParseManager(ILogger<TextParseManager> log, IRawTextLineParsingEngine rawTextEngine, 
            IExcelParsingEngine excelTextEngine, IPdfFileTextParsingEngine pdfTextEngine, IWordDocTextParsingEngine wordTextEngine)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));

            _rawTextEngine = rawTextEngine
                ?? throw new ArgumentNullException(nameof(rawTextEngine));

            _excelTextEngine = excelTextEngine
                ?? throw new ArgumentNullException(nameof(excelTextEngine));

            _pdfTextEngine = pdfTextEngine
                ?? throw new ArgumentNullException(nameof(pdfTextEngine));

            _wordTextEngine = wordTextEngine
                ?? throw new ArgumentNullException(nameof(wordTextEngine));
        }
        #endregion

        /// <summary>
        /// Will parse a text request and route to the appropriate logic
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultWrapper<ParseResult>> ParseTextFromSource(ParseRequest request)
        {
            var result = new ResultWrapper<ParseResult>();

            try
            {
                _log.LogDebug("Manager hit.");

                //route to the proper engine
                switch(request.SourceFormat)
                {
                    case TextSourceType.ExcelFile:
                        _log.LogInformation("Starting Excel file parse...");
                        result = await _excelTextEngine.ParseExcelLines(request);
                        if (result.IsFailure)
                            _log.LogError("Excel Parsing failed.");
                        break;
                    case TextSourceType.PdfFile:
                        _log.LogInformation("Starting PDF file parse...");
                        result = await _pdfTextEngine.ParsePdfText(request);
                        if (result.IsFailure)
                            _log.LogError("PDF Text Parsing failed.");
                        break;
                    case TextSourceType.RawTextLines:
                        _log.LogInformation("Starting Raw Text file parse...");
                        result = await _rawTextEngine.ParseRawTextLines(request);
                        if (result.IsFailure)
                            _log.LogError("Raw Text Parsing failed.");
                        break;
                    case TextSourceType.WordDoc:
                        _log.LogInformation("Starting Word Doc file parse...");
                        result = await _wordTextEngine.ParseDocTextLines(request);
                        if (result.IsFailure)
                            _log.LogError("Word Doc Parsing failed.");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("ParseRequest.SourceFormat");
                }
            }
            catch(Exception ex)
            {
                result.IsFailure = true;
                result.Data = null;
                result.Message = "Exception caught in manager.";
                result.Exception = ex;
            }

            _log.LogInformation($"Parsing finished. Did the parse work? : '{!result.IsFailure}'");
            _log.LogInformation($"Result Message: '{result.Message}'");
            if (result.IsFailure && result.Exception != null)
            {
                _log.LogInformation($"Exception Message: '{result.Exception.Message}'");
                if (result.Exception.InnerException != null)
                    _log.LogInformation($"Inner: '{result.Exception.InnerException.Message}'");
                _log.LogInformation($"Stack: ");
                _log.LogInformation(result.Exception.StackTrace);
            }

            return await Task.FromResult(result);
        }
    }
}
