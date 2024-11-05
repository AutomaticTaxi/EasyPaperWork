using EasyPaperWork.Models;
using EasyPaperWork.Security;
using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.Diagnostics;


namespace EasyPaperWork.Services
{
    public class FirebaseService
    {
        private EncryptData _encryptData;
        private readonly FirestoreDb _firestoreDb;
        public FirebaseService()
        {
            //Credecial colocada direto no códico F segurança 
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _encryptData = new EncryptData();

            // Caminho relativo para o arquivo de credenciais
            string relativePath = @"Services\easypaperwork-firebase-firebase-adminsdk-4asf7-dc3b16ccbd.json";

            // Constrói o caminho completo
            string pathToCredentials = Path.Combine(baseDirectory, relativePath);
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToCredentials);
            Debug.WriteLine($"Credential Path: {pathToCredentials}");
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
        public async Task<bool> DeleteFileAsync(string rootfolder, string documentid)
        {
            try
            {
                DocumentReference document = _firestoreDb.Collection("Users").Document(AppData.UserUid).Collection(rootfolder).Document(documentid);
                await document.DeleteAsync();
                return true;
             
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return false;

            }

        }
        public async Task<bool> DeleteFolderAsync(string rootfolder,string folder_name)
        {
            try
            {
                List<Documents> listdocs = new List<Documents>();
                List<Documents> listfolder = new List<Documents>();
                rootfolder = Adjust_path_for_listing_in_folder(rootfolder);
                listdocs = await ListFiles(rootfolder );
                foreach (Documents doc in listdocs)
                {

                    if (_encryptData.Decrypt(doc.DocumentType, AppData.Key, AppData.Salt) != "folder")
                    {
                        await DeleteFileAsync(rootfolder, doc.Name);//remove arquivos dentro da pasta
                        listfolder.Add(doc);//adiciona a uma lista de pastas dento da pasta original
                    }
                }
                rootfolder= Adjust_path_for_listing_once_folder_back(rootfolder);
                listdocs = await ListFiles(rootfolder);
                foreach (Documents doc in listdocs)
                {
                    if(!string.IsNullOrEmpty(doc.Name)){
                        if ( _encryptData.Decrypt(doc.Name, AppData.Key, AppData.Salt) == folder_name)
                        {
                            await DeleteFileAsync(rootfolder, doc.Name);//remove a pasta da pasta raiz

                        }
                    }
                    
                }

                return true;


            }

            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("error", $"Exception: {ex.ToString()}", "Ok");
                return false;
            }

        }
        public string Adjust_path_for_listing_in_folder(string rootfolder)
        {
            string[] pathParts = rootfolder.Split("/");
            List<string> duplicatedParts = new List<string>();
            foreach (string part in pathParts)
            {
                duplicatedParts.Add(part);  // Adiciona o item original
                duplicatedParts.Add(part);  // Adiciona a duplicata
            }
            duplicatedParts.RemoveAt(duplicatedParts.Count - 1);

            string lastFolder = string.Join("/", duplicatedParts);
            return lastFolder;
        }
        public string Adjust_path_for_listing_once_folder_back(string rootfolder)
        {
            string path = Normalize_path(rootfolder);
            if (path.Contains("/"))
            {
                path = backfolder(path);
                path = Adjust_path_for_listing_in_folder(path);
            }
           
            return path;
        }
        public string backfolder(string path)
        {
            int indexUltimaBarra = path.LastIndexOf('/');
            string newpath = path.Substring(0, indexUltimaBarra);
            return newpath;
        }
        public string Normalize_path(string rootfolder)
        {
            string[] pathParts = rootfolder.Split("/");
            List<string> listParts = new List<string>();
            foreach (string part in pathParts)
            {
                if (listParts.Count < 1)
                {
                    listParts.Add(part);
                }
                else
                {
                    if (!pathParts.Contains(part))
                    {
                        listParts.Add(part);
                    }
                }
            }
            

            string lastFolder = string.Join("/", listParts);
            return lastFolder;
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
                        Id = dados.ContainsKey("Id") ? dados["Id"].ToString() : string.Empty,
                        Name = dados.ContainsKey("Name") ? dados["Name"].ToString() : string.Empty,
                        Email = dados.ContainsKey("Email") ? dados["Email"].ToString() : string.Empty,
                        Password = dados.ContainsKey("Password") ? dados["Password"].ToString() : string.Empty,
                        AccountType = dados.ContainsKey("AccountType") ? dados["AccountType"].ToString() : string.Empty



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
                Debug.WriteLine("Documento não encontrado.");
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        public async Task<Documents> BuscarDocumentosNaMainPageFilesAsync(string ColecaoUser, string IdUser, string ColecaoDocument, string IdDocument)
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
                        DocumentType = dados.ContainsKey("DocumentType") ? dados["DocumentType"].ToString() : string.Empty,
                        UploadTime = dados.ContainsKey("UploadTime") ? dados["UploadTime"].ToString() : string.Empty


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
                Debug.WriteLine("Documento não encontrado.");
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Documents>> ListFiles( string CurrentFolder)
        {
            try
            {

                CollectionReference collectionRef = _firestoreDb.Collection("Users").Document(AppData.UserUid).Collection(CurrentFolder);
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
                return new List<Documents>();

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return new List<Documents>();
            }
        }


        public async Task<List<Log>> ListLogs()
        {
            try
            {
                CollectionReference collectionRef = _firestoreDb.Collection("Users").Document(AppData.UserUid).Collection("Logs");
                QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

                List<Log> logList = new List<Log>();

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    var documents = new Log()
                    {
                        menssage = document.ContainsField("menssage") ? document.GetValue<string>("menssage") : null,
                        dateTime = document.ContainsField("dateTime") ? document.GetValue<DateTime?>("dateTime") : null // Alteração para DateTime?

                        // Adicione outros campos conforme necessário
                    };
                    logList.Add(documents);
                }
                return logList;
            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine($"ArgumentException: {ex.Message}");
                return new List<Log>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");
                return new List<Log>();
            }
        }

