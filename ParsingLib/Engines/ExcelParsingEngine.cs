using ExcelDataReader;
using Microsoft.Extensions.Logging;
using ParsingLib.Contract;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParsingLib.Engines
{
    internal sealed class ExcelParsingEngine : IExcelParsingEngine
    {
        #region Private Members and Constructor
        private readonly ILogger<ExcelParsingEngine> _log;

        public ExcelParsingEngine(ILogger<ExcelParsingEngine> log)
        {
            _log = log
                ?? throw new ArgumentNullException(nameof(log));
        }
        #endregion

        public async Task<ResultWrapper<ParseResult>> ParseExcelLines(ParseRequest request)
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
                var dataset = request.GetType() == typeof(ParseStreamRequest)
                    ? GetStreamDataSet((request as ParseStreamRequest).RawData)
                    : GetByteDataSet((request as ParseByteRequest).RawData);

                var pages = new List<ParsedPage>();

                //Lets take each table, as a 'page' 
                foreach(DataTable table in dataset.Tables)
                {
                    var page = new ParsedPage
                    {
                        PageName = table.TableName
                    };

                    var cols = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName.TryTrim()).Distinct().ToList();
                    var lines = new List<string>();

                    //each row will corespond to a line of text
                    foreach(var row in table.AsEnumerable().AsEnumerable())
                        lines.Add(cols.Select(c => $"'{c}' : '{row[c]}'")
                                       .Aggregate((c, n) => c + ", " + n)
                                       .TrimEnd(','));

                    page.Lines = lines.ToList();
                    _log.LogInformation($"Parsed '{page.LineCount}' Lines for Excel Workbook page named: '{page.PageName}'.");
                    pages.Add(page);
                }

                result.Data.Pages = pages;
            }
            catch(Exception ex)
            {
                result.IsFailure = true;
                result.Data = null;
                result.Message = "Exception caught in Excel parsing Engine trying to parse text.";
                result.Exception = ex;
            }

            return await Task.FromResult(result);
        }

        #region Private Methods
        /// <summary>
        /// Will convert a streamed excel file into a dataset object containing its data.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        private DataSet GetStreamDataSet(Stream rawData, bool firstRowIsHeader = true)
        {
            using (var reader = ExcelReaderFactory.CreateOpenXmlReader(rawData))
                return reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = firstRowIsHeader
                    }
                });
        }

        /// <summary>
        /// Will convert a byte array of an excel file into a dataset object containing its data.
        /// </summary>
        /// <param name="rawData"></param>
        /// <param name="firstRowIsHeader"></param>
        /// <returns></returns>
        private DataSet GetByteDataSet(byte[] rawData, bool firstRowIsHeader = true)
        {
            using (var ms = new MemoryStream(rawData))
                return GetStreamDataSet(ms, firstRowIsHeader);
        }

        #endregion
    }
}
