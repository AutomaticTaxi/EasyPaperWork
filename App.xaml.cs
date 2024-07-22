using EasyPaperWork.Views;
namespace EasyPaperWork

{

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();


            MainPage = new NavigationPage (new LoginPage());
        }

    }
}
