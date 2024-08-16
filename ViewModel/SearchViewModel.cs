using EasyPaperWork.Models;
using EasyPaperWork.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel
{
    public class SearchViewModel:INotifyPropertyChanged
    {
        private string EntryArchiveName;
        private Documents Documento;
        public ObservableCollection<Documents> DocumentCollection { get; set; }
        public FirebaseService firebaseService;
        public ICommand BtSearch { get; }
        public string _EntryArchiveName {
            get{
                return EntryArchiveName;}
            set {
                EntryArchiveName = value;
                OnPropertyChanged(nameof(EntryArchiveName));
            } 
        }
       

        
        public SearchViewModel() {
            BtSearch = new Command(async () => await SearchArchive());
            DocumentCollection = new ObservableCollection<Documents>();
            Documento = new Documents();
            firebaseService = new FirebaseService();
        }
        private async Task SearchArchive()
        {
            if (EntryArchiveName != null)
               Documento = await firebaseService.BuscarDocumentosNaMainPageFilesAsync("Users", AppData.UserUid, "Documents", EntryArchiveName);
            DocumentCollection.Add(Documento);
        }
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
