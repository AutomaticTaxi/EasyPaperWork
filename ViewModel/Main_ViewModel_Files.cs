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
 
    public UserModel _userModel;
    public UserModel userModel {
        get { return _userModel; }
        set { _userModel = value;
            OnPropertyChanged(nameof(userModel));
        }
    }
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
            UidUser = HttpUtility.UrlDecode( value);
            OnPropertyChanged();
            atulizarPage();
           
            
        }
    }
    public  Main_ViewModel_Files()
    {
        _firebaseService = new FirebaseService();
        atulizarPage();
        userModel = new UserModel();
        Document = new Documents();
        _LabelNomeDocumento = Document.Name;
        _ImageDocumento = Document.Image;


        FolderCollection = new ObservableCollection<Folder_Files>
            {
                new Folder_Files { Name = "Pasta 1" }
            };
        DocumentCollection = new ObservableCollection<Documents>();
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
            if (userModel.AccountType == "PersonalAccount")
            {
                LabelTituloRepositorio = "Seu repositório pessual ";
                DocumentCollection.Clear();
                List<Documents> list = await _firebaseService.ListarDocumentosNaMainPageFilesAsync("Users", UidUser, "Documents");
                Debug.WriteLine(list);
                foreach (Documents doc in list) {
                    DocumentCollection.Add(doc); 
            }
            if (userModel.AccountType == "EnterpriseAccount")
                {
                    LabelTituloRepositorio = "Repositório empresarial";
                }
                else
                {
                    LabelTituloRepositorio = "Setor";
                } 
            }
            
        }
        else { Debug.WriteLine("banco off"); }
        

    }
    public void atulizarPage()
    {
        if (UidUser != null)
        {
            Debug.WriteLine("Uid = " + UidUser);
        }
        else
        {
            Debug.WriteLine("Uid é null");
        }
    }

    public string PassarUidContentPage()
    {
        if (UidUser != null)
        {
            Debug.WriteLine("Uid passado para content page");
            return UidUser;
           
        }
        return null;
     
    }
    
    public event PropertyChangedEventHandler PropertyChanged;


    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }



}


