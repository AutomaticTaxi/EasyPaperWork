
using EasyPaperWork.Models;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Storage;
using System.Diagnostics;
using EasyPaperWork.Services;
using EasyPaperWork.Security;

public class FirebaseStorageService
{
    private FirebaseAuthClient _authClient;
    private UserCredential userCredential;
    private FirebaseService firebaseService;
    private EncryptData _encryptData;
    private HttpClient _httpClient;
    public FirebaseStorageService()
    {
        _httpClient = new HttpClient();
        _encryptData = new EncryptData();
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
        firebaseService =new FirebaseService();
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
                    .PutAsync(stream);

            // Track progress of the upload
            task.Progress.ProgressChanged += (s, e) => Debug.WriteLine($"Progress: {e.Percentage} %");

            // await the task to wait until upload completes and get the download url
            var downloadUrl = await task;
            return downloadUrl;
        }
        catch (Exception ex)
        {
            
            await Application.Current.MainPage.DisplayAlert("Error", $"Erro ao enviar para o servidor de arquivo {ex.Message}","Ok");
            return "error";
        }

    }
    public async Task<bool> DeleteFolderAsync(string folderName, string pathfolder)
    {
        try
        {
            List<Documents> documents = new List<Documents>();
            documents = await ListFilesInFolderAsync(string.Concat(pathfolder,"/",folderName));
            foreach (Documents doc in documents) {
                if (_encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt).Equals(doc.Name)) {
                    await DeleteFileAsync(AppData.UserUid, pathfolder, doc.Name);
                }
               
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Erro ao remover a pasta '{folderName}': {ex.Message}");
            return false;
        }
    }
    public async Task<List<Documents>> ListFilesInFolderAsync(string folderPath)
    {
        List<Documents> listOfFiles = new List<Documents>();
        string[] pathParts = folderPath.Split("/");
        List<string> duplicatedParts = new List<string>();
        foreach (string part in pathParts)
        {
            duplicatedParts.Add(part);  // Adiciona o item original
            duplicatedParts.Add(part);  // Adiciona a duplicata
        }
        duplicatedParts.RemoveAt(duplicatedParts.Count - 1);

        string lastFolder = string.Join("/", duplicatedParts);

        listOfFiles = await firebaseService.ListFiles("Users",AppData.UserUid,lastFolder);
        return listOfFiles;
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

    public async Task<byte[]> DownloadFileByNameAsync(string uid,string filepath, string fileName)
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

              .Child(uid)
              .Child (filepath)
            .Child(fileName);
            // Faz o download do arquivo e o converte para um array de bytes
          var fileBytes = await DownloadFileByUrlAsync(await task.GetDownloadUrlAsync());
            if (fileBytes != null)
            {

                return fileBytes;
            }
            else
            {
                return null;
            }

        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error",$"Erro ao fazer download do arquivo: {ex.Message}","ok");
            return null;
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
            await Application.Current.MainPage.DisplayAlert("Error",$"Erro ao baixar o arquivo: {ex.Message}","Ok");
            return null;
        }
    }
}
