using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ParsingLib.Engines
{
    internal sealed class PdfFileTextParsingEngine : IPdfFileTextParsingEngine
    {
        #region Private Members and Constructor
        private readonly ILogger<PdfFileTextParsingEngine> _log;

        public PdfFileTextParsingEngine(ILogger<PdfFileTextParsingEngine> log)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));
        }
        #endregion

        public async Task<ResultWrapper<ParseResult>> ParsePdfText(ParseRequest request)
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
                //The excel file as a DataSet
                result.Data.Pages = request.GetType() == typeof(ParseStreamRequest)
                    ? await ParsePdfStream((request as ParseStreamRequest).RawData)
                    : await ParsePdfBytes((request as ParseByteRequest).RawData);
            }
            catch(Exception ex)
            {
                result.IsFailure = true;
                result.Data = null;
                result.Message = "Exception caught in PDF parsing Engine trying to parse text.";
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
        private async Task<IEnumerable<ParsedPage>> ParsePdfStream(Stream data)
        {
            _log.LogInformation("Parsing text stream into lines...");
            var parsed = new List<ParsedPage>();
            using (var pdf = UglyToad.PdfPig.PdfDocument.Open(data))
            {
                foreach(var pdfPage in pdf.GetPages())
                    parsed.Add(new ParsedPage
                    {
                        PageName = $"Page {pdfPage.Number}",
                        Lines = pdfPage.Text.Split('\n')

                    });
            }

            return await Task.FromResult(parsed);
        }

        /// <summary>
        /// Read the bytes as a stream, parse text from it.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private async Task<IEnumerable<ParsedPage>> ParsePdfBytes(byte[] data)
        {
            _log.LogInformation("Converting Byte data to stream for parsing...");
            using (var ms = new MemoryStream(data))
                return await ParsePdfStream(ms);
        }
        #endregion
    }
}
