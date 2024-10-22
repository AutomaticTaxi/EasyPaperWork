using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{

    public class Documents : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                if(_Name == "Adicone um documento")
                {
                    Image = "add_file.png";
                }
                
                OnPropertyChanged(nameof(_Name));
            }
        }
       
        private string _DocumentType;
        public string DocumentType
        {
            get { return _DocumentType; }
            set
            {
                _DocumentType = value;
                switch (DocumentType)
                {
                    case ".pdf":
                        Image = "pdf_icon.png";
                        break;
                    case ".docx":
                        Image = "doc_icon.png";
                        break;
                    case ".xlsx":
                        Image = "xlsx_icon.png";
                        break;
                    case ".pptx":
                        Image = "ppt_icon.png";
                        break;
                    case "folder":
                        Image = "folder_icon.png";
                        break;

                }
                OnPropertyChanged(nameof(DocumentType));
            }
        }
        private string _Image;
        public string Image
        {
            get => _Image;
            set
            {
                _Image = value;
                OnPropertyChanged(nameof(Image));
            }
        }

        private string _UrlDownload;
        public string UrlDownload
        {
            get
            {
                return _UrlDownload;
            }
            set
            {
                _UrlDownload = value;
                OnPropertyChanged(nameof(UrlDownload));
            }
        }
        private string _UploadTime;
        public string UploadTime
        {
            get
            {
                return _UploadTime;
            }
            set
            {
                _UploadTime = value;
                OnPropertyChanged(nameof(UploadTime));
            }
        }

        private string _RootFolder;
        public string RootFolder
        {
            get
            {
                return _RootFolder;
            }
            set
            {
                _RootFolder = value;
                OnPropertyChanged(nameof(RootFolder));
            }
        }
        public Documents()
        {
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

    }
}



