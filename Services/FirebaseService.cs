using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

namespace EasyPaperWork.Services
{
   public class FirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        public FirebaseService()
        {
                //Credecial colocada direto no códico F segurança 
            string pathToCredentials = "\"C:\\Users\\lucas\\OneDrive\\Documents\\ProjetosC#\\EasyPaperWork\\easypaperwork-firebase-firebase-adminsdk-4asf7-dc3b16ccbd.json\"\"C:\\Users\\lucas\\OneDrive\\Documents\\ProjetosC#\\EasyPaperWork\\easypaperwork-firebase-firebase-adminsdk-4asf7-dc3b16ccbd.json\""; 
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredentials);
            _firestoreDb = FirestoreDb.Create("easypaperwork-firebase"); // Substitua pelo ID do seu projeto Firebase
        }
        public async Task AdicionarDocumentoAsync(string colecao, string documentoId, Dictionary<string, object> dados)
        {
            DocumentReference document = _firestoreDb.Collection(colecao).Document(documentoId);
            await document.SetAsync(dados);
            Console.WriteLine("Documento adicionado com sucesso!");
        }

        public async Task BuscarDocumentoAsync(string colecao, string documentoId)
        {
            DocumentReference document = _firestoreDb.Collection(colecao).Document(documentoId);
            DocumentSnapshot snapshot = await document.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dados = snapshot.ToDictionary();
                foreach (var item in dados)
                {
                    Console.WriteLine($"{item.Key}: {item.Value}");
                }
            }
            else
            {
                Console.WriteLine("Documento não encontrado.");
            }
        }
    }
}
