using EasyPaperWork.Models;
using EasyPaperWork.ViewModel;
using System.Diagnostics;
using System.Web;


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
        viewModel.Initialize();

    }
    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        base.OnNavigatingFrom(args);
        viewModel.Initialize();
    }
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = e.CurrentSelection.FirstOrDefault() as Documents;
        if (selectedItem != null)
        {
            // Chame o método para manipular a seleção
            viewModel.OnItemTapped(selectedItem);
        }
    }

}