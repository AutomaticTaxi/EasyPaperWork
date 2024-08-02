using EasyPaperWork.ViewModel;
using System.Diagnostics;
using System.Web;

namespace EasyPaperWork.Views;

public partial class Main_Page_Files : ContentPage
{
    private Main_ViewModel_Files viewModel;
    /*private string userId;
    public string UserId
    {
        get => userId;
        set
        {
            userId = value;
            PropagateUserId();
        }
    }*/
    public Main_Page_Files()
    {
       
            InitializeComponent();
        BindingContext = viewModel = new Main_ViewModel_Files();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
       
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.Initialize();
        //UserId= viewModel.PassarUidContentPage();
    }
    /*private void PropagateUserId()
    {
        if (!string.IsNullOrEmpty(UserId))
        {
            Debug.WriteLine($"UserId in Main_Page_Files: {UserId}");
            MessagingCenter.Send(this, "UserIdReceived", UserId);
        }
    }
   */
}