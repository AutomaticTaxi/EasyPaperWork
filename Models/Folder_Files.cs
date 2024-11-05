using System.ComponentModel;

namespace EasyPaperWork.Models
{
    public class Folder_Files : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {

            get { return _Name; }
            set
            {
                _Name = value;
                if (_Name == "Adicione uma pasta")
                {
                    Image = "add_folder.png";
                }
                else
                {
                    Image = "folder_icon.png";
                }
                OnNotifyPropertyChanged(nameof(Name));
            }
        }
        private string _Image;
        public string Image
        {
            get => _Image;
            set
            {
                _Image = value;
                OnNotifyPropertyChanged(nameof(Image));
            }
        }
        private Array _DocumentsVinculeted;
        public Array DocumentsVinculeted
        {
            get { return _DocumentsVinculeted; }
            set
            {
                _DocumentsVinculeted = value;
                OnNotifyPropertyChanged(nameof(DocumentsVinculeted));
            }
        }

        public Folder_Files()
        {

        }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
