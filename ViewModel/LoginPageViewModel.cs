using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Firebase.Auth;
using Firebase.Auth.Providers;
using System.Windows;


using Firebase.Auth.Repository;
using Microsoft.Maui.Controls;
using EasyPaperWork.Views;
using EasyPaperWork.Services;

namespace EasyPaperWork.ViewModel
{
    public class LoginPageViewModel : INotifyPropertyChanged
    {
        public string EntryEmail { get; set; }
        public string EntryPassword { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand CadastroCommand { get; set; }
        private FirebaseAuthServices _fireBaseAuthServices;

        public LoginPageViewModel()
        {
            LoginCommand = new Command(Login);

            _fireBaseAuthServices = new FirebaseAuthServices();
          
        }
      

     

        public event PropertyChangedEventHandler PropertyChanged;


        private async void Login()
        {
            // Exemplo básico de navegação para uma nova página
            _fireBaseAuthServices.SignInButton_Click(EntryEmail, EntryPassword);
        }

      
    }
}

