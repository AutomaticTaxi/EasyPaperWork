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
using System.Web;
using System.Text.Encodings.Web;
using EasyPaperWork.Models;
using EasyPaperWork.Security;

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
            EntryEmail = "contact.lsadecoration@gmail.com";
            EntryPassword = "Abajur.857";

            _fireBaseAuthServices = new FirebaseAuthServices();
          
        }
      

     

   

        private async void Login()
        {

            // Exemplo básico de navegação para uma nova página
            string UserUid = await _fireBaseAuthServices.GetUidToken(EntryEmail, EntryPassword);
            switch (UserUid)
            {
                case "UnknownEmailAddress":
                    await Application.Current.MainPage.DisplayAlert("Erro", "O não está vinculado a nenhuma conta", "ok");
                    break;
                case "InvalidEmailAddress":
                    await Application.Current.MainPage.DisplayAlert("Erro", "O email é inválido", "ok");
                    break;
                case "MissingEmail":
                    await Application.Current.MainPage.DisplayAlert("Erro", "O Email não foi digitado", "ok");
                    break;
                case "MissingPassword":
                    await Application.Current.MainPage.DisplayAlert("Erro", "A senha não foi digitada", "ok");
                    break;
                case "WrongPassword":
                    await Application.Current.MainPage.DisplayAlert("Erro", "A senha está incorreta", "ok");
                    break;
                case "TooManyAttemptsTryLater":
                    await Application.Current.MainPage.DisplayAlert("Erro", "Muitas tentativas feitas", "ok");

                    break;
                case "error":
                    await Application.Current.MainPage.DisplayAlert("Erro", "Um erro inesperado aconteceu, tente novamente ", "ok");
                    break;
                default:
                 
                    AppData.UserEmail = EntryEmail;
                    AppData.UserPassword = EntryPassword;
                    AppData.UserUid = UserUid;
                    
                    Shell.Current.GoToAsync("//mainTabBar/Main_Page_Files");
                    break;
            }
         
           
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }



    }
}

