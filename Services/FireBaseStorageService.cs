using EasyPaperWork.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public class FireBaseStorageService
    {
        private readonly FirebaseStorage _firebaseStorage;
        private readonly FirebaseAuthProvider _authProvider;
        public FireBaseStorageService()
        {
           
                _authProvider = new FirebaseAuthProvider(new FirebaseAuthConfig()
                {
                  
                   
                });
             

           
            _firebaseStorage = new FirebaseStorage("gs://easypaperwork-firebase.appspot.com");
        }
        public async Task<string> UploadFileAsync( Stream fileStream, string fileName)
        {
            try
            {
                // Autenticar o usuário
                var auth = _authProvider.SignInWithEmailAndPasswordAsync(AppData.UserEmail.ToString, AppData.UserPassword.ToString);
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken),
                    ThrowOnCancel = true
                };


                // Upload do arquivo
                var task = _firebaseStorage
                    .Child("uploads")
                    .Child(fileName)
                    .PutAsync(fileStream, options);

                // Monitorar progresso
                task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progresso: {e.Percentage} %");

                // Esperar o upload terminar
                var downloadUrl = await task;

                return downloadUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
                throw;
            }
        }
    }
}
