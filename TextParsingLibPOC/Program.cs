using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ParsingLib;
using ParsingLib.Contract;

namespace TextParsingLibPOC
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Bootstrap the application / configuration
            var explicitConfigVals = new Dictionary<string, string>()
            {
                { "testconfigkey1", "testconfigval1" },
                { "testconfigkey2", "testconfigval2" },
            };

            var assembly = typeof(TextParsingLibPOC.Program).GetTypeInfo().Assembly;
            var host = args.BoostrapApplication(explicitConfigVals).Build();

            //Get the manager interface handle.
            var manager = host.Services.GetRequiredService<ITextParseManager>();

            //DO WORK!!!

            Console.WriteLine("Parsing text file stream now....");
            //Example using text file stream
            var result = await manager.ParseTextFromSource(new ParseStreamRequest
            {
                SourceFormat = TextSourceType.RawTextLines,
                MimeType = "text/plain",
                FileExtension = ".txt",
                FileName = "test1.txt",
                RawData = assembly.GetManifestResourceStream("TextParsingLibPOC.test1.txt")
            });

            Console.WriteLine("Parsing Excel file stream now....");
            //Example using excel file stream
            var result2 = await manager.ParseTextFromSource(new ParseStreamRequest
            {
                SourceFormat = TextSourceType.ExcelFile,
                //MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                MimeType = "application/vnd.ms-excel",
                FileExtension = ".xlsx",
                FileName = "test.xlsx",
                RawData = assembly.GetManifestResourceStream("TextParsingLibPOC.test1.xlsx")
            });

            Console.WriteLine("Parsing Pdf file stream now....");
            //Example using pdf file stream
            var result3 = await manager.ParseTextFromSource(new ParseStreamRequest
            {
                SourceFormat = TextSourceType.PdfFile,
                MimeType = "application/pdf",
                FileExtension = ".pdf",
                FileName = "test.pdf",
                RawData = assembly.GetManifestResourceStream("TextParsingLibPOC.test1.pdf")
            });

            OutputResults(result);
            Console.WriteLine();

            OutputResults(result2);
            Console.WriteLine();

            OutputResults(result3);
            Console.WriteLine();

            Console.WriteLine();
            Console.WriteLine("Hit any key to exit.");
            Console.ReadKey();
        }

        private static void OutputResults(ResultWrapper<ParseResult> result)
        {
            //Output results:
            Console.WriteLine(result.Message);
            Console.WriteLine($"File Name:  {result.Data.FileName}");
            Console.WriteLine($"Page Count: {result.Data.PageCount}");
            Console.WriteLine($"Total Lines: {result.Data.TotalLineCount}");

            //Individual Page data
            var ndx = 1;
            foreach (var page in result.Data.Pages)
            {
                Console.WriteLine($"Page #{ndx} named: '{page.PageName}' Line Count: {page.LineCount}");
                Console.WriteLine("Content:");
                Console.WriteLine();
                foreach (var line in page.Lines)
                    Console.WriteLine(line);
                Console.WriteLine();
                ndx++;
            }
        }
    }
}
