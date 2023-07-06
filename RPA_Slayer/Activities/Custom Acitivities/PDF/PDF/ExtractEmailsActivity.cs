using System;
using System.Activities;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PDF
{
    public class ExtractEmailsActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> PdfFilePath { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string pdfFilePath = PdfFilePath.Get(context);
            string extractedEmails = ExtractEmailsFromPdf(pdfFilePath);

            // Copy extracted emails to clipboard
            SetClipboardText(extractedEmails);
        }

        private string ExtractEmailsFromPdf(string filePath)
        {
            string extractedEmails = string.Empty;

            try
            {
                using (PdfReader reader = new PdfReader(filePath))
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        string pageText = PdfTextExtractor.GetTextFromPage(reader, i);
                        extractedEmails += ExtractEmails(pageText);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting emails from PDF: {ex.Message}");
            }

            return extractedEmails;
        }

        private string ExtractEmails(string text)
        {
            const string emailPattern = @"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}\b";

            MatchCollection matches = Regex.Matches(text, emailPattern);
            string extractedEmails = string.Join(", ", matches);

            return extractedEmails;
        }

        private void SetClipboardText(string text)
        {
            // Set STA mode for accessing the clipboard
            Thread thread = new Thread(() => SetClipboardTextSTA(text));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }

        private void SetClipboardTextSTA(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (COMException ex)
            {
                Console.WriteLine($"Error setting clipboard text: {ex.Message}");
            }
        }
    }
}
