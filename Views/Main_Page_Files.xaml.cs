using EasyPaperWork.ViewModel;

namespace EasyPaperWork.Views;

public partial class Main_Page_Files : ContentPage
{
    private Main_ViewModel_Files viewModel;
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
    }
}