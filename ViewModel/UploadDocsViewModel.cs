using Castle.Components.DictionaryAdapter.Xml;
using EasyPaperWork.Models;
using EasyPaperWork.Services;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;

using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel
{

    public class UploadDocsViewModel: INotifyPropertyChanged
    {
        
        private Documents documentsModel;
        public ICommand PickFileCommand { get; }
        private FirebaseStorageService storageService;
        private FirebaseService firebaseService;
        private string _selectedFileName;
        public string SelectedFileName
        {
            get => _selectedFileName;
            set
            {
                _selectedFileName = value;
                OnPropertyChanged(nameof(SelectedFileName));
            }
        }
        private string UidUser;
        public string _UidUser
        {
            get { return UidUser; }
            set
            {
                UidUser = HttpUtility.UrlDecode(value);
                OnPropertyChanged(nameof(_UidUser));
               
            }
        }

        public UploadDocsViewModel()
        {
            storageService =  new FirebaseStorageService();
            firebaseService = new FirebaseService();
            _UidUser =AppData.UserUid;
            Initialize();   
            documentsModel = new Documents();
            PickFileCommand = new Command(async () => await PickAndShowFileAsync());

           
        }
        private async Task PickAndShowFileAsync()
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Por favor selecione um arquivo",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.WinUI, new[] { ".pdf", ".docx", ".doc", ".xls", ".xlsx", ".pptx" } },
                    { DevicePlatform.Android, new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/msword", "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.openxmlformats-officedocument.presentationml.presentation" } },
                    { DevicePlatform.iOS, new[] { "com.adobe.pdf", "org.openxmlformats.wordprocessingml.document", "com.microsoft.word.doc", "com.microsoft.excel.xls", "org.openxmlformats.spreadsheetml.sheet", "org.openxmlformats.presentationml.presentation" } }
                })
            });

            if (fileResult != null)
            {
                var stream = File.Open(fileResult.FullPath, FileMode.Open);
                documentsModel.Name = fileResult.FileName;
                if (fileResult.FileName.Contains(".docx")|| fileResult.FileName.Contains(".doc"))
                {
                    documentsModel.DocumentType = ".docx";
                }if (fileResult.FileName.Contains(".pdf"))
                {
                    documentsModel.DocumentType = ".pdf";
                }
                if (fileResult.FileName.Contains(".xls") || fileResult.FileName.Contains(".xlsx")){
                    documentsModel.DocumentType = ".xlsx";
                }
                if (fileResult.FileName.Contains(".pptx")){
                    documentsModel.DocumentType = ".pptx";
                }
                if (AppData.CurrentFolder != null)
                {
                    documentsModel.RootFolder = AppData.CurrentFolder;
                }else if (AppData.CurrentFolder == null)
                {
                    documentsModel.RootFolder = "Main_Page_Files";   
                }

                documentsModel.UrlDownload= await storageService.UploadFileAsync(stream, fileResult.FileName);
                await firebaseService.AddFiles("Users",AppData.UserUid,AppData.CurrentFolder,documentsModel.Name,documentsModel);


            }
        }

      
        public void Initialize()
        {
            if (!string.IsNullOrEmpty(_UidUser))
            {
                Debug.WriteLine("Recebeu UId");
            }
            else {
                Debug.WriteLine("Não Recebeu UId");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
