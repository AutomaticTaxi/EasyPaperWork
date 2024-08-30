
using EasyPaperWork.Services;
using Org.BouncyCastle.Cms;

namespace EasyPaperWork.Views;

public partial class LoginPage : ContentPage
{
    private FirebaseAuthServices firebaseAuthServices;
    public LoginPage()
    {
        firebaseAuthServices = new FirebaseAuthServices();
        InitializeComponent();

    }

    public void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        OpenRegistrationPage();
    }
    public void LabelEsqueceuSenhaTapped(object sender, TappedEventArgs e)
    {
        OpenEsqueceuSenha();
    }
    public async Task OpenRegistrationPage()
    {
        await Application.Current.MainPage.Navigation.PushAsync(new Page_Register_User());
      
    }
    public async Task OpenEsqueceuSenha()
    {
        string action = await Application.Current.MainPage.DisplayActionSheet("Esqueceu a senha ?", "Cancelar", null, "Sim");
        string EntryText = EntryEmail.Text;
        switch (action)
        {
            case "Sim":
                string result = await firebaseAuthServices.ChangingUserPassword(EntryText);
                switch (result)
                {
                    case "MissingEmail":
                        await Application.Current.MainPage.DisplayAlert("Error", "Email não foi digitado", "Ok");
                        break;
                    case "ResetPasswordExceedLimit":
                        await Application.Current.MainPage.DisplayAlert("Error", "Muitas tentativas foram feitas, aguarde", "Ok");
                        break;
                    case "InvalidEmailAddress":
                        await Application.Current.MainPage.DisplayAlert("Error", "Email iválido", "Ok");
                        break;
                    default:
                        await Application.Current.MainPage.DisplayAlert("Succsses", "Enviaremos um e-mail para troca da senha", "Ok");
                        break;
                }

                break;
        }
     

    }
}