
using EasyPaperWork.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class FirebaseStorageService
{
   

    private FirebaseAuthClient _authClient;
    private UserCredential userCredential;
    private readonly HttpClient _httpClient;
    public FirebaseStorageService()
    {
        _httpClient = new HttpClient();
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
    public async void CreateFolderAsync(string NameofFolder) {
        var emptyBytes = new byte[0];
        userCredential = await _authClient.SignInWithEmailAndPasswordAsync(AppData.UserEmail, AppData.UserPassword);
        var task = new FirebaseStorage(
            "easypaperwork-firebase.appspot.com",

             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken),
                 ThrowOnCancel = true,
             }).Child(NameofFolder).PutAsync(new MemoryStream(emptyBytes));
    } 
    public async Task<string> UploadFileAsync(FileStream stream, string fileName,string RootFolder )
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

              .Child(AppData.UserUid)
              .Child(RootFolder)
                .Child(fileName)
                .PutAsync(stream);

        // Track progress of the upload
        task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");

        // await the task to wait until upload completes and get the download url
        var downloadUrl = await task;
        return downloadUrl;

    }
    public async Task<bool> DeleteFolderAsync(string RootFolder)
    {
        try
        {
            // Referência à pasta
            userCredential = await _authClient.SignInWithEmailAndPasswordAsync(AppData.UserEmail, AppData.UserPassword);

            // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
            var task = new FirebaseStorage(
                "easypaperwork-firebase.appspot.com",

                 new FirebaseStorageOptions
                 {
                     AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken),
                     ThrowOnCancel = true,
                 })

                  .Child(AppData.UserUid)
                    .Child(RootFolder).DeleteAsync();
            return true;



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao remover a pasta '{RootFolder}': {ex.Message}");
            return false;
        }
    }
    public async Task<bool> DeleteFileAsync(string userid, string RootFolder ,string fileName )
    {

        try
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

                  .Child(AppData.UserUid)
                    .Child(RootFolder)
                        .Child(fileName)
                        .DeleteAsync();
            return true;
        }catch(Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return false;
        }

    }

    public async Task<byte[]> DownloadFileByNameAsync( string fileName)
    {
        try
        {
          
            userCredential = await _authClient.SignInWithEmailAndPasswordAsync(AppData.UserEmail, AppData.UserPassword);

            var task = new FirebaseStorage(
            "easypaperwork-firebase.appspot.com",

             new FirebaseStorageOptions
             {
                 AuthTokenAsyncFactory = () => Task.FromResult(userCredential.User.Credential.IdToken),
                 ThrowOnCancel = true,
             })

              .Child("uploads")
            .Child(fileName);
            // Faz o download do arquivo e o converte para um array de bytes
          var fileBytes = await DownloadFileByUrlAsync(await task.GetDownloadUrlAsync());

            return fileBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao fazer download do arquivo: {ex.Message}");
            throw;
        }

    }
    public async Task<byte[]> DownloadFileByUrlAsync(string downloadUrl)
    {
        try
        {
            // Baixa o arquivo da URL fornecida
            var fileBytes = await _httpClient.GetByteArrayAsync(downloadUrl);
            return fileBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao baixar o arquivo: {ex.Message}");
            throw;
        }
    }
}
