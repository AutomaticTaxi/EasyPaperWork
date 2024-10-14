﻿
using __XamlGeneratedCode__;
using EasyPaperWork.Models;
using EasyPaperWork.Security;
using EasyPaperWork.Services;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Web;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel;

public  class Main_ViewModel_Files: INotifyPropertyChanged
{
    public ObservableCollection<Folder_Files> FolderCollection { get; set; }
    public ObservableCollection<Documents> DocumentCollection { get; set; }
    private Documents documentsModel;
    private Scanner scanner;
    public ICommand BtSearchFile { get; }
    public ICommand BtSearchFolder { get; }
    public ICommand BtRefresh { get; }
    private Documents Documento;
    private Log log;
    private FirebaseService _firebaseService;
    private FirebaseStorageService _firebaseStorageService;
    private EncryptData encryptData;
    private string _LabelTituloRepositorio;
    private IFileSavePickerService _fileSavePickerService;
    private byte[] key;

    private WindowsFileSavePickerService service;

    private string _EntryArchiveName;
    public string EntryArchiveName
    {
        get { return _EntryArchiveName; }
        set
        {
            _EntryArchiveName = value;
            OnPropertyChanged(nameof(EntryArchiveName));
        }
    }
    private string _EntryFolderName;
    public string EntryFolderName
    {
        get { return _EntryFolderName; }
        set
        {
            _EntryFolderName = value;
            OnPropertyChanged(nameof(EntryFolderName));
        }
    }
    public IFileSavePickerService FileSavePickerService
    {
        get => _fileSavePickerService;
        set
        {
            _fileSavePickerService = value;
            OnPropertyChanged(nameof(FileSavePickerService));
        }
    }

