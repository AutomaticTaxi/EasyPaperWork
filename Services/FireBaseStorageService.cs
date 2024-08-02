using EasyPaperWork.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using FirebaseAdmin.Auth;
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
        private readonly FirebaseAuthClient _firebaseAuthClient;
        private FirebaseAuthProvider _firebaseAuthProvider;
        
        public FireBaseStorageService(string apiKey)
        {
            
            _firebaseStorage = new FirebaseStorage( "easypaperwork-firebase");

            var config= new FirebaseAuthConfig
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
                _firebaseAuthClient = new FirebaseAuthClient(config);
           
            }
            catch (Exception ex) { Debug.WriteLine(ex.ToString()); }


        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            try
            {
                // Autenticar o usuário
                var auth = await _firebaseAuthClient.SignInWithEmailAndPasswordAsync(AppData.UserEmail, AppData.UserPassword);
                var options = new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(auth.User.Credential.IdToken),
                    ThrowOnCancel = true
                };

                // Upload do arquivo
                var task = _firebaseStorage
                    
                    .Child(fileName)
                    .PutAsync(fileStream);

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
