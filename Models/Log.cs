using Microsoft.Maui.Storage;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace EasyPaperWork.Models
{
    public class Log
    {
        private string _dateTime;
        public string? dateTime { get; set; }

        private string _menssage;
        public string menssage { get { return _menssage; } set { _menssage = value; } }
        public Log(string filename, int option)
        {
            switch (option)
            {
                case 1:
                    _menssage = string.Concat("Documento adicionado. Nome = ", filename);
                    dateTime = DateTime.UtcNow.ToString();
                    break;
                case 2:
                    _menssage = string.Concat("Documento Removido. Nome = ", filename);
                    _dateTime = DateTime.UtcNow.ToString();
                    break;
                case 3:
                    _menssage = string.Concat("Documento Atualizado. Nome =  ", filename);
                    _dateTime = DateTime.UtcNow.ToString();
                    break;
                case 4:
                    _menssage = string.Concat("Documento Baixado. Nome =  ", filename);
                    _dateTime = DateTime.UtcNow.ToString();
                    break;

            }
        }
        public Log() { }
        public async void GeneratePdfFromLogs(Collection<Log> logs)
        {
            try
            {
                PdfDocument pdfDocument = new PdfDocument();


                PdfPage page = pdfDocument.AddPage();

                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("Arial", 12, XFontStyle.Regular);

                double yPosition = 40; // Posição inicial na página

                // Título do documento
                gfx.DrawString("Logs", new XFont("Arial", 18, XFontStyle.Bold), XBrushes.Black, new XRect(0, 20, page.Width, page.Height), XStringFormats.TopCenter);

                // Itera sobre a coleção de logs
                foreach (Log log in logs)
                {
                    string logEntry1 = $"Message: {log.menssage}";
                    gfx.DrawString(logEntry1, font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += 20;
                    string logEntry2 = $"| DateTime: {log.dateTime}";
                    gfx.DrawString(logEntry2, font, XBrushes.Black, new XPoint(40, yPosition));

                    // Incrementa a posição vertical
                    yPosition += 20;

                    // Se ultrapassar o limite da página, cria uma nova página
                    if (yPosition > page.Height - 40)
                    {
                        page = pdfDocument.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40;
                    }
                }
                string PathFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "logs.pdf");
                using (MemoryStream pdfStream = new MemoryStream())
                { pdfDocument.Save(pdfStream, false);
                    // Salva o documento em um arquivo
                    using (FileStream stream = new FileStream(PathFile, FileMode.Create, FileAccess.Write))
                    {
                       await pdfStream.CopyToAsync(stream);
                        stream.Dispose();
                        stream.Close();
                        pdfStream.Dispose();
                        pdfStream.Close();
                    }

                    Debug.WriteLine("PDF gerado com sucesso "); }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "ok");
            }
        }
    }
}
