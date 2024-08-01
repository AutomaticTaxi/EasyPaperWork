using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.Generic;

using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.Maui.Controls.PlatformConfiguration;
using EasyPaperWork.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EasyPaperWork.Services
{
   public class FirebaseService
    {
        private readonly FirestoreDb _firestoreDb;
        public FirebaseService()
        {
                //Credecial colocada direto no códico F segurança 
            string pathToCredentials = "C:\\Users\\lucas\\source\\repos\\AutomaticTaxi\\EasyPaperWork\\easypaperwork-firebase-firebase-adminsdk-4asf7-dc3b16ccbd.json"; 
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredentials);
            _firestoreDb = FirestoreDb.Create("easypaperwork-firebase"); // Substitua pelo ID do seu projeto Firebase
        }
        public async Task AdicionarDocumentoAsync(string colecao, string documentoId, Dictionary<string, object> dados)
        {
            try
            {
                if (string.IsNullOrEmpty(colecao))
                    throw new ArgumentException("A coleção não pode ser nula ou vazia.", nameof(colecao));

                if (string.IsNullOrEmpty(documentoId))
                    throw new ArgumentException("O ID do documento não pode ser nulo ou vazio.", nameof(documentoId));

                if (dados == null)
                    throw new ArgumentException("Os dados não podem ser nulos.", nameof(dados));

                DocumentReference document = _firestoreDb.Collection(colecao).Document(documentoId);
                await document.SetAsync(dados);
                Debug.WriteLine("Documento adicionado com sucesso!");
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task PrintBuscarDocumentoByIdAsync(string colecao, string documentoId)
        {
            DocumentReference document = _firestoreDb.Collection(colecao).Document(documentoId);
            DocumentSnapshot snapshot = await document.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> dados = snapshot.ToDictionary();
                foreach (var item in dados)
                {
                    Debug.WriteLine($"{item.Key}: {item.Value}");
                }
            }
            else
            {
                Debug.WriteLine("Documento não encontrado.");
            }
        }
        public async Task BuscarDocumentosPorCampoAsync(string colecao, string campo, object valor)
        {
            try
            {
                CollectionReference collection = _firestoreDb.Collection(colecao);
            Query query = collection.WhereEqualTo(campo, valor);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
          
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    Dictionary<string, object> dados = documentSnapshot.ToDictionary();
                    Console.WriteLine($"Documento ID: {documentSnapshot.Id}");
                    foreach (var item in dados)
                    {
                        Console.WriteLine($"{item.Key}: {item.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async Task<UserModel> BuscarUserModelAsync(string colecao, string documentId)
        {
            try
            {
                DocumentReference document = _firestoreDb.Collection(colecao).Document(documentId);
                DocumentSnapshot snapshot = await document.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Dictionary<string, object> dados = snapshot.ToDictionary();
                    var userModel = new UserModel
                    {
                        Id = dados.ContainsKey("Id") ? dados["Id"].ToString():string.Empty,
                        Name = dados.ContainsKey("Name") ? dados["Name"].ToString() : string.Empty,
                        Email = dados.ContainsKey("Email") ? dados["Email"].ToString() : string.Empty,
                        Password = dados.ContainsKey ("Password")? dados["Password"].ToString():string.Empty,
                        AccountType = dados.ContainsKey("AccountType") ? dados["AccountType"].ToString():string.Empty
                       
                        
                    };
                    Debug.WriteLine(userModel);
                    return userModel;
                }
                else
                {
                    Debug.WriteLine("Documento não encontrado.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<Documents> BuscarDocumentosNaMainPageFilesAsync(string ColecaoUser, string IdUser,string ColecaoDocument,string IdDocument)
        {
            try
            {
                DocumentReference document = _firestoreDb.Collection(ColecaoUser).Document(IdUser).Collection(ColecaoDocument).Document(IdDocument);
                DocumentSnapshot snapshot = await document.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Dictionary<string, object> dados = snapshot.ToDictionary();
                    var documentsModel = new Documents
                    {
                        Name = dados.ContainsKey("Name") ? dados["Name"].ToString() : string.Empty,
                        DocumentType = dados.ContainsKey("DocumentType") ? dados["DocumentType"].ToString() : string.Empty


                    };
                    Debug.WriteLine(documentsModel.ToString());
                    return documentsModel;
                }
                else
                {
                    Debug.WriteLine("Documento não encontrado.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Documents>> ListarDocumentosNaMainPageFilesAsync(string colecaoUser,string IdUser,string CollectionDocument)
        {
            try
            {
                if (string.IsNullOrEmpty(colecaoUser))
                    throw new ArgumentException("A coleção não pode ser nula ou vazia.", nameof(colecaoUser));

                CollectionReference collectionRef = _firestoreDb.Collection(colecaoUser).Document(IdUser).Collection(CollectionDocument);
                QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

                List<Documents> documentosList = new List<Documents>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    var documents = new Documents()
                    {
                        Name = document.ContainsField("Name") ? document.GetValue<string>("Name") : null,
                        DocumentType = document.ContainsField("DocumentType") ? document.GetValue<string>("DocumentType") : null
                        // Adicione outros campos conforme necessário
                    };

                    documentosList.Add(documents);
                }

                return documentosList;
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
        public async Task AdicionarDocumentoNaMainPageFiles<T>(string ColecaoUser, string IdUser, string ColecaoDocument, string IdDocument, T objetoDocumento)
        {
            try {
                Dictionary<string, object> dados = ConvertToDictionary(objetoDocumento);
                DocumentReference document = _firestoreDb.Collection(ColecaoUser).Document(IdUser).Collection(ColecaoDocument).Document(IdDocument);
                await document.SetAsync(dados);

            } catch (ArgumentException ex) { Debug.WriteLine(ex.Message); }
        }

        public async Task AdicionarObjetoAsync<T>(string colecao, string documentoId, T objeto)
        {
            Dictionary<string, object> dados = ConvertToDictionary(objeto);
            await AdicionarDocumentoAsync(colecao, documentoId, dados);
        }

        private Dictionary<string, object> ConvertToDictionary<T>(T objeto)
        {
            // Converte o objeto para um dicionário usando JsonConvert
            string json = JsonConvert.SerializeObject(objeto);
            Debug.Write(json);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
        }
    }
}
