using EasyPaperWork.ViewModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;

namespace EasyPaperWork.Views;
 public partial class Page_Document_Upload : ContentPage
{
	private UploadDocsViewModel viewModel1;

    private string UidUser;
  
    public Page_Document_Upload()
    {
        InitializeComponent();
        BindingContext = viewModel1 = new UploadDocsViewModel();

       
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
     
    }


}