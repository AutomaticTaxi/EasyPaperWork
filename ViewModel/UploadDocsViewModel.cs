using EasyPaperWork.Models;
using EasyPaperWork.Services;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel
{

    public class UploadDocsViewModel: INotifyPropertyChanged
    {
        private Main_ViewModel_Files main_ViewModel_Files;
        private Documents documentsModel;
        private FirebaseService firebaseService;
        private FireBaseStorageService firebaseStorageService;
        private SharedViewModel sharedViewModel;
        public ICommand PickFileCommand { get; }
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
            _UidUser =AppData.UserUid;
            Initialize();   
            documentsModel = new Documents();
            firebaseService = new FirebaseService();
            PickFileCommand = new Command(async () => await PickAndShowFileAsync());
            firebaseStorageService=new FireBaseStorageService(apiKey: AppData.UserUid);

        }
        private async Task PickAndShowFileAsync()
        {

            var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.iOS, new[] { "com.adobe.pdf", "org.openxmlformats.wordprocessingml.document", "org.openxmlformats.spreadsheetml.sheet", "org.openxmlformats.presentationml.presentation", "com.microsoft.word.doc" } }, // UTType values for iOS
                    { DevicePlatform.Android, new[] { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/vnd.openxmlformats-officedocument.presentationml.presentation", "application/msword" } }, // MIME type
                    { DevicePlatform.WinUI, new[] { ".pdf", ".docx", ".doc", ".xls", ".xlsx", ".pptx" } }, // file extensions for WinUI
                    { DevicePlatform.Tizen, new[] { "*/*" } },
                    { DevicePlatform.macOS, new[] { "pdf", "docx", "doc", "xls", "xlsx", "pptx" } } // UTType values for macOS
                });

            PickOptions options = new PickOptions
            {
                PickerTitle = "Selecione um arquivo",
                FileTypes = customFileType,
            };

            try
            {
                var result = await FilePicker.Default.PickAsync(options);
                if (result != null)
                {
                    var stream = await result.OpenReadAsync();
                    SelectedFileName = result.FileName;
                    documentsModel.Name = result.FileName;
                    documentsModel.DocumentType = result.ContentType;
                    Debug.WriteLine(result.ContentType);
                    await firebaseService.AdicionarDocumentoNaMainPageFiles("Users", _UidUser, "Documents", documentsModel.Name, documentsModel);
                    await Shell.Current.DisplayAlert("Resultado", "Documento enviado com sucesso", "Ok");
                    
                    await firebaseStorageService.UploadFileAsync(stream, result.FileName);
                    
                    
                    // Você pode adicionar lógica adicional para processar o arquivo, se necessário
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao selecionar arquivo: {ex.Message}");
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
