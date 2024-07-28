
namespace EasyPaperWork.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();

    }

    public void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        OpenRegistrationPage();
    }
    public async Task OpenRegistrationPage()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new Page_Register_User());
      
    } 
}