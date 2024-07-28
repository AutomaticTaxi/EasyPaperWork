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
            EntryEmail = "automatictaxi2@gmail.com";
            EntryPassword = "Abajur.857";

            _fireBaseAuthServices = new FirebaseAuthServices();
          
        }
      

     

        public event PropertyChangedEventHandler PropertyChanged;


        private async void Login()
        {

            // Exemplo básico de navegação para uma nova página
            string UserUid = await _fireBaseAuthServices.GetUidToken(EntryEmail, EntryPassword);
            if (UserUid!="error")
            {

                await Application.Current.MainPage.DisplayAlert("Success", "Logado com sucesso","ok");
               

                Shell.Current.GoToAsync($"//Main_Page_Files?texto={UserUid}");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Eror", "Falha ao logar verifique seu email e senha ", "ok");
            }
           
        }

      
    }
}

