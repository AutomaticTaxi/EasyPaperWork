using Firebase.Storage;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class FirebaseStorageService
{
    private readonly string _storageBucket;
  

    public FirebaseStorageService(string storageBucket, string apiKey)
    {
        _storageBucket = storageBucket;

    }

    public async Task<string> UploadFileAsync(string filePath, string email, string password)
    {
        var cts = new CancellationTokenSource();
        var stream = File.Open(filePath, FileMode.Open);

        // Authenticate user and get token


        var storage = new FirebaseStorage(
            _storageBucket,
            new FirebaseStorageOptions
            {

                ThrowOnCancel = true // optional, default is false
            });

        try
        {
            // Construct FirebaseStorage with path to where you want to upload the file and put it there
            var storageRef = storage.Child("dumentTest").Child(Path.GetFileName(filePath));
            var downloadUrl = await storageRef.PutAsync(stream, cts.Token);
            return downloadUrl;
        }
        catch (Exception ex)
        {
            throw new Exception("Erro no upload: " + ex.Message);
        }
    }
}
