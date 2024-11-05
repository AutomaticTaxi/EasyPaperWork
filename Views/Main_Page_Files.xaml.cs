using EasyPaperWork.Models;
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
        //viewModel.Initialize();


    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        //viewModel.Initialize();

    }
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.list_files(AppData.CurrentFolder);

    }
    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);

    }
    private void OnSelectionDocumentChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as Documents;

        if (selectedItem != null)
        {
            // Chame o m�todo para manipular a sele��o
            viewModel.OnDocumentItemTapped(selectedItem);

        }
    }


}