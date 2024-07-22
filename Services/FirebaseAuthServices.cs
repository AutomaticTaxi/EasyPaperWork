using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public class FirebaseAuthServices
    {
        private FirebaseAuthClient _authClient;
        private string UserTokenID { get; set; }
    public FirebaseAuthServices()
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
            _authClient = new FirebaseAuthClient(config);
        }
        public async void SignInButton_Click(string email,string password)
        {
            try
            {

                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Aqui você pode lidar com o usuário logado (userCredential.User)


                // Exemplo de como obter um token de ID
                var idToken = await userCredential.User.GetIdTokenAsync();
                UserTokenID=idToken.ToString();
                Console.WriteLine("ID Token: " + idToken.ToString());
                Debug.WriteLine("Logado com sucesso");
            
            }
            catch (FirebaseAuthException ex)
            {
                Debug.WriteLine("Falha ao logar", ex.ToString);
            }
        }
        public async void RegiterUser(string email,string password,string name)
        {
            await _authClient.CreateUserWithEmailAndPasswordAsync(email,password,name);
        }
       
        

        public void SignOutButton_Click()
        {
            // Para deslogar o usuário
            _authClient.SignOut();

        }
    }
}
