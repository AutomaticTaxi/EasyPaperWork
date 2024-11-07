using EasyPaperWork.Models;
using EasyPaperWork.Security;
using EasyPaperWork.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Windows.Input;


namespace EasyPaperWork.ViewModel;

public class Main_ViewModel_Files : INotifyPropertyChanged
{


    public ObservableCollection<Documents> DocumentCollection { get; set; }
    private Documents documentsModel;
    private Scanner scanner;
    public ICommand BtSearchFile { get; }
    public ICommand BtHome { get; }
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

    public string LabelTituloRepositorio
    {
        get { return _LabelTituloRepositorio; }
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
    private bool _IsVisibleGifLoading;
    public bool IsVisibleGifLoading
    {
        get { return _IsVisibleGifLoading; }
        set
        {
            _IsVisibleGifLoading = value;
            OnPropertyChanged(nameof(IsVisibleGifLoading));
        }
    }
    private bool _IsVisibleDocumentCollection;
    public bool IsVisibleDocumentCollection
    {
        get => _IsVisibleDocumentCollection;
        set
        {
            _IsVisibleDocumentCollection = value;
            OnPropertyChanged(nameof(IsVisibleDocumentCollection));
        }
    }
    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; }
        set
        {
            UidUser = value;
            OnPropertyChanged(nameof(_UidUser));
        }
    }

    private Folder_Files Folder_Files;
    public Main_ViewModel_Files()
    {
        BtSearchFile = new Command(async () => await SearchFile());
        BtRefresh = new Command(async () => await list_files(AppData.CurrentFolder));
        BtHome = new Command(async () => homefolder());
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
        DocumentCollection = new ObservableCollection<Documents>();

    }

    public async Task list_files(string currentfolder)
    {
        IsVisibleDocumentCollection = false;
        IsVisibleGifLoading = true;
        try
        {
            if (!string.IsNullOrEmpty(UidUser))
            {

                Debug.WriteLine("banco on");

                AppData.Salt = encryptData.GetSaltBytes(await _firebaseService.GetSalt(AppData.UserUid));

                AppData.Key = encryptData.GetKey(AppData.Salt, AppData.UserPassword);
                key = encryptData.GetKey(AppData.Salt, AppData.UserPassword);
                userModel = await _firebaseService.BuscarUserModelAsync("Users", UidUser);
                Debug.WriteLine(userModel.Id);

                DocumentCollection.Clear();
                DocumentCollection.Add(new Documents { Name = "Adicione um item" });



                if (string.IsNullOrEmpty(currentfolder))
                {
                    LabelTituloRepositorio = "Pasta inicial";
                    AppData.CurrentFolder = "Pasta inicial";
                    AppData.listdocs = await _firebaseService.ListFiles(AppData.CurrentFolder);
                    List<Documents> decryptedDocs = new List<Documents>();
                    foreach (Documents doc in AppData.listdocs)
                    {
                        Documents decryptedDoc = new Documents();

                        decryptedDoc.Name = encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt);
                        if (string.IsNullOrEmpty(doc.DocumentType))
                        {
                            decryptedDoc.DocumentType = "folder";
                        }
                        else
                        {
                            decryptedDoc.DocumentType = encryptData.Decrypt(doc.DocumentType, AppData.Key, AppData.Salt);
                        }

                        decryptedDocs.Add(decryptedDoc);
                    }
                    foreach (Documents doc in decryptedDocs)
                    {
                        DocumentCollection.Add(doc);
                    }
                    List<Folder_Files> list_folder = new List<Folder_Files>();
                    list_folder = await _firebaseService.ListFolder("Users", AppData.UserUid, AppData.CurrentFolder);
                    foreach (Folder_Files folder in list_folder)
                    {
                        Documents decrypt_folder = new Documents
                        {
                            //Name = encryptData.Decrypt(folder.Name, key, AppData.Salt)
                            Name = folder.Name,
                            DocumentType = "folder"
                        };
                        DocumentCollection.Add(decrypt_folder);
                    }
                }
                else
                {
                    try
                    {
                        LabelTituloRepositorio = currentfolder;

                        if (!currentfolder.Contains("/"))
                        {
                            AppData.listdocs = await _firebaseService.ListFiles(currentfolder);
                        }
                        else
                        {
                            string[] pathParts = currentfolder.Split("/");
                            List<string> duplicatedParts = new List<string>();
                            foreach (string part in pathParts)
                            {
                                duplicatedParts.Add(part);  // Adiciona o item original
                                duplicatedParts.Add(part);  // Adiciona a duplicata
                            }
                            duplicatedParts.RemoveAt(duplicatedParts.Count - 1);

                            string lastFolder = string.Join("/", duplicatedParts);

                            AppData.listdocs = await _firebaseService.ListFiles(lastFolder);
                        }

                        List<Documents> decryptedDocs = new List<Documents>();
                        foreach (Documents doc in AppData.listdocs)
                        {
                            Documents decryptedDoc = new Documents();

                            decryptedDoc.Name = encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt);
                            if (string.IsNullOrEmpty(doc.DocumentType))
                            {
                                decryptedDoc.DocumentType = "folder";
                            }
                            else
                            {
                                decryptedDoc.DocumentType = encryptData.Decrypt(doc.DocumentType, AppData.Key, AppData.Salt);
                            }

                            decryptedDocs.Add(decryptedDoc);
                        }
                        foreach (Documents doc in decryptedDocs)
                        {
                            DocumentCollection.Add(doc);
                        }
                        List<Folder_Files> list_folder = new List<Folder_Files>();
                        list_folder = await _firebaseService.ListFolder("Users", AppData.UserUid, AppData.CurrentFolder);
                        foreach (Folder_Files folder in list_folder)
                        {
                            Documents decrypt_folder = new Documents
                            {
                                //Name = encryptData.Decrypt(folder.Name, key, AppData.Salt)
                                Name = folder.Name,
                                DocumentType = "folder"
                            };
                            DocumentCollection.Add(decrypt_folder);
                        }
                    }
                    catch (Exception ex)
                    {
                        await Application.Current.MainPage.DisplayAlert("error", ex.ToString(), "ok");
                    }
                }


            }
            else { Debug.WriteLine("banco off"); }
        }
        finally
        {
            IsVisibleDocumentCollection = true;
            IsVisibleGifLoading = false;
        }
    }

    public async void OnDocumentItemTapped(Documents item)
    {
        if (item == null)
        {
            Debug.WriteLine("Item is null");
            return;
        }

        Debug.WriteLine($"Item selecionado: {item.Name}");
        if (item.Name.Equals("Adicione um item"))
        {
            if (string.IsNullOrEmpty(AppData.CurrentFolder))
            {
                AppData.CurrentFolder = "Pasta inicial";
            }
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Adicionar arquivo do PC", "Scannear arquivo", "Adicionar pasta");
            switch (action)
            {
                case "Adicionar arquivo do PC":
                    if (IsRunningAsAdministrator())
                    {
                        await Application.Current.MainPage.DisplayAlert("Ação não permitida", "Re inicie o programa em modo não administrador", "Ok");
                        item = null;
                    }
                    else
                    {
                        await PickAndShowFileAsync(null, null, null);
                        item = null;
                    }

                    break;
                case "Scannear arquivo":
                    await ScanFileAsync();

                    break;
                case "Adicionar pasta":
                    string nameFolder = await Application.Current.MainPage.DisplayPromptAsync("Adição de pasta", "Insira o nome da pasta", "Ok");
                    AddFolder(nameFolder, 1);
                    break;
                default:

                    break;
            }
        }
        else if (item.DocumentType == "folder")
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Abrir", "Excluir");

            switch (action)
            {
                case "Abrir":
                    AppData.CurrentFolder = nextfolder(AppData.CurrentFolder, item.Name);
                    LabelTituloRepositorio = item.Name;
                    await list_files(AppData.CurrentFolder);
                    break;
                case "Excluir":

                    await DeleteFolder(item);
                    break;
                default:
                    break;
            }

        }
       
        else
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Download", "Visualizar", "Adicionar versão", "Excluir");

            switch (action)
            {
                case "Download":
                    await DownloadFile(item, 1);
                    break;
                case "Visualizar":
                    await VisualizarArquivo(item);
                    break;
                case "Adicionar versão":
                    CreateVersion(item, 1);
                    break;
               
                case "Excluir":
                    await DeleteFile(item);
                    break;
                default:
                    break;
            }
        }
    }

    private async void AddFolder(string namefolder, int option)
    {
        if (option == 1)
        {
            if (!string.IsNullOrEmpty(namefolder) && !string.Equals("Adicione uma pasta", namefolder))
            {
                Folder_Files.Name = encryptData.Encrypt(namefolder, key, AppData.Salt);
                Documents Decrypt_folder = new Documents
                {
                    Name = namefolder,
                    DocumentType = "folder"
                };
                DocumentCollection.Add(Decrypt_folder);

                await _firebaseService.AddFolder("Users", AppData.UserUid, AppData.CurrentFolder, Folder_Files.Name, Folder_Files);

            }
            else { await Application.Current.MainPage.DisplayAlert("Erro", "Nome Inválido", "Ok"); }
        }
        else if (option == 2)
        {
            namefolder = string.Concat(namefolder, "Versions");
            Folder_Files.Name = encryptData.Encrypt(namefolder, key, AppData.Salt);
            Documents Decrypt_folder = new Documents
            {
                Name = namefolder,
                DocumentType = "folder"
            };
            DocumentCollection.Add(Decrypt_folder);

            await _firebaseService.AddFolder("Users", AppData.UserUid, AppData.CurrentFolder, Folder_Files.Name, Folder_Files);
        }
    }

    private async Task<string> DownloadFile(Documents selectedItem, int option)
    {
        if (option == 1)
        {
            try
            {
                Debug.WriteLine($"Downloading file {selectedItem.Name}");
                byte[] fileBytes = null;
                if (string.IsNullOrEmpty(AppData.CurrentFolder))
                {
                    Log newlog = new Log(selectedItem.Name, 4);

                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                    if (AppData.CurrentFolder.Contains("/"))
                    {
                        AppData.listdocs = await _firebaseService.ListFiles(AppData.CurrentFolder);
                    }
                    else
                    {
                        string[] pathParts = AppData.CurrentFolder.Split("/");
                        List<string> duplicatedParts = new List<string>();
                        foreach (string part in pathParts)
                        {
                            duplicatedParts.Add(part);  // Adiciona o item original
                            duplicatedParts.Add(part);  // Adiciona a duplicata
                        }
                        duplicatedParts.RemoveAt(duplicatedParts.Count - 1);

                        string lastFolder = string.Join("/", duplicatedParts);

                        AppData.listdocs = await _firebaseService.ListFiles(lastFolder);
                    }

                    foreach (Documents doc in AppData.listdocs)
                    {
                        if (encryptData.Decrypt(doc.Name, key, AppData.Salt) == selectedItem.Name)
                        {
                            fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, "Pasta inicial", doc.Name);
                            break;
                        }
                    }
                }
                else
                {
                    Log newlog = new Log(selectedItem.Name, 4);
                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                    if (!AppData.CurrentFolder.Contains("/"))
                    {
                        AppData.listdocs = await _firebaseService.ListFiles(AppData.CurrentFolder);
                    }
                    else
                    {
                        string[] pathParts = AppData.CurrentFolder.Split("/");
                        List<string> duplicatedParts = new List<string>();
                        foreach (string part in pathParts)
                        {
                            duplicatedParts.Add(part);  // Adiciona o item original
                            duplicatedParts.Add(part);  // Adiciona a duplicata
                        }
                        duplicatedParts.RemoveAt(duplicatedParts.Count - 1);

                        string lastFolder = string.Join("/", duplicatedParts);

                        AppData.listdocs = await _firebaseService.ListFiles(lastFolder);
                    }

                    foreach (Documents doc in AppData.listdocs)
                    {
                        if (encryptData.Decrypt(doc.Name, key, AppData.Salt) == selectedItem.Name)
                        {
                            fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, AppData.CurrentFolder, doc.Name);
                            break;
                        }
                    }

                }
                if (fileBytes != null)
                {
                    string path = await service.PickFolderAsync();
                    string CompletedPath = Path.Combine(path, $"{selectedItem.Name}");
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
                        
                        return CompletedPath;
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
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
                return "error";

            }

        }
        else if (option == 2)
        {
            try
            {
                Debug.WriteLine($"Downloading file {selectedItem.Name}");
                byte[] fileBytes = null;
                if (string.IsNullOrEmpty(AppData.CurrentFolder))
                {
                    Log newlog = new Log(selectedItem.Name, 4);

                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                    List<Documents> list = await _firebaseService.ListFiles("Pasta inicial");
                    foreach (Documents doc in list)
                    {
                        if (encryptData.Decrypt(doc.Name, key, AppData.Salt) == selectedItem.Name)
                        {
                            fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, "Pasta inicial", doc.Name);
                            break;
                        }
                    }
                }
                else
                {
                    Log newlog = new Log(selectedItem.Name, 4);
                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                    List<Documents> list = await _firebaseService.ListFiles( AppData.CurrentFolder);
                    foreach (Documents doc in list)
                    {
                        if (encryptData.Decrypt(doc.Name, key, AppData.Salt) == selectedItem.Name)
                        {
                            fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(AppData.UserUid, AppData.CurrentFolder, doc.Name);
                            break;
                        }
                    }

                }
                if (fileBytes != null)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string CompletedPath = Path.Combine(path, $"{selectedItem.Name}");
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
                        return CompletedPath;
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
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
                return "error";

            }


        }
        else
        {
            return "Option inválida";
        }
    }
    private async Task<string> DeleteFile(Documents selectedItem)
    {

        string action = await Application.Current.MainPage.DisplayActionSheet("Deseja remover o documento", "cancel", null, "sim");
        if (action == "sim")
        {
            foreach (Documents doc in AppData.listdocs)
            {
                if (encryptData.Decrypt(doc.Name, key, AppData.Salt) == selectedItem.Name)
                {
                    if (string.IsNullOrEmpty(AppData.CurrentFolder))
                    {


                        if (await _firebaseService.DeleteFileAsync("Pasta inicial", doc.Name))
                        {
                            if (await _firebaseStorageService.DeleteFileAsync(AppData.UserUid, "Pasta inicial", doc.Name))
                            {
                                Log newlog = new Log(selectedItem.Name, 2);

                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                                DocumentCollection.Remove(selectedItem);
                                await list_files("Pasta inicial");
                                Console.WriteLine("Arquivo removido");
                                return "success";
                                
                            }//Adicionar algoritimo para solucionar caso a exclusão falar em umas das partes 
                        }
                    }
                    else if (await _firebaseService.DeleteFileAsync(AppData.CurrentFolder, doc.Name))
                    {
                        if (await _firebaseStorageService.DeleteFileAsync(AppData.UserUid, AppData.CurrentFolder, doc.Name))
                        {

                            Log newlog = new Log(selectedItem.Name, 2);

                            await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                            DocumentCollection.Remove(selectedItem);
                            await list_files(AppData.CurrentFolder);
                            Console.WriteLine("Arquivo removido");
                            return "success";
                        }
                    }//Adicionar algoritimo para solucionar caso a exclusão falar em umas das partes 
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
    private async Task DeleteFolder(Documents selectedItem)
    {
        if (await _firebaseStorageService.DeleteFolderAsync(selectedItem.Name, AppData.CurrentFolder))
        {
            if (await _firebaseService.DeleteFolderAsync(string.Concat(AppData.CurrentFolder, "/", selectedItem.Name),selectedItem.Name))
                DocumentCollection.Remove(selectedItem);
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

    private async Task ScanFileAsync()
    {
        try
        {
            IsVisibleDocumentCollection = false;
            IsVisibleGifLoading = true;
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
                            await encryptData.EncryptFile(filepath, PathTemporaryEncryptFile, AppData.UserPassword, AppData.Salt);
                            var stream = File.Open(PathTemporaryEncryptFile, FileMode.Open);
                            if (string.IsNullOrEmpty(AppData.CurrentFolder))
                            {
                                documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                                documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, "Pasta inicial");
                                Log newlog = new Log(documentsModel.Name, 1);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                                documentsModel.DocumentType = ".pdf";
                                documentsModel.RootFolder = "Pasta inicial";

                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                                documentsModel.UploadTime = encryptData.Encrypt(DateTime.UtcNow.ToString(), key, AppData.Salt);

                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);
                                Documents doc = new Documents();
                                doc.Name = encryptData.Decrypt(documentsModel.Name, key, AppData.Salt);
                                doc.Image = encryptData.Decrypt(documentsModel.Image, key, AppData.Salt);
                                DocumentCollection.Add(doc);
                                await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial ", "Ok");
                            }
                            else
                            {
                                documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                                documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, AppData.CurrentFolder);
                                Log newlog = new Log(documentsModel.Name, 1);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);
                                documentsModel.DocumentType = ".pdf";
                                documentsModel.RootFolder = AppData.CurrentFolder;

                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                                documentsModel.UploadTime = encryptData.Encrypt(DateTime.UtcNow.ToString(), key, AppData.Salt);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
                                Documents doc = new Documents();
                                doc.Name = encryptData.Decrypt(documentsModel.Name, key, AppData.Salt);
                                doc.Image = encryptData.Decrypt(documentsModel.Image, key, AppData.Salt);
                                DocumentCollection.Add(doc);
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
        finally
        {
            IsVisibleDocumentCollection = true;
            IsVisibleGifLoading = false;
        }
    }
    private async Task PickAndShowFileAsync(string pathfile, string folder, Documents doc)
    {

        try
        {
            if (string.IsNullOrEmpty(pathfile) || string.IsNullOrEmpty(folder))
            {
                await Application.Current.MainPage.DisplayAlert("Aviso", "O arquivos mantém em seu Pc", "Ok");

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

                    if (DocumentCollection.Count > 0)
                    {
                        Documents tempdoc = new Documents();
                        tempdoc.Name = fileResult.FileName;
                        tempdoc.DocumentType = documentsModel.DocumentType;
                        if (fileResult.FileName.Contains(".docx") || fileResult.FileName.Contains(".doc"))
                        {
                            tempdoc.DocumentType = ".docx";
                        }
                        if (fileResult.FileName.Contains(".pdf"))
                        {
                            tempdoc.DocumentType = ".pdf";
                        }
                        if (fileResult.FileName.Contains(".xls") || fileResult.FileName.Contains(".xlsx"))
                        {
                            tempdoc.DocumentType = ".xlsx";
                        }
                        if (fileResult.FileName.Contains(".pptx"))
                        {
                            tempdoc.DocumentType = ".pptx";
                        }
                        bool exists = DocumentCollection.Any(doc => doc.Name == tempdoc.Name && doc.DocumentType == tempdoc.DocumentType);
                        if (exists)
                        {
                            //A rezolver 
                            try
                            {
                                var actions = await Application.Current.MainPage.DisplayActionSheet(
                        "Este documento já existe", "Cancelar", null, "Criar versionamento", "Renomear");
                                switch (actions)
                                {
                                    case "Criar versionamento":
                                        CreateVersion(tempdoc, 1);



                                        break;
                                    case "Renomear":
                                        await ScanFileAsync();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                await Application.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
                            }

                        }
                        IsVisibleDocumentCollection = false;
                        IsVisibleGifLoading = true;
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


                            await encryptData.EncryptFile(fileResult.FullPath, PathTemporaryEncryptFile, AppData.UserPassword, AppData.Salt);

                            var stream = File.Open(PathTemporaryEncryptFile, FileMode.Open, FileAccess.ReadWrite);
                            if (string.IsNullOrEmpty(AppData.CurrentFolder))
                            {
                                List<Folder_Files> listFolder = new List<Folder_Files>();
                                listFolder = await _firebaseService.ListFolder("Users", AppData.UserUid, encryptData.Encrypt("Pasta inicial", key, AppData.Salt));
                                documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                                documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                                documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, "Pasta inicial");
                                Log newlog = new Log(fileResult.FileName, 1);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);

                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.UploadTime = encryptData.Encrypt(DateTime.UtcNow.ToString(), key, AppData.Salt);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Pasta inicial", documentsModel.Name, documentsModel);
                                Documents doc2 = new Documents();
                                doc2.Name = encryptData.Decrypt(documentsModel.Name, key, AppData.Salt);
                                doc2.Image = encryptData.Decrypt(documentsModel.Image, key, AppData.Salt);
                                DocumentCollection.Add(doc);
                                DateTime.UtcNow.ToString();
                                await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial", "Ok");
                            }
                            else
                            {
                                documentsModel.Name = encryptData.Encrypt(fileResult.FileName, key, AppData.Salt);
                                documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, AppData.CurrentFolder);
                                Log newlog = new Log(fileResult.FileName, 1);
                                await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);

                                documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                                documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                                documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                                documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                                documentsModel.UploadTime = encryptData.Encrypt(DateTime.UtcNow.ToString(), key, AppData.Salt);

                                await _firebaseService.AddFiles("Users", AppData.UserUid, AppData.CurrentFolder, documentsModel.Name, documentsModel);
                                Documents doc2 = new Documents();
                                doc2.Name = encryptData.Decrypt(documentsModel.Name, key, AppData.Salt);
                                doc2.Image = encryptData.Decrypt(documentsModel.Image, key, AppData.Salt);
                                DocumentCollection.Add(doc);
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
            }
            else
            {



                string PathTemporaryEncryptFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{doc.Name}");


                IsVisibleDocumentCollection = false;
                IsVisibleGifLoading = true;
                documentsModel.Name = doc.Name;
                if (doc.Name.Contains(".docx") || doc.Name.Contains(".doc"))
                {
                    documentsModel.DocumentType = ".docx";
                }
                if (doc.Name.Contains(".pdf"))
                {
                    documentsModel.DocumentType = ".pdf";
                }
                if (doc.Name.Contains(".xls") || doc.Name.Contains(".xlsx"))
                {
                    documentsModel.DocumentType = ".xlsx";
                }
                if (doc.Name.Contains(".pptx"))
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
                if (!string.IsNullOrEmpty(doc.Name) && !string.Equals("Adicone um documento", doc.Name))
                {


                    await encryptData.EncryptFile(pathfile, PathTemporaryEncryptFile, AppData.UserPassword, AppData.Salt);

                    FileStream stream = new FileStream(pathfile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);


                    List<Folder_Files> listFolder = new List<Folder_Files>();
                    listFolder = await _firebaseService.ListFolder("Users", AppData.UserUid, encryptData.Encrypt(folder, key, AppData.Salt));
                    Log newlog = new Log(documentsModel.Name, 1);
                    documentsModel.Name = encryptData.Encrypt(documentsModel.Name, key, AppData.Salt);
                    documentsModel.UrlDownload = await _firebaseStorageService.UploadFileAsync(stream, documentsModel.Name, folder);

                    await _firebaseService.AddFiles("Users", AppData.UserUid, "Logs", newlog.menssage, newlog);

                    documentsModel.RootFolder = encryptData.Encrypt(documentsModel.RootFolder, key, AppData.Salt);
                    documentsModel.DocumentType = encryptData.Encrypt(documentsModel.DocumentType, key, AppData.Salt);
                    documentsModel.Image = encryptData.Encrypt(documentsModel.Image, key, AppData.Salt);
                    documentsModel.UrlDownload = encryptData.Encrypt(documentsModel.UrlDownload, key, AppData.Salt);
                    documentsModel.UploadTime = encryptData.Encrypt(DateTime.UtcNow.ToString(), key, AppData.Salt);
                    await _firebaseService.AddFiles("Users", AppData.UserUid, folder, documentsModel.Name, documentsModel);
                    await Application.Current.MainPage.DisplayAlert("Succsses", "Aquivo enviado para Pasta inicial", "Ok");

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
        finally
        {
            IsVisibleGifLoading = false;
            IsVisibleDocumentCollection = true;
        }


    }
    public async void CreateVersion(Documents doc, int option)
    {
        if (option == 1)
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
                string documentPathforVersion = fileResult.FullPath;
                AddFolder(doc.Name, 2);
                string path = await DownloadFile(doc, 1);
                AppData.CurrentFolder = nextfolder(AppData.CurrentFolder, string.Concat(doc.Name, "Versions"));

                LabelTituloRepositorio = string.Concat(doc.Name, "Versions");
                await list_files(AppData.CurrentFolder);
                await PickAndShowFileAsync(null, null, null);


            }


        }




    }
    public string nextfolder(string previous, string pathfinal)
    {
        if (!previous.Contains("/"))
        {
            return string.Concat(previous, "/", pathfinal);
        }
        else
        {
            string[] pathParts = previous.Split("/");
            string lastFolder = pathParts[pathParts.Length - 1];
            return string.Concat(previous, "/", pathfinal);
        }



    }
    public string backfolder(string path)
    {
        int indexUltimaBarra = path.LastIndexOf('/');
        string newpath = path.Substring(0, indexUltimaBarra);
        return newpath;
    }
    public async void homefolder()
    {
        AppData.CurrentFolder = "Pasta inicial";
        list_files(AppData.CurrentFolder);
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

