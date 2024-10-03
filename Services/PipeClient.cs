using EasyPaperWork.Models;
using ModuleScanner;
using System.Diagnostics;
using System.IO.Pipes;

public class PipeClient
{
    ScannerModule scannerModule = new ScannerModule();

    public async Task<string> StartClientAsync(string nome)
    {
        try
        {
            using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("mypipe"))
            {
                // Conectar ao servidor de pipes
                await pipeClient.ConnectAsync();
                Debug.WriteLine("Conectado ao servidor de pipes.");

                using (StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(pipeClient))
                {
                    // Envia a mensagem para o servidor
                    await writer.WriteLineAsync(nome);
                    await writer.FlushAsync(); // Garante que a mensagem seja enviada imediatamente
                    Debug.WriteLine($"Mensagem enviada ao servidor: {nome}");
                    await Task.Delay(23000);
                    // Recebe a resposta do servidor
                    string messageFromServer = await reader.ReadLineAsync();
                    if (!string.IsNullOrEmpty(messageFromServer))
                    {
                        Debug.WriteLine($"Mensagem recebida do servidor: {messageFromServer}");
                        AppData.ServerResult= messageFromServer;
                        return messageFromServer;
                    }
                    else
                    {
                        Debug.WriteLine("Nenhuma mensagem recebida do servidor.");
                        return null;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro ao se comunicar com o servidor de pipes: {ex.Message}");
            return null;
        }
    }
}
