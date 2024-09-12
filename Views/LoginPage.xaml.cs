
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
    protected override void OnAppearing()
    {
        base.OnAppearing();
        MakeFullScreen();

    }
    private void MakeFullScreen()
    {
        // Defina a janela para tela cheia
#if WINDOWS
            var window = (Microsoft.UI.Xaml.Window)MauiWinUIApplication.Current.Application.Windows[0].Handler.PlatformView;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd));

            var displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            var fullScreenSize = displayArea.WorkArea;
            windowId.MoveAndResize(new Windows.Graphics.RectInt32(0, 0, fullScreenSize.Width, fullScreenSize.Height));
#endif
    }
}