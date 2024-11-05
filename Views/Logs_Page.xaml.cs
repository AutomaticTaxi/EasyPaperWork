using EasyPaperWork.ViewModel;

namespace EasyPaperWork.Views;

public partial class Logs_Page : ContentPage
{
    private Logs_ViewModel viewModel;

    public Logs_Page()
    {
        InitializeComponent();

        BindingContext = viewModel = new Logs_ViewModel();
    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.List_LogsAsync();

    }
}