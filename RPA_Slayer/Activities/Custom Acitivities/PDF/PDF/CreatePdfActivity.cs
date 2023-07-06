using System.Activities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace PDF
{
    public class CreatePdfActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> FilePath { get; set; }

        [RequiredArgument]
        public InArgument<string> Text { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            string filePath = FilePath.Get(context);
            string text = Text.Get(context);

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                using (Document doc = new Document())
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                    doc.Open();
                    doc.Add(new Paragraph(text));
                    doc.Close();
                }
            }
        }
    }
}
