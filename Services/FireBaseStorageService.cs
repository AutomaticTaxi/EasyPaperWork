using EasyPaperWork.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class FirebaseStorageService
{
   

    private FirebaseAuthClient _authClient;
    private UserCredential userCredential;
    public FirebaseStorageService()
    {
      
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

    }

    public async Task<string> UploadFileAsync(FileStream stream,string fileName)
    {
       

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
                .Child(fileName)
                .PutAsync(stream);

        // Track progress of the upload
        task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");

        // await the task to wait until upload completes and get the download url
        var downloadUrl = await task;
        return downloadUrl;

    }
}