        public async Task<List<Folder_Files>> ListFolder(string colecaoUser, string IdUser, string caminhoCompleto)
        {
            try
            {
                // Quebra o caminho de subcoleções e documentos (ex: "Pasta1/Pasta2/Pasta3")
                var subPastas = caminhoCompleto.Split('/');

                // Começa acessando a coleção principal
                DocumentReference docRef = _firestoreDb.Collection(colecaoUser).Document(IdUser);

                // Itera através das subcoleções para chegar ao nível desejado
                for (int i = 0; i < subPastas.Length; i++)
                {
                    // Acessa o documento e a subcoleção correspondente no caminho
                    docRef = docRef.Collection(subPastas[i]).Document(subPastas[i]);
                }

                // Pega todas as subcoleções dentro do último documento acessado
                var subcolecoes = docRef.ListCollectionsAsync();

                // Lista as subcoleções encontradas
                Debug.WriteLine("Subcoleções encontradas:");
                var folderlist = new List<Folder_Files>();

                // Use 'await foreach' para iterar através das subcoleções
                await foreach (var colecao in subcolecoes)
                {
                    // Adiciona cada subcoleção à lista de pastas
                    folderlist.Add(new Folder_Files { Name = colecao.Id });
                }

                return folderlist;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar subcoleções: {ex.Message}");
                return null;
            }
        }
        public async Task AddFiles<T>(string colecaoUser, string idUser, string caminhoCompleto, string idDocument, T objFile)
        {
            try
            {
                // Converte o objeto em um dicionário
                Dictionary<string, object> dados = ConvertToDictionary(objFile);

                // Quebra o caminho de subcoleções e documentos (ex: "Pasta1/Pasta2/Pasta3")
                var subPastas = caminhoCompleto.Split('/');

                // Começa acessando a coleção principal
                CollectionReference colecao = _firestoreDb.Collection(colecaoUser).Document(idUser).Collection(subPastas[0]);

                // Itera através das subcoleções, criando o caminho dinamicamente
                for (int i = 1; i < subPastas.Length; i++)
                {
                    // Acessa o próximo documento e subcoleção
                    colecao = colecao.Document(subPastas[i - 1]).Collection(subPastas[i]);
                }

                // Acessa o documento dentro da última subcoleção e insere os dados
                DocumentReference document = colecao.Document(idDocument);
                await document.SetAsync(dados);

                Console.WriteLine("Dados inseridos com sucesso!");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao adicionar arquivo: {ex.Message}");
            }
        }



        public async Task AddFolder<T>(string ColecaoUser, string IdUser, string folder_path, string IdDocument, T objFile)
        {
            try
            {
                Dictionary<string, object> dados = ConvertToDictionary(objFile);
                DocumentReference document = _firestoreDb.Collection(ColecaoUser).Document(IdUser).Collection(folder_path).Document(IdDocument);
                await document.SetAsync(dados);

            }
            catch (ArgumentException ex)
            {
                Debug.WriteLine(ex.Message);
            }
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
        public async Task<string> GetSalt(string Uid)
        {
            try
            {
                DocumentReference document = _firestoreDb.Collection("Users").Document(Uid);
                DocumentSnapshot snapshot = await document.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Dictionary<string, object> dados = snapshot.ToDictionary();
                    var userModel = new UserModel
                    {
                        Id = dados.ContainsKey("Id") ? dados["Id"].ToString() : string.Empty,
                        Name = dados.ContainsKey("Name") ? dados["Name"].ToString() : string.Empty,
                        Email = dados.ContainsKey("Email") ? dados["Email"].ToString() : string.Empty,
                        Password = dados.ContainsKey("Password") ? dados["Password"].ToString() : string.Empty,
                        AccountType = dados.ContainsKey("AccountType") ? dados["AccountType"].ToString() : string.Empty,
                        Salt = dados.ContainsKey("Salt") ? dados["Salt"].ToString() : string.Empty


                    };
                    string salt = userModel.Salt;
                    return salt;
                }
                else
                {
                    Debug.WriteLine("Documento não encontrado.");
                    return "error";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Documento não encontrado.");
                Debug.WriteLine($"Exception: {ex.Message}");
                return "error";
            }

        }
    }
}
