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

    public ObservableCollection<Documents> DocumentCollection { get; set; }
    private string _LabelTituloRepositorio;
    public string LabelTituloRepositorio { get { return _LabelTituloRepositorio; }
        set
        {
            _LabelTituloRepositorio = value;
            OnPropertyChanged();
        }
    }
  
    private FirebaseService _firebaseService;
    public UserModel _userModel;
    public UserModel userModel {
        get { return _userModel; }
        set { _userModel = value;
            OnPropertyChanged();
        }
    }


    public ObservableCollection<Folder_Files> FolderCollection { get; set; }
    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; } 
        set { 
            UidUser = HttpUtility.UrlDecode( value);
            OnPropertyChanged();
            
        }
    }


    public Label Label { get; set; }
 
    

    public  Main_ViewModel_Files()
    {
        _firebaseService = new FirebaseService();
        atulizarPage();
        userModel = new UserModel();
         
        
        FolderCollection = new ObservableCollection<Folder_Files>
            {
                new Folder_Files { Name = "Pasta 1" }
            };
        DocumentCollection = new ObservableCollection<Documents>
            {
                new Documents { Name = "Documento 1", DocumentType = ".pdf" },
                new Documents { Name = "Documento 2", DocumentType = ".pdf" },
                new Documents { Name = "Documento 3", DocumentType = ".pdf" },
            };
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
                List<Documents> list = await _firebaseService.ListarDocumentosNaMainPageFilesAsync("Users", UidUser, "Documents");
                Debug.WriteLine(list);
                foreach (Documents doc in list) { 
                DocumentCollection.Add(doc);
                }
               
               
            }
            else if(userModel.AccountType == "EnterpriseAccount")
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


