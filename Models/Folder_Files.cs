﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Models
{
   public class Folder_Files : INotifyPropertyChanged
    {
        private string _Name;
        public string Name
        {

        get { return _Name; } 
        set{
            _Name = value;
                OnNotifyPropertyChanged(nameof(Name));
            }
        }
        private Array _DocumentsVinculeted;
        public Array DocumentsVinculeted
        {
            get { return _DocumentsVinculeted; }
            set { _DocumentsVinculeted = value;
                OnNotifyPropertyChanged(nameof(DocumentsVinculeted)); }
        }

        public Folder_Files() { }
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnNotifyPropertyChanged(string propertyName) {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
