using DocumentFormat.OpenXml.Packaging;
using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib.Engines
{
    internal sealed class WordDocTextParsingEngine : IWordDocTextParsingEngine
    {
        #region Private Members and Constructor
        private readonly ILogger<WordDocTextParsingEngine> _log;

        public WordDocTextParsingEngine(ILogger<WordDocTextParsingEngine> log)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));
        }
        #endregion

        public async Task<ResultWrapper<ParseResult>> ParseDocTextLines(ParseRequest request)
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
                //The word file as a DataSet
                result.Data.Pages = request.GetType() == typeof(ParseStreamRequest)
                    ? await ParseDocStream((request as ParseStreamRequest).RawData)
                    : await ParseDocBytes((request as ParseByteRequest).RawData);
            }
            catch(Exception ex)
            {
                result.IsFailure = true;
                result.Data = null;
                result.Message = "Exception caught in Word Doc parsing Engine trying to parse text.";
                result.Exception = ex;
            }

            return await Task.FromResult(result);
        }

        #region Private Methods
        /// <summary>
        /// Read the stream and detect byte order marks to handle ASCII / Unicode
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ParsedPage>> ParseDocStream(Stream data)
        {
            _log.LogInformation("Parsing text stream into lines...");
            var parsed = new List<ParsedPage>();
            using (var doc = WordprocessingDocument.Open(data, false))
            {
                //TODO: Parse out pages

                //foreach (var pdfPage in pdf.GetPages())
                //    parsed.Add(new ParsedPage
                //    {
                //        PageName = $"Page {pdfPage.Number}",
                //        Lines = pdfPage.Text.Split('\n')

                //    });
            }

            return await Task.FromResult(parsed);
        }

        /// <summary>
        /// Read the bytes as a stream, parse text from it.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ParsedPage>> ParseDocBytes(byte[] data)
        {
            _log.LogInformation("Converting Byte data to stream for parsing...");
            using (var ms = new MemoryStream(data))
                return await ParseDocStream(ms);
        }
        #endregion
    }
}
