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
[QueryProperty(nameof(_UidUser),"text")]

public  class Main_ViewModel_Files: INotifyPropertyChanged
{
    public ObservableCollection<Folder_Files> FolderCollection { get; set; }
    public ObservableCollection<Documents> DocumentCollection { get; set; }
    private FirebaseService _firebaseService;
    private string _LabelTituloRepositorio;

    public string LabelTituloRepositorio { get { return _LabelTituloRepositorio; }
        set
        {
            _LabelTituloRepositorio = value;
            OnPropertyChanged();
        }
    }
    private string _LabelNomeDocumento;
    public string LabelNomeDocumento
    {
        get { return _LabelNomeDocumento; }
        set
        {
            _LabelNomeDocumento = value;
            OnPropertyChanged();
        }
    }
    private string _ImageDocumento;
    public string ImageDocumento
    {
        get { return _ImageDocumento; }
        set
        {
            _ImageDocumento = value;
            OnPropertyChanged();
        }
    }
 
    public UserModel _userModel;
    public UserModel userModel {
        get { return _userModel; }
        set { _userModel = value;
            OnPropertyChanged();
        }
    }
    public Documents _Document;
    public Documents Document
    {
        get { return _Document; }
        set
        {
            _Document = value;
            OnPropertyChanged();
        }
    }

    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; } 
        set { 
            UidUser = HttpUtility.UrlDecode( value);
            OnPropertyChanged();
            
        }
    }
    public  Main_ViewModel_Files()
    {
        _firebaseService = new FirebaseService();
        atulizarPage();
        userModel = new UserModel();
        Document = new Documents { Name = "Documento 1", DocumentType = "pdf" };
        _LabelNomeDocumento = Document.Name;
        _ImageDocumento = Document.Image;


        FolderCollection = new ObservableCollection<Folder_Files>
            {
                new Folder_Files { Name = "Pasta 1" }
            };
        DocumentCollection = new ObservableCollection<Documents>
        {
            new Documents{Name="Documento 2",DocumentType="docx"}
        };
        DocumentCollection.Add(Document);
        Debug.WriteLine(UidUser);
        //_firebaseService.BuscarDocumentoByIdAsync("Users", UidUser.ToString());

    }

    public async void Initialize()
    {
        if (!string.IsNullOrEmpty(_UidUser))
        {
            Debug.WriteLine("banco on");
            userModel = await _firebaseService.BuscarUserModelAsync("Users", UidUser);
            Debug.WriteLine($"{userModel.Id}");
            Debug.WriteLine($"{userModel.Name}");
            Debug.WriteLine($"{userModel.AccountType}");
            Debug.WriteLine($"{userModel.Email}");
            Debug.WriteLine($"{userModel.Password}");
            if(userModel.AccountType== "PersonalAccount")
            {
                LabelTituloRepositorio= "Seu repositório pessual ";
              /*  List<Documents> list = await _firebaseService.ListarDocumentosNaMainPageFilesAsync("Users", UidUser,"Documents");
                Debug.WriteLine(list);
                foreach (Documents doc in list) { 
                DocumentCollection.Add(doc);*/
            }else if(userModel.AccountType == "EnterpriseAccount")
            {
                LabelTituloRepositorio = "Repositório empresarial";
            }
            else
            {
                LabelTituloRepositorio = "Setor";
            }
            




        }
        else { Debug.WriteLine("banco off"); }
    }
  
    public void atulizarPage()
    {
        if (UidUser != null)
        {
            Debug.WriteLine("Uid = "+ UidUser);
        }
        else
        {
            Debug.WriteLine("Uid é null");
        }
    }
    
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


