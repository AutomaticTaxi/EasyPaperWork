using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Windows;


using Firebase.Auth.Repository;
using Microsoft.Maui.Controls;
using EasyPaperWork.Views;

namespace EasyPaperWork.ViewModel
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public ICommand LoginCommand { get; set; }
        public ICommand CadastroCommand { get; set; }
        private FirebaseAuthClient _authClient;

        public LoginPageViewModel()
        {
            LoginCommand = new Command(Login);
            CadastroCommand = new Command(OpenCadastro);
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
        private async void SignInButton_Click()
        {
            try
            {
                // Exemplo de login com e-mail e senha
                var email = "user@example.com";
                var password = "password123";

                var userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Aqui você pode lidar com o usuário logado (userCredential.User)


                // Exemplo de como obter um token de ID
                var idToken = await userCredential.User.GetIdTokenAsync();
                Console.WriteLine("ID Token: " + idToken);
                Debug.WriteLine("Logado com sucesso");
                await Application.Current.MainPage.Navigation.PushAsync(new PageSetor());
            }
            catch (FirebaseAuthException ex)
            {
                Debug.WriteLine("Falha ao logar",ex.ToString);
            }
        }

        private void SignOutButton_Click()
        {
            // Para deslogar o usuário
            _authClient.SignOut();

        }

        public event PropertyChangedEventHandler PropertyChanged;


        private async void Login()
        {
            // Exemplo básico de navegação para uma nova página
            SignInButton_Click();
        }

        private async void OpenCadastro()
        {
            // Exemplo básico de navegação para uma nova página de cadastro
        }
    }
}

