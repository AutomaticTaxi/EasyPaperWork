using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public class PipeServer
    {
        public async Task<string> StartServerAsync(string messageToSend)
        {
           
                NamedPipeServerStream pipeServer = new NamedPipeServerStream("mypipe", PipeDirection.InOut);
                
                    Debug.WriteLine("Aguardando conexão do cliente...");
                    await pipeServer.WaitForConnectionAsync();

                    StreamWriter writer = new StreamWriter(pipeServer);
                    StreamReader reader = new StreamReader(pipeServer);
            try
            {
                // Envia uma mensagem para o cliente
                await writer.WriteLineAsync(messageToSend);
                        Debug.WriteLine($"Mensagem enviada ao cliente: {messageToSend}");

                        // Recebe a resposta do cliente
                        string response = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(response))
                        {
                            Debug.WriteLine($"Resposta do cliente: {response}");
                            return response;
                        }
                        return "Error ao conectar com moduloScan";
                    
                
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("error", ex.ToString(), "ok");
                return "Error ao conectar com moduloScan";
            }
            finally
            {
                
              
                    // Garante que a conexão será fechada de forma correta
                    if (pipeServer.IsConnected)
                    {
                        pipeServer.Disconnect();
                        Debug.WriteLine("Conexão com o cliente encerrada.");
                    }
                
            }
        }

    }
}
