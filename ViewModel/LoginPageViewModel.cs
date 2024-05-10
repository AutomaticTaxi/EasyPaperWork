using EasyPaperWork.Models;
using EasyPaperWork.Views;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.AppCompat;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel
{ 
    public class LoginPageViewModel
    {
        public ICommand LoginCommand { get; set; }
        public User User { get; set; }
        public LoginPageViewModel() {
            User = new User();
            LoginCommand = new Command(async()=>await Login());

        } 
        private async Task Login()
        {
            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(new PageSetor());
        }
       
            
        
    }
}
