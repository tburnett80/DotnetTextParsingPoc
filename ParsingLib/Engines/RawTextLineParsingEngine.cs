using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ParsingLib.Engines
{
    internal sealed class RawTextLineParsingEngine : IRawTextLineParsingEngine
    {
        #region Private Members and Consutructor
        private readonly ILogger<RawTextLineParsingEngine> _log;

        public RawTextLineParsingEngine(ILogger<RawTextLineParsingEngine> log)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));
        }
        #endregion

        /// <summary>
        /// Will Parse raw text into lines of strings with some meta data as a ParseResult object.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<ResultWrapper<ParseResult>> ParseRawTextLines(ParseRequest request)
        {
            var result = new ResultWrapper<ParseResult> 
            {
                Data = new ParseResult
                {
                    FileName = request.FileName.TryTrim()
                }
            };

            try
            {
                //Lets add a new page with the contents of the text file
                var lines = request.GetType() == typeof(ParseStreamRequest)
                    ? await ParseTextStream((request as ParseStreamRequest).RawData)
                    : await ParseTextBytes((request as ParseByteRequest).RawData);

                //Add the lines as a new page, this will be the only page since its a single text file.
                var page = result.Data.AddNewPage(lines);
                _log.LogInformation($"Parsed '{page.LineCount}' Lines for page named: '{page.PageName}'.");
            }
            catch(Exception ex)
            {
                result.IsFailure = true;
                result.Data = null;
                result.Message = "Exception caught in Text parsing Engine trying to parse raw text.";
                result.Exception = ex;
            }

            return result;
        }

        #region Private Methods
        /// <summary>
        /// Read the stream and detect byte order marks to handle ASCII / Unicode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> ParseTextStream(Stream data)
        {
            _log.LogInformation("Parsing text stream into lines...");
            using (var reader = new StreamReader(data, true))
            {
                var str = await reader.ReadToEndAsync();
                return str.Split('\n');
            }
        }

        /// <summary>
        /// Read the bytes as a stream, parse text from it.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IEnumerable<string>> ParseTextBytes(byte[] data)
        {
            _log.LogInformation("Converting Byte data to stream for parsing...");
            using (var ms = new MemoryStream(data))
                return await ParseTextStream(ms);
        }
        #endregion
    }
}
