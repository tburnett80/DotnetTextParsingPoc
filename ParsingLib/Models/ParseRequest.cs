using System.IO;

namespace ParsingLib
{
    public enum TextSourceType: int
    {
        RawTextLines = 0,
        ExcelFile = 1,
        PdfFile = 2
    }

    public abstract class ParseRequest
    {
        public TextSourceType SourceFormat { get; set; }

        public string MimeType { get; set; }

        public string FileExtension { get; set; }

        public string FileName { get; set; }
    }

    public class ParseStreamRequest : ParseRequest
    {
        public Stream RawData { get; set; }
    }

    public class ParseByteRequest : ParseRequest
    {
        public byte[] RawData { get; set; }
    }
}
