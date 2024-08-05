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
                    case "application/pdf":
                        Image = "pdficon.png";
                        break;
                    case "application/msword":
                        Image = "wordfileicon.png";
                        break;
                    case "application/octet-stream":
                        Image = "excelicon.png";
                        break;
                    case "pptx":
                        Image = "powerpointicon.png";
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



