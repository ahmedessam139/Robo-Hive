using System.Activities;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Threading;
using System.Windows;

namespace YourNamespace
{
    public class ExtractTextFromPdfActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FilePath.Get(context);
            StringBuilder text = new StringBuilder();

            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, page, strategy);
                    text.AppendLine(pageText);
                }
            }

            string extractedText = text.ToString();

            // Copy extracted text to clipboard on a separate thread with STA mode
            Thread staThread = new Thread(() => SetClipboardText(extractedText));
            staThread.SetApartmentState(ApartmentState.STA);
            staThread.Start();
        }

        private void SetClipboardText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