    public string LabelTituloRepositorio { get { return _LabelTituloRepositorio; }
        set
        {
            _LabelTituloRepositorio = value;
            OnPropertyChanged(nameof(LabelTituloRepositorio));
        }
    }
    private string _LabelNomeDocumento;
    public string LabelNomeDocumento
    {
        get { return _LabelNomeDocumento; }
        set
        {
            _LabelNomeDocumento = value;
            OnPropertyChanged(nameof(LabelNomeDocumento));
        }
    }
    private string _ImageDocumento;
    public string ImageDocumento
    {
        get { return _ImageDocumento; }
        set
        {
            _ImageDocumento = value;
            OnPropertyChanged(nameof(ImageDocumento));
        }
    }
    private UserModel userModel { get; set; }


    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; } 
        set { 
            UidUser = value;
            OnPropertyChanged(nameof(_UidUser));
        }
    }
    private Folder_Files Folder_Files;
    public  Main_ViewModel_Files()
    {
        BtSearchFile = new Command(async () => await SearchFile());
        BtRefresh = new Command(async () => list_files(AppData.CurrentFolder));
       
        service = new WindowsFileSavePickerService();
        scanner = new Scanner();
        Log log = new Log();
        UidUser = AppData.UserUid;
        _firebaseService = new FirebaseService();
        _firebaseStorageService = new FirebaseStorageService();
        encryptData = new EncryptData();
        userModel = new UserModel();
        documentsModel = new Documents();
        Folder_Files = new Folder_Files();
        

        FolderCollection = new ObservableCollection<Folder_Files>();
     
        DocumentCollection = new ObservableCollection<Documents>();
       // list_files(AppData.CurrentFolder);


    }

 public async void list_files(string currentfolder)
    {
        if (!string.IsNullOrEmpty(UidUser))
        {

            Debug.WriteLine("banco on");
            
            AppData.Salt = encryptData.GetSaltBytes( await _firebaseService.GetSalt(AppData.UserUid));

            AppData.Key = encryptData.GetKey(AppData.Salt,AppData.UserPassword);
            key = encryptData.GetKey(AppData.Salt, AppData.UserPassword);
            userModel = await _firebaseService.BuscarUserModelAsync("Users", UidUser);
            Debug.WriteLine(userModel.Id);

            DocumentCollection.Clear();
            DocumentCollection.Add(new Documents { Name = "Adicone um documento" });
            FolderCollection.Clear();
            FolderCollection.Add(new Folder_Files { Name = "Adicione uma pasta" });
            
            
            if (string.IsNullOrEmpty(currentfolder))
            {
                LabelTituloRepositorio = "Pasta inicial";
                List<Documents> list = await _firebaseService.ListFiles("Users", AppData.UserUid, "Pasta inicial");
                
                foreach (Documents doc in list)
                {

                    doc.Name = encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt);                 
                    doc.DocumentType = encryptData.Decrypt(doc.DocumentType, AppData.Key, AppData.Salt); 
                    
                    DocumentCollection.Add(doc);

                }
            }
            else
            {
                LabelTituloRepositorio = currentfolder;
                List<Documents> list = await _firebaseService.ListFiles("Users", AppData.UserUid, currentfolder);
                Debug.WriteLine(list.ToString());
                foreach (Documents doc in list)
                {
                   doc.Name = encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt);
                    doc.DocumentType = encryptData.Decrypt(doc.DocumentType, AppData.Key, AppData.Salt);

                    DocumentCollection.Add(doc);

                }
            }
            List<Folder_Files> list_folder = await _firebaseService.ListFolder("Users", AppData.UserUid);
            foreach(Folder_Files folder in list_folder)
            {
                FolderCollection.Add(folder);   
            }
           
        }
        else { Debug.WriteLine("banco off"); }
    }

    public async void OnDocumentItemTapped(Documents item)
    {
        if (item == null)
        {
            Debug.WriteLine("Item is null");
            return;
        }

        Debug.WriteLine($"Item selecionado: {item.Name}");
        if (item.Name == "Adicone um documento")
        {
         if (string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                AppData.CurrentFolder = "Pasta inicial";
            }
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Buscar no PC", "Scannear", "Excluir");
            switch (action)
            {
                case "Buscar no PC":
                    if (IsRunningAsAdministrator())
                    {
                        await Application.Current.MainPage.DisplayAlert("Ação não permitida", "Re inicie o programa em modo não administrador", "Ok");
                    }
                    await PickAndShowFileAsync();
                    break;
                case "Scannear":
                    await ScanFileAsync();
                    break;
                default:
                    break;
            }
        }
        else
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Download", "Visualizar","Excluir");

            switch (action)
            {
                case "Download":
                    await DownloadFile(item);
                    break;
                case "Visualizar":
                    await VisualizarArquivo(item);
                    break;
                case "Excluir":
                    await DeleteFile(item);
                    break;
                default:
                    break;
            }
        }
    }
    public async void OnFolderItemTaped(Folder_Files item)
    {
        if(item.Name == "Adicione uma pasta")
        {
            string namefolder =  await Application.Current.MainPage.DisplayPromptAsync("Criação de pasta", "Digite o nome de sua pasta", "Ok", "Cancel");
            if (!string.IsNullOrEmpty(namefolder) && !string.Equals("Adicione uma pasta",namefolder) )
            {
                Folder_Files.Name= namefolder;
                FolderCollection.Add(Folder_Files);
                await _firebaseService.AdicionarObjetoAsync("Users", AppData.UserUid, Folder_Files);
                
            }
            else { await Application.Current.MainPage.DisplayAlert("Erro", "Nome Inválido", "Ok"); }
        }
        else if(item.Name == "Logs")
        {
            await Shell.Current.GoToAsync("//Logs_Page");
        }
        else
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Escolha uma ação", "Cancelar", null, "Abrir", "Editar", "Excluir");

            switch (action)
            {
                case "Abrir":
                    AppData.CurrentFolder = item.Name;
                    LabelTituloRepositorio = item.Name;
                    list_files(item.Name);
                    break;
                case "Editar":
                    
                    break;
                case "Excluir":
                    await DeleteFolder(item);
                    list_files("Pasta inicial");
                    break;
                default:
                    break;
            }
            
        }
    }
    private async Task<string> DownloadFile(Documents selectedItem)
    {
        try
        {
            Debug.WriteLine($"Downloading file {selectedItem.Name}");
            byte[] fileBytes;
            if (string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                Log newlog = new Log(selectedItem.Name,4);
               //Consertar download
                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, "Pasta inicial", selectedItem.Name); // Sua lógica para obter os bytes do arquivo
            }
            else
            {
                Log newlog = new Log(selectedItem.Name,4);
                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, AppData.CurrentFolder, selectedItem.Name); // Sua lógica para obter os bytes do arquivo
            }
            if (fileBytes != null)
            {
                string path = await service.PickFolderAsync();
                string CompletedPath = Path.Combine(path, $"{selectedItem.Name}.pdf");
                string PathTemporaryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{selectedItem.Name}encrypt.pdf");
                FileStream DocumentEncryptSave = new FileStream(PathTemporaryFile, FileMode.Create, FileAccess.Write);
                DocumentEncryptSave.Write(fileBytes);
                DocumentEncryptSave.Dispose();
                DocumentEncryptSave.Close();

                encryptData.DecryptFile(PathTemporaryFile, CompletedPath, AppData.UserPassword);
                if (File.Exists(PathTemporaryFile))
                {
                    File.Delete(PathTemporaryFile);
                }

                if (path != null)
                {
                    // Arquivo salvo com sucesso
                    Console.WriteLine($"Arquivo salvo em: {path}");
                    return "success";
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Caminho para downlod inválido", "ok");
                    return "caminho inválido";
                }
            }
            return "error";
        }
        catch (DirectoryNotFoundException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            return "error";
        }
        catch (PermissionException ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            return "error";
        }
        catch (Exception ex) {

            await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
            return "error";

        }
    }
    private async Task<string> DeleteFile(Documents selectedItem)
    {
       
        string action = await Application.Current.MainPage.DisplayActionSheet("Deseja remover o documento", "cancel", null, "sim");
        if (action == "sim") {
            if (string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                if (await _firebaseService.DeleteFileAsync("Users", AppData.UserUid, "Pasta inicial", selectedItem.Name))
                {
                    if (await _firebaseStorageService.DeleteFileAsync(AppData.UserUid, "Pasta inicial", selectedItem.Name))
                    {
                        Log newlog = new Log(selectedItem.Name, 2);

                        await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                        DocumentCollection.Remove(selectedItem);
                        Console.WriteLine("Arquivo removido");
                        return "success";
                    }
                }
            }
            else if (await _firebaseService.DeleteFileAsync("Users", AppData.UserUid, AppData.CurrentFolder, selectedItem.Name))
            {
                if (await _firebaseStorageService.DeleteFileAsync(AppData.UserUid, AppData.CurrentFolder, selectedItem.Name))
                {

                    Log newlog = new Log(selectedItem.Name, 2);

                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                    DocumentCollection.Remove(selectedItem);
                    Console.WriteLine("Arquivo removido");
                    return "success";
                }
            }
        }
        return "error";
    }

    private Task VisualizarArquivo(Documents selectedItem)
    {
        // Implementar a lógica para visualização
        return Task.CompletedTask;
    }
    private async Task DeleteFolder(Folder_Files selectedItem)
    {
        if (await _firebaseStorageService.DeleteFolderAsync(selectedItem.Name))
         {
             if (await _firebaseService.DeleteFolderAsync(selectedItem.Name))
                 FolderCollection.Remove(selectedItem);
                 await Application.Current.MainPage.DisplayAlert("Succsses", "Pasta removida com sucesso", "Ok");
         }
         else { await Application.Current.MainPage.DisplayAlert("Error", "Falha em remover a pasta ", "Ok"); }
         
       
    }
    private async Task SearchFile()
    {
        if (EntryArchiveName != null)
            if (!string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                var result = DocumentCollection
                 .Where(doc => doc.Name != null &&
                               doc.Name.IndexOf(EntryArchiveName, StringComparison.OrdinalIgnoreCase) >= 0)
                 .ToList();
                DocumentCollection.Clear();
                foreach (Documents doc in result)
                {
                  DocumentCollection.Add(doc);
                }
            }
            else
            {
                var result = DocumentCollection
                .Where(doc => doc.Name != null &&
                              doc.Name.IndexOf(EntryArchiveName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
                DocumentCollection.Clear();
                foreach (Documents doc in result)
                {
                    DocumentCollection.Add(doc);
                }
            }
    }
    private async Task SearchFolder()
    {
        if (EntryArchiveName != null)
            if (!string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                var result = FolderCollection
                 .Where(doc => doc.Name != null &&
                               doc.Name.IndexOf(EntryArchiveName, StringComparison.OrdinalIgnoreCase) >= 0)
                 .ToList();
                FolderCollection.Clear();
                foreach (Folder_Files folder in result)
                {
                    FolderCollection.Add(folder);
                }
            }
            else
            {
                var result = FolderCollection
                .Where(doc => doc.Name != null &&
                              doc.Name.IndexOf(EntryArchiveName, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
                FolderCollection.Clear();
                foreach (Folder_Files folder in result)
                {
                    FolderCollection.Add(folder);
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
                            documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, "Pasta inicial");
                            Log newlog = new Log(documentsModel.Name, 1);
                            await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                            documentsModel.DocumentType = ".pdf";
                            documentsModel.RootFolder = "Pasta inicial";
                            documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                            documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                            documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                            documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                            documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);


                            await _firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);
                            await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial ", "Ok");
                        }
                        else
                        {

                            documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, AppData.CurrentFolder);
                            Log newlog = new Log(documentsModel.Name, 1);
                            await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                            documentsModel.DocumentType = ".pdf";
                            documentsModel.RootFolder = AppData.CurrentFolder;
                            documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                            documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                            documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                            documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                            documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                            await _firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
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

                        documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, fileResult.FileName, "Pasta inicial");
                        Log newlog = new Log(fileResult.FileName, 1);
                        await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                        documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                        documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                        documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                        documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                        documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);

                        await _firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);

                        await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial", "Ok");
                    }
                    else
                    {

                        documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, fileResult.FileName, AppData.CurrentFolder);
                        Log newlog = new Log(fileResult.FileName, 1);
                        await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                        documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                        documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                        documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                        documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                        documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);

                        await _firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
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
    public bool IsRunningAsAdministrator()
    {
        using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
        {
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;


    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}

