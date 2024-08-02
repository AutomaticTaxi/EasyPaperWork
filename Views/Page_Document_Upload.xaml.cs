using EasyPaperWork.ViewModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Web;

namespace EasyPaperWork.Views;
 public partial class Page_Document_Upload : ContentPage
{
	private UploadDocsViewModel viewModel;
    private string UidUser;
  
    public Page_Document_Upload()
    {
        InitializeComponent();
        BindingContext = viewModel = new UploadDocsViewModel();
       
    }
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
     
    }


}