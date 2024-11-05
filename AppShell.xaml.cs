using EasyPaperWork.Views;

namespace EasyPaperWork;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("Page_Login", typeof(LoginPage));
        Routing.RegisterRoute("Page_Register_User", typeof(Page_Register_User));
        Routing.RegisterRoute("Main_Page_Files", typeof(Main_Page_Files));


    }
}