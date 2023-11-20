//file with functions to handle input of .txt, .docx, .doc, .pdf files and web links.
//it should texte the file /link and return the text in a string

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using HtmlAgilityPack;
using Xceed.Words.NET;
using System.Text.RegularExpressions;
using System.Net;

public class TextProcessor
{
    // This method extracts text from either a file or a URL.
    public string ExtractTextFromFileOrUrl(string source)
    {
        //if the input is a copied-pasted text, return it, else extract the text from the path or the text from the url
        if (source.Length > 150)
            return source;
        else
        {
            FileOrUrl fileOrUrl = new FileOrUrl(source);
            // check if the source is a url or a path
            if (source.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("URL");
                if (source.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)){
                Console.WriteLine("TXT");
                return ExtractTextFromTxtUrl(source).Result;
            }
            else if (source.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)){
                Console.WriteLine("PDF");
                return ExtractTextFromPdfUrl(source).Result;
            }
            if (source.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)){
                Console.WriteLine("DOCX");
                return ExtractTextFromDocxUrl(source).Result;
            }
            else{
                Console.WriteLine("URL");
                return ExtractTextFromUrl(fileOrUrl.PathOrUrl).Result;
            }
            }
            else
            {
                Console.WriteLine("PATH");
                switch (fileOrUrl.FileType)
                {
                    case FileType.Txt:
                        return ExtractTextFromTxt(fileOrUrl.PathOrUrl);
                    case FileType.Pdf:
                        return ExtractTextFromPdf(fileOrUrl.PathOrUrl);
                    case FileType.Docx:
                        return ExtractTextFromDocx(fileOrUrl.PathOrUrl);
                    default:
                        return "File type not supported.";
                }
            }
            
        }    
    }

    // This method asynchronously extracts text from a web page given its URL.
    public async Task<string> ExtractTextFromUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            try
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
            catch (Exception ex)
            {
                return $"Error retrieving the web page. {ex.Message}";
            }
        }
    }

    // This method extracts text from a text file.
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
    public async Task<string> ExtractTextFromTxtUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // Read the text content from the response stream asynchronously.
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return $"Failed to retrieve the web page. Status Code: {response.StatusCode}";
            }
        }
    }
    
    // This method extracts text from a PDF file.
    public async Task<string> ExtractTextFromPdfUrl(string pdfUrl)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                byte[] pdfBytes = await client.GetByteArrayAsync(pdfUrl);
                using (MemoryStream pdfStream = new MemoryStream(pdfBytes))
                {
                    return ExtractTextFromPdfStream(pdfStream).Result;
                }
            }
            catch (Exception ex)
            {
                return $"Failed to retrieve the PDF file. Error: {ex.Message}";
            }
        }
    }
    private string ExtractTextFromPdf(string pathOrUrl)
    {        
        using (PdfReader pdfReader = new PdfReader(pathOrUrl))
        {
            using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
            {
                string text = "";
                for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
                {
                    PdfPage page = pdfDoc.GetPage(pageNum);
                    LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                    new PdfCanvasProcessor(strategy).ProcessPageContent(page);

                    // Append the text extracted from this page to the result.
                    text += strategy.GetResultantText();
                }
                string extractedText = ExtractUsefulText(text);
                Console.WriteLine(extractedText);
                return extractedText;
            }
        }        
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
    public async Task<string> ExtractTextFromDocxUrl(string url)
    {
        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                // Read the DocX document from the response stream asynchronously.
                var docBytes = await response.Content.ReadAsByteArrayAsync();
                using (MemoryStream stream = new MemoryStream(docBytes))
                {
                    return await ExtractTextFromDocxStream(stream);
                }
            }
            else
            {
                return $"Failed to retrieve the web page. Status Code: {response.StatusCode}";
            }
        }
    }
    
    //this method parses the text and removes all the useless characters
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

    //these methods extracts files from streams
    private async Task<string> ExtractTextFromPdfStream(MemoryStream stream)
    {
        string text = "";

        await Task.Run(() =>
        {
            using (PdfReader pdfReader = new PdfReader(stream))
            {
                using (PdfDocument pdfDoc = new PdfDocument(pdfReader))
                {
                    for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
                    {
                        PdfPage page = pdfDoc.GetPage(pageNum);
                        LocationTextExtractionStrategy strategy = new LocationTextExtractionStrategy();
                        new PdfCanvasProcessor(strategy).ProcessPageContent(page);

                        // Append the text extracted from this page to the result.
                        text += strategy.GetResultantText();
                    }
                }
            }
        });

        string extractedText = ExtractUsefulText(text);
        Console.WriteLine(extractedText);
        return extractedText;
    }
    public async Task<string> ExtractTextFromDocxStream(MemoryStream stream)
    {
        try
        {
            var doc = await Task.Run(() => DocX.Load(stream));
            // Extract text from the DocX document.
            string extractedText = doc.Text;
            extractedText = ExtractUsefulText(extractedText);
            return extractedText;
        }
        catch (Exception ex)
        {
            return $"Failed to extract text from DocX stream. Error: {ex.Message}";
        }
    }

    public async Task<string> ExtractTextFromTxtStream(MemoryStream stream)
    {
        try
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                // Read the text content from the stream asynchronously.
                return await reader.ReadToEndAsync();
            }
        }
        catch (Exception ex)
        {
            return $"Failed to extract text from TXT stream. Error: {ex.Message}";
        }
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
        //else, if the input is a path to a file try to get the file type
        else if (File.Exists(pathOrUrl))
        {
            IsUrl = false;
            FileType = GetFileType(pathOrUrl);
            PathOrUrl = pathOrUrl;
        }
        else
        {
            // If the input is neither a well-formed URL nor a path to a file, use it as final text only if it is longer tha 150 characters.
            if (pathOrUrl.Length < 150)
                throw new ArgumentException("Invalid input.");
            IsUrl = false;
            PathOrUrl = pathOrUrl;
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

