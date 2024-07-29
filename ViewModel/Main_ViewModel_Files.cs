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
  
    private FirebaseService _firebaseService;
    public ObservableCollection<FolderModel> FolderCollection { get; set; }
    private string UidUser;
    public string _UidUser
    {
        get { return UidUser; } 
        set { 
            UidUser = HttpUtility.UrlDecode( value);
            OnPropertyChanged();
            Initialize();
        }
    }


    public Label Label { get; set; }
 
    

    public  Main_ViewModel_Files()
    {
        _firebaseService = new FirebaseService();
        atulizarPage();
     
        
        FolderCollection = new ObservableCollection<FolderModel>
            {
                new FolderModel { Name = "Pasta 1" }
            };
        DocumentCollection = new ObservableCollection<Documents>
            {
                new Documents { Name = "Documento 1", Description = "Description for document 1", DocumentType = ".pdf" },
                new Documents { Name = "Documento 2", Description = "Description for document 2", DocumentType = ".pdf" },
                new Documents { Name = "Documento 3", Description = "Description for document 3", DocumentType = ".pdf" },
            };
        Debug.WriteLine(UidUser);
        //_firebaseService.BuscarDocumentoByIdAsync("Users", UidUser.ToString());

    }
    public void Initialize()
    {
        if (!string.IsNullOrEmpty(_UidUser))
        {
            Debug.WriteLine("Uid = " + _UidUser);
        }
        else
        {
            Debug.WriteLine("Uid é null");
        }
    }
    public void TestBanco()
    {
        if (!string.IsNullOrEmpty(_UidUser))
        {
            Debug.WriteLine("banco on");
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


