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
        /// 
        private FirebaseAuthClient _authClient;
        private UserCredential userCredential;



        /// 
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
            PickFileCommand = new Command(async () => await receber_arq());

            //////////////
            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCIHw3fP1XoNiuIZK6eNs0LIwi1SDDAyao",
                AuthDomain = "easypaperwork-firebase.firebaseapp.com",
                Providers = new Firebase.Auth.Providers.FirebaseAuthProvider[]
                      {
                    new EmailProvider()
                      },
                UserRepository = new FileUserRepository("Users")
            };
            try
            {
                _authClient = new FirebaseAuthClient(config);
            }
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
            /////////////////

        }
        private async Task PickAndShowFileAsync()
        {

           
        }
        public async Task<string>  receber_arq()
        {

            // Get any Stream - it can be FileStream, MemoryStream or any other type of Stream
            var stream = File.Open(@"C:\Users\lucas\Downloads\images.jpeg", FileMode.Open);

            //authentication

             userCredential = await _authClient.SignInWithEmailAndPasswordAsync(AppData.UserEmail, AppData.UserPassword);

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                "easypaperwork-firebase.appspot.com",
            
                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken),
                     ThrowOnCancel = true,
                 })
               
                .Child("uploads")
                .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
            return downloadUrl;

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
