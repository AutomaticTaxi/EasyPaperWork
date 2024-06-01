using EasyPaperWork.Models;
using EasyPaperWork.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.AppCompat;
using System.Windows.Input;
using NavigationPage = Microsoft.Maui.Controls.NavigationPage;

namespace EasyPaperWork.ViewModel
{ 
    public class LoginPageViewModel
    {
        public ICommand LoginCommand { get; set; }
        public ICommand CadastroCommand { get; set; }
      
        private async void Login()
        {
            NavigationPage navigationPage = (NavigationPage)App.Current.MainPage;
            navigationPage.PushAsync(new PageSetor());
        }
        private async void OpenCadastro()
        {
            NavigationPage navigationPage = (NavigationPage)App.Current.MainPage;
            navigationPage.PushAsync(new Page_Register_User());
        }



    }
}
