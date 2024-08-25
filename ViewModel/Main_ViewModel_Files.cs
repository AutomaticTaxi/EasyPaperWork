using __XamlGeneratedCode__;
using EasyPaperWork.Models;
using EasyPaperWork.Services;
using Microsoft.Maui.Controls.Compatibility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel;

public  class Main_ViewModel_Files: INotifyPropertyChanged
{
    public ObservableCollection<Folder_Files> FolderCollection { get; set; }
    public ObservableCollection<Documents> DocumentCollection { get; set; }
    private FirebaseService _firebaseService;
    private FirebaseStorageService _firebaseStorageService;
    private string _LabelTituloRepositorio;
    private IFileSavePickerService _fileSavePickerService;
    private WindowsFileSavePickerService service;

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
    public Documents _Document;
    public Documents Document
    {
        get { return _Document; }
        set
        {
            _Document = value;
            OnPropertyChanged(nameof(Document));
        }
    }

    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; } 
        set { 
            UidUser = value;
            OnPropertyChanged(nameof(_UidUser));
        }
    }
    public  Main_ViewModel_Files()
    {
        service = new WindowsFileSavePickerService();

        UidUser = AppData.UserUid;
        AppData.CurrentFolder = "Main_Page_Files";
        _firebaseService = new FirebaseService();
        _firebaseStorageService = new FirebaseStorageService();

        userModel = new UserModel();
        Document = new Documents();
        _LabelNomeDocumento = Document.Name;
        _ImageDocumento = Document.Image;
        


        FolderCollection = new ObservableCollection<Folder_Files>
            {
                new Folder_Files { Name = "Adicione uma pasta " }
            };
        DocumentCollection = new ObservableCollection<Documents>();
        list_files();


    }

 public async void list_files()
    {
        if (!string.IsNullOrEmpty(UidUser))
        {

            Debug.WriteLine("banco on");


            userModel = await _firebaseService.BuscarUserModelAsync("Users", UidUser);
            Debug.WriteLine(userModel.Id);

            DocumentCollection.Clear();
            DocumentCollection.Add(new Documents { Name = "Adicone um documento" });
            LabelTituloRepositorio = "Seu repositório pessual ";

            List<Documents> list = await _firebaseService.ListFiles("Users", userModel.Id, AppData.CurrentFolder);
            Debug.WriteLine(list.ToString());
            foreach (Documents doc in list)
            {

                DocumentCollection.Add(doc);

            }
        }
        else { Debug.WriteLine("banco off"); }
    }

    public async void OnItemTapped(Documents item)
    {
        if (item == null)
        {
            Debug.WriteLine("Item is null");
            return;
        }

        Debug.WriteLine($"Item selecionado: {item.Name}");
        if (item.Name == "Adicone um documento")
        {
            AppData.CurrentFolder = "Main_Page_Files";

            Shell.Current.GoToAsync("//mainTabBar/PageUUploadDocs");
        }
        else
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Escolha uma ação", "Cancelar", null, "Download", "Visualizar");

            switch (action)
            {
                case "Download":
                    await DownloadFile(item);
                    break;
                case "Visualizar":
                    await VisualizarArquivo(item);
                    break;
                default:
                    break;
            }
        }
    }
    private async Task<string> DownloadFile(Documents selectedItem)
    {
        Debug.WriteLine($"Downloading file {selectedItem.Name}");
        byte[] fileBytes = await _firebaseStorageService.DownloadFileByNameAsync(selectedItem.Name); // Sua lógica para obter os bytes do arquivo
        string path = await service.SaveFileAsync(fileBytes, selectedItem.Name,selectedItem.DocumentType);

        if (path != null)
        {
            // Arquivo salvo com sucesso
            Console.WriteLine($"Arquivo salvo em: {path}");
        }
        return "ddhfgdhfg";
    }

    private Task VisualizarArquivo(Documents selectedItem)
    {
        // Implementar a lógica para visualização
        return Task.CompletedTask;
    }
    public event PropertyChangedEventHandler PropertyChanged;


    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


