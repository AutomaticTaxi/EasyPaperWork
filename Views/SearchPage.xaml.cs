using EasyPaperWork.ViewModel;

namespace EasyPaperWork.Views;

public partial class SearchPage : ContentPage
{
    private SearchViewModel viewModel;
    public SearchPage()
	{
		InitializeComponent();
        BindingContext = viewModel = new SearchViewModel();
    }
}