using PdfSharpCore.Drawing;  
using PdfSharpCore.Pdf;      
using System.Runtime.InteropServices;
using WIA;
using WiaDeviceInfo = WIA.DeviceInfo;  // Alias para WIA.DeviceInfo

namespace EasyPaperWork.Services
{
    public class Scanner
    {
        public async Task<string> ScanDocumentAsync(string filename)
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

                        //ToDo mostrar aos usuários scanners disponíveis
                        availableScanner = deviceInfo;
                        break;
                    }
                }

                if (availableScanner == null)
                {
                    Console.WriteLine("Nenhum scanner detectado.");
                    return null;
                }

                try
                {
                    // Conecta ao scanner disponível
                    IDevice scannerDevice = availableScanner.Connect();

                    // Seleciona o item do scanner
                    Item scannerItem = scannerDevice.Items[1];

                    // Configura o processo de digitalização
                    var commonDialog = new CommonDialog();
                    ImageFile imageFile = (ImageFile)commonDialog.ShowTransfer(scannerItem, "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}", false);

                    // Salva o arquivo digitalizado
                    imageFile.SaveFile(filename);
                    Console.WriteLine($"Arquivo salvo como: {filename}");

                    // Converte os dados binários da imagem
                    byte[] imageBytes = (byte[])imageFile.FileData.get_BinaryData();

                    // Criando o PDF e retornando um stream de memória
                    using (MemoryStream pdfStream = new MemoryStream())
                    {
                        PdfDocument document = new PdfDocument();
                        PdfPage page = document.AddPage();
                        XGraphics gfx = XGraphics.FromPdfPage(page);

                        using (MemoryStream imageStream = new MemoryStream(imageBytes))
                        {
                            XImage image = XImage.FromStream(() => new MemoryStream(imageBytes));

                            // Ajusta o tamanho da página ao tamanho da imagem
                            page.Width = image.PointWidth;
                            page.Height = image.PointHeight;

                            gfx.DrawImage(image, 0, 0, image.PointWidth, image.PointHeight);

                            // Salva o PDF em um stream de memória
                            document.Save(pdfStream, false);
                        }

                        // Define o caminho do arquivo PDF a ser salvo
                        string PathTemporaryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}.pdf");

                        // Verifica se o arquivo já existe
                        if (File.Exists(PathTemporaryFile))
                        {
                            string action = await Application.Current.MainPage.DisplayActionSheet(
                            "Na pasta Documentos este arquivo já existe ", null, null, "Remover", "Manter Ambos");

                            switch (action)
                            {
                                case "Remover":
                                    File.Delete(PathTemporaryFile);
                                    break;
                                case "Manter Ambos":
                                    PathTemporaryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{filename}{DateTime.Now}.pdf");
                                    break;
                                default:
                                    break;
                            }

                        }

                        // Salva o PDF no caminho especificado
                        using (FileStream DocumentScannedSave = new FileStream(PathTemporaryFile, FileMode.Create, FileAccess.ReadWrite))
                        {
                            pdfStream.Position = 0; // Reseta a posição do stream para o início
                            await pdfStream.CopyToAsync(DocumentScannedSave);
                            DocumentScannedSave.Dispose();
                            DocumentScannedSave.Close();
                        }

                        await Application.Current.MainPage.DisplayAlert("Sucesso", "Documento escaneado e salvo com sucesso.", "Ok");
                        pdfStream.Dispose();
                        pdfStream.Close();
                      
                        return PathTemporaryFile;
                        
                    }
                }
                catch (COMException comEx)
                {
                    // Trata a exceção COMException específica do scanner
                    await Application.Current.MainPage.DisplayAlert("Error", $"Erro de COM: {comEx.Message}", "ok");

                    if (comEx.ErrorCode == unchecked((int)0x8021000A))
                    {
                        Console.WriteLine("O scanner está ocupado ou não está disponível.");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", $"Erro desconhecido: {comEx.Message}", "ok");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "Acesso negado. (0x80070005 (E_ACCESSDENIED))")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Execute o aplicativo em modo administrador para realizar o escaneamento.", "ok");
                }
                await Application.Current.MainPage.DisplayAlert("Error", $"Erro ao escanear: {ex.Message}", "ok");
            }

            return null; // Retorna null em caso de erro
        }
    }
}
