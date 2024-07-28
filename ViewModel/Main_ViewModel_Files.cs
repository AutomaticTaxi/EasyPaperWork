using EasyPaperWork.Models;
using EasyPaperWork.Services;
using Firebase.Auth;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Web;

namespace EasyPaperWork.ViewModel;

[QueryProperty(nameof(UserUid),"texto")]
    public class Main_ViewModel_Files : INotifyPropertyChanged
    {
        public ObservableCollection<Documents> DocumentCollection { get; set; }
        
        public ObservableCollection<FolderModel> FolderCollection { get; set; }
        private string _UserUid;
        public string UserUid
    {
            get { return _UserUid; }
            set { _UserUid = HttpUtility.UrlDecode(value); }
        }
        public FirebaseAuthServices authServices;
        public FirebaseService firebaseService;
        public UserModel userModel;

        public Main_ViewModel_Files()
        {
        authServices = new FirebaseAuthServices();
            firebaseService = new FirebaseService();
            userModel = new UserModel();
       
            
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

        var id = UserUid;

        BuscarUser(id);

    }

   
    public async void BuscarUser(string id)
    {
        await firebaseService.BuscarDocumentoByIdAsync("Users", id);
    }
    
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

