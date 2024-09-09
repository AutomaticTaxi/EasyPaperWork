using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using WIA;
using WiaDeviceInfo = WIA.DeviceInfo;  // Alias para WIA.DeviceInfo
using PdfSharpCore.Drawing;  // Necessário para XGraphics e XImage
using PdfSharpCore.Pdf;      // Necessário para PdfDocument e PdfPage

namespace EasyPaperWork.Services
{
    public class Scanner
    {
        public async Task<MemoryStream> ScanDocumentAsync(string filename)
        {
            try
            {
                // Cria o objeto DeviceManager da WIA
                DeviceManager deviceManager = new DeviceManager();

                // Procura por dispositivos de scanner conectados
                WiaDeviceInfo availableScanner = null;
                for (int i = 1; i <= deviceManager.DeviceInfos.Count; i++)
                {
                    WiaDeviceInfo deviceInfo = deviceManager.DeviceInfos[i];
                    if (deviceInfo.Type == WiaDeviceType.ScannerDeviceType)
                    {
                        availableScanner = deviceInfo;
                        break;
                    }
                }

                if (availableScanner == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Nenhum scanner detectado", "Ok");
                    return null;
                }

                try
                {
                    // Conecta ao scanner disponível
                    IDevice scannerDevice = availableScanner.Connect();  // Use o alias WiaDevice

                    // Seleciona o item do scanner
                    Item scannerItem = scannerDevice.Items[1];

                    // Configura o processo de digitalização
                    var commonDialog = new CommonDialog();
                    ImageFile imageFile = (ImageFile)commonDialog.ShowTransfer(scannerItem, "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}",false);

                    // Salva o arquivo digitalizado
                    imageFile.SaveFile(filename);
                    Debug.WriteLine($"Arquivo salvo como: {filename}");
                    byte[] imageBytes = (byte[])imageFile.FileData.get_BinaryData();

                    // Criando o PDF e retornando um stream de memória
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PdfSharpCore.Pdf.PdfDocument document = new PdfSharpCore.Pdf.PdfDocument();  // Use o namespace completo
                        PdfSharpCore.Pdf.PdfPage page = document.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);

                        using (MemoryStream imageStream = new MemoryStream(imageBytes))
                        {
                            XImage image = XImage.FromStream(() => new MemoryStream(imageBytes));  // Passa um Func<Stream>

                            // Ajusta o tamanho da página ao tamanho da imagem
                            page.Width = image.PointWidth;
                            page.Height = image.PointHeight;

                            gfx.DrawImage(image, 0, 0, image.PointWidth, image.PointHeight);
                        }

                        // Salva o PDF em um stream de memória
                        document.Save(pdfStream, false);

                        // Retorna o stream de memória com o PDF
                        pdfStream.Position = 0;  // Reseta a posição do stream para o início
                        await Application.Current.MainPage.DisplayAlert("Sucesso", "Documeto scaneado com sucesso", "Ok");
                        Debug.WriteLine("Documeto scaneado com sucesso");
                        return pdfStream;
                    }
                }
                catch (COMException comEx)
                {
                    // Trata a exceção COMException específica do scanner
                    Debug.WriteLine($"Erro de COM: {comEx.Message}");
                  
                    if (comEx.ErrorCode == unchecked((int)0x8021000A))
                    {
                        await Application.Current.MainPage.DisplayAlert("Erro", "O scanner está ocupado ou não está disponível.", "Ok");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erro", $"Erro desconhecido: {comEx.Message}", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message== "Acesso negado. (0x80070005 (E_ACCESSDENIED))")
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", "Para efetuar o scaneameto execute o aplicativo em modo administrador.", "Ok");
                }
                await Application.Current.MainPage.DisplayAlert("Erro", "Erro ao scannear", "Ok");
            }

            return null;  // Retorna null em caso de erro
        }
    }
}
