#if WINDOWS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Diagnostics;

namespace EasyPaperWork.Services
{
    public class WindowsFileSavePickerService 
    {
        public async Task<string> SaveFileAsync(byte[] fileBytes, string suggestedFileName, string fileType)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = suggestedFileName
            };

            picker.FileTypeChoices.Add(fileType, new List<string>() { fileType });

            // Assegure-se de que o aplicativo tenha uma janela ativa (necessário para o MAUI no Windows)
            var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                Debug.WriteLine("Metodo retornou nuull");
                await FileIO.WriteBytesAsync(file, fileBytes);
                return file.Path;

            }

            return null;
        }
    }
}
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public class WindowsFileSavePickerService
    {
        public async Task<string> SaveFileAsync(byte[] fileBytes, string suggestedFileName, string fileType)
        {
            return "Erro";
        }
    } 
}
#endif