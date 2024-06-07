using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{

    class Documents : INotifyPropertyChanged
    {
        private Guid _Id { get; set; }
        public Guid Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
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
        private string _DocumentType { get; set; }
        public string DocumentType
        {
            get { return _DocumentType; }
            set
            {
                _DocumentType = value;
                OnPropertyChanged(nameof(DocumentType));
            }
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



