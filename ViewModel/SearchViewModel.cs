using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        
        }
        private async Task SearchArchive()
        {

        }
        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
