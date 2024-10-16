using Castle.Components.DictionaryAdapter.Xml;
using EasyPaperWork.Models;
using EasyPaperWork.Security;
using EasyPaperWork.Services;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;



using System.Threading.Tasks;
using System.Security.Principal;


namespace EasyPaperWork.ViewModel
{

    public class UploadDocsViewModel : INotifyPropertyChanged
    {

        private Documents documentsModel;
        private EncryptData encryptData;
        private Log log;
        private byte[] key;
        public ICommand PickFileCommand { get; }
        private Scanner scanner;
        private Label LabelMensageError;
        public ICommand ScanFileCommand { get; }
        private FirebaseStorageService storageService;
        private FirebaseService firebaseService;
        private string _selectedFileName;
        private Process _process;
        public string SelectedFileName
        {
            get => _selectedFileName;
            set
            {
                _selectedFileName = value;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        private string UidUser;
        public string _UidUser
        {
            get { return UidUser; }
            set
            {
                UidUser = HttpUtility.UrlDecode(value);
                OnPropertyChanged(nameof(_UidUser));

            }
        }

        public UploadDocsViewModel()
        {
       
            encryptData = new EncryptData();
            LabelMensageError = new Label();
            storageService = new FirebaseStorageService();
            firebaseService = new FirebaseService();
            _UidUser = AppData.UserUid;
            Initialize();
            ScanFileCommand = new Command(async () => await ScanFileAsync());
            documentsModel = new Documents();
            PickFileCommand = new Command(async () => await PickAndShowFileAsync());
            scanner = new Scanner();
            key = encryptData.GetKey(AppData.Salt, AppData.UserPassword);

        }
        public bool IsRunningAsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public  void RestartAsAdministrator()
        {
            if (!IsRunningAsAdministrator())
            {
                try
                {
                    // Cria um novo processo do próprio aplicativo com privilégios de administrador
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = Process.GetCurrentProcess().MainModule.FileName, // Nome do executável atual
                        UseShellExecute = true,
                        Verb = "runas",
                        Arguments = string.Empty,
                        // Executa com permissões elevadas
                    };
                    Process atual = Process.GetCurrentProcess();

                    // Inicia o novo processo com permissões elevadas
                    using (Process newProcess = Process.Start(processInfo))
                    {
                    }
                }
                finally
                {
                    try
                    {
                        Environment.Exit(0);
                    }
                    catch (Exception ex)
                    {
                        // Caso o usuário cancele ou ocorra outro erro
                        Debug.WriteLine($"Erro ao tentar reiniciar o aplicativo como administrador: {ex.Message}");
                    }

                }
               

                
            }
        }



        private async Task ScanFileAsync()
        {
            if (IsRunningAsAdministrator())
            {
                documentsModel.Name = await Application.Current.MainPage.DisplayPromptAsync("Scan", "Isira o nome do arquivo", "Ok", "Cancelar");
                if (!string.IsNullOrEmpty(documentsModel.Name))
                {
                    string filepath = await scanner.ScanDocumentAsync(documentsModel.Name);

                    if (!string.IsNullOrEmpty(filepath))
                    {

                        string PathTemporaryEncryptFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), string.Concat("Encrypt", documentsModel.Name, ".pdf"));
                        try
                        {
                            encryptData.EncryptFile(filepath, PathTemporaryEncryptFile, AppData.UserPassword, AppData.Salt);
                            var stream = File.Open(PathTemporaryEncryptFile, FileMode.Open);
                            if (string.IsNullOrEmpty(AppData.CurrentFolder))
                            {
                                documentsModel.UrlDownload = await storageService.UploadFileAsync(stream, documentsModel.Name, "Pasta inicial");
                                Log newlog = new Log(documentsModel.Name,1);
                                await firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                                documentsModel.DocumentType = ".pdf";
                                documentsModel.RootFolder = "Pasta inicial";
                                documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);


                                await firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);
                                await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial ", "Ok");
                            }
                            else
                            {

                                documentsModel.UrlDownload = await storageService.UploadFileAsync(stream, documentsModel.Name, AppData.CurrentFolder);
                                Log newlog = new Log(documentsModel.Name, 1);
                                await firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                                documentsModel.DocumentType = ".pdf";
                                documentsModel.RootFolder = AppData.CurrentFolder;
                                documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                                await firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
                                await Application.Current.MainPage.DisplayAlert("Succsses", $"Aquivo enviado para {AppData.CurrentFolder} ", "Ok");
                            }
                            stream.Dispose();
                            stream.Close();
                            if (File.Exists(filepath))
                            {
                                File.Delete(filepath);
                            }
                            if (File.Exists(PathTemporaryEncryptFile))
                            {
                                File.Delete(PathTemporaryEncryptFile);
                            }


                        }
                        catch (Exception ex)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                        }

                    }

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Nome inválido", "ok");

                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Re inicie a aplicação em modo administrador ", "ok");
                RestartAsAdministrator(); 
            }
        }
        private async Task PickAndShowFileAsync()
        {
            try
            {
                var fileResult = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Por favor selecione um arquivo",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>

                {
                    { DevicePlatform.WinUI, new[] { ".pdf", ".docx", ".doc", ".xls", ".xlsx", ".pptx" } },
                    { DevicePlatform.Android, new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/msword", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.openxmlformats-officedocument.presentationml.presentation" } },
                    { DevicePlatform.iOS, new[] { "com.adobe.pdf", "org.openxmlformats.wordprocessingml.document", "com.microsoft.word.doc", "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet", "org.openxmlformats.presentationml.presentation" } }
                })
                });

                if (fileResult != null)
                {

                    string PathTemporaryEncryptFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{fileResult.FileName}");

                    encryptData.EncryptFile(fileResult.FullPath, PathTemporaryEncryptFile, AppData.UserPassword, AppData.Salt);

                    var stream = File.Open(PathTemporaryEncryptFile, FileMode.Open);
                    documentsModel.Name = fileResult.FileName;
                    if (fileResult.FileName.Contains(".docx") || fileResult.FileName.Contains(".doc"))
                    {
                        documentsModel.DocumentType = ".docx";
                    }
                    if (fileResult.FileName.Contains(".pdf"))
                    {
                        documentsModel.DocumentType = ".pdf";
                    }
                    if (fileResult.FileName.Contains(".xls") || fileResult.FileName.Contains(".xlsx"))
                    {
                        documentsModel.DocumentType = ".xlsx";
                    }
                    if (fileResult.FileName.Contains(".pptx"))
                    {
                        documentsModel.DocumentType = ".pptx";
                    }
                    if (!string.IsNullOrEmpty(AppData.CurrentFolder))
                    {
                        documentsModel.RootFolder = AppData.CurrentFolder;
                    }
                    else if (string.IsNullOrEmpty(AppData.CurrentFolder))
                    {
                        documentsModel.RootFolder = "Pasta inicial";
                    }
                    if (!string.IsNullOrEmpty(fileResult.FileName) && !string.Equals("Adicone um documento", fileResult.FileName))
                    {
                        if (string.IsNullOrEmpty(AppData.CurrentFolder))
                        {

                            documentsModel.UrlDownload = await storageService.UploadFileAsync(stream, fileResult.FileName, "Pasta inicial");
                            Log newlog = new Log(fileResult.FileName, 1);
                            await firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                            documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                            documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                            documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                            documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                            documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);

                            await firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);

                            await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial", "Ok");
                        }
                        else
                        {

                            documentsModel.UrlDownload = await storageService.UploadFileAsync(stream, fileResult.FileName, AppData.CurrentFolder);
                            Log newlog = new Log(fileResult.FileName, 1);
                            await firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                            documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                            documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                            documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                            documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                            documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);

                            await firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
                            await Application.Current.MainPage.DisplayAlert("Succsses", $"Aquivo enviado para {AppData.CurrentFolder} ", "Ok");
                        }
                        stream.Dispose();
                        stream.Close();
                        if (File.Exists(PathTemporaryEncryptFile))
                        {
                            File.Delete(PathTemporaryEncryptFile);
                        }

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "O nome do arquivo  é  inválido", "Ok");
                    }



                }
            }
            catch (System.Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        private async Task<byte[]> SendStringToConsoleApp(string message)
        {
            try
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream("mypipe"))
                {
                    // Conectar ao servidor de pipes
                    await pipeClient.ConnectAsync();

                    using (StreamWriter writer = new StreamWriter(pipeClient) { AutoFlush = true })
                    using (StreamReader reader = new StreamReader(pipeClient))
                    {
                        // Enviar a string para o servidor de console
                        await writer.WriteLineAsync(message);

                        // Ler a resposta (byte[] codificado em Base64)
                        string base64String = await reader.ReadLineAsync();
                        if (!string.IsNullOrEmpty(base64String))
                        {
                            // Converter Base64 para byte[]
                            byte[] byteArray = Convert.FromBase64String(base64String);
                            return byteArray;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Tratar a exceção e retornar null em caso de erro
                Console.WriteLine($"Erro: {ex.Message}");
            }

            // Retornar null se ocorrer algum erro
            return null;
        }

        public void Initialize()
        {
            if (!string.IsNullOrEmpty(_UidUser))
            {
                Debug.WriteLine("Recebeu UId");
            }
            else
            {
                Debug.WriteLine("Não Recebeu UId");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

