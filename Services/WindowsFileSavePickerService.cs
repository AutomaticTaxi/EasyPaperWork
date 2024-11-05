#if WINDOWS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

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
        public async Task<string> PickFolderAsync()
        {
            var picker = new FolderPicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;

            // Definir as extensões de arquivo para garantir a compatibilidade com todas as pastas
            picker.FileTypeFilter.Add("*");

            // Inicializar o picker com a janela ativa
            var hwnd = ((MauiWinUIWindow)App.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFolder folder = await picker.PickSingleFolderAsync();

            if (folder != null)
            {
                return folder.Path;  // Retorna o caminho da pasta selecionada
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
            return "Função para windows";
        }
        public async Task<string> PickFolderAsync()
        {
            return "Função para windows";
        }
    }
}
#endif