//file with functions to handle input of .txt, .docx, .doc, .pdf files and web links.
//it should texte the file /link and return the text in a string

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using HtmlAgilityPack;
using Xceed.Words.NET;
using System.Text.RegularExpressions;

public class TextProcessor
{
    // This method extracts text from either a file or a URL.
    public string ExtractTextFromFileOrUrl(FileOrUrl source)
    {
        if (source.IsUrl)
        {
            // If the source is a URL, extract text from the URL.
            return ExtractTextFromUrl(source.PathOrUrl).Result;
        }
        else
        {
            // If the source is a local file, determine the file type and extract text accordingly.
            switch (source.FileType)
            {
                case FileType.Txt:
                    return ExtractTextFromTxt(source.PathOrUrl);
                case FileType.Pdf:
                    return ExtractTextFromPdf(source.PathOrUrl);
                case FileType.Docx:
                    return ExtractTextFromDocx(source.PathOrUrl);
                default:
                    return "Unsupported file type.";
            }
        }
    }

    // This method asynchronously extracts text from a web page given its URL.
    public async Task<string> ExtractTextFromUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string html = await response.Content.ReadAsStringAsync();
                var doc = new HtmlDocument();
                doc.LoadHtml(html);
                // Extract the text from the HTML document.
                string extractedText = doc.DocumentNode.InnerText;
                extractedText = ExtractUsefulText(extractedText);
                return extractedText;
            }
            else
            {
                return "Failed to retrieve the web page.";
            }
        }
    }

    // This method extracts text from a local text file.
    public string ExtractTextFromTxt(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Read the text content from the local file.
            string extractedText = File.ReadAllText(filePath);
            extractedText = ExtractUsefulText(extractedText);
            return extractedText;
        }
        else
        {
            return "File not found.";
        }
    }

    // This method extracts text from a PDF file.
    public static string ExtractTextFromPdf(string pdfFilePath)
    {
        string text = "";

        PdfDocument pdfDoc = new PdfDocument(new iText.Kernel.Pdf.PdfReader(pdfFilePath));

        for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
        {
            PdfPage page = pdfDoc.GetPage(pageNum);
            LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
            new PdfCanvasProcessor(strategy).ProcessPageContent(page);

            // Append the text extracted from this page to the result.
            text += strategy.GetResultantText();
        }

        pdfDoc.Close();

        string extractedText = ExtractUsefulText(text);
        return extractedText;
    }

    // This method extracts text from a local Word document (DocX).
    public string ExtractTextFromDocx(string filePath)
    {
        if (File.Exists(filePath))
        {
            DocX doc = DocX.Load(filePath);
            // Extract text from the DocX document.
            string extractedText = doc.Text;
            extractedText = ExtractUsefulText(extractedText);
            return extractedText;
        }
        else
        {
            return "File not found.";
        }
    }

    public static string ExtractUsefulText(string inputText)
    {
        // Remove line breaks, extra spaces, \n, and \r
        inputText = Regex.Replace(inputText, @"[\n\r\t]+", " ");

        // Remove content that doesn't contain letters or numbers
        inputText = Regex.Replace(inputText, @"[^\p{L}\p{N}\s]+", string.Empty);

        // Trim any leading or trailing spaces
        inputText = inputText.Trim();

        return inputText;
    }

}


// An enum to represent supported file types.
public enum FileType
{
    Txt,   // Text file
    Pdf,   // PDF file
    Docx   // Word document file (DocX)
}

// A class to represent a file or URL source.
public class FileOrUrl
{
    public bool IsUrl { get; private set; }   // True if it's a URL, false if it's a local file.
    public FileType FileType { get; private set; }  // The type of the file.
    public string PathOrUrl { get; private set; }   // The path or URL.

    public FileOrUrl(string pathOrUrl)
    {
        if (Uri.IsWellFormedUriString(pathOrUrl, UriKind.Absolute))
        {
            // If the input is a well-formed URL, set the URL flag.
            IsUrl = true;
            PathOrUrl = pathOrUrl;
        }
        else
        {
            // If the input is not a URL, assume it's a local file and set the file type.
            IsUrl = false;
            PathOrUrl = pathOrUrl;
            FileType = GetFileType(pathOrUrl);
        }
    }

    // A private method to determine the file type based on its extension.
    private FileType GetFileType(string path)
    {
        if (path.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            return FileType.Txt;
        if (path.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return FileType.Pdf;
        if (path.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
            return FileType.Docx;
        return FileType.Txt; // Default to text if the type is not recognized.
    }
}

