using EasyPaperWork.Models;
using EasyPaperWork.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
namespace EasyPaperWork.ViewModel
{
    public class Logs_ViewModel
    {
        public ObservableCollection<Log> LogsCollection { get; set; }
        private FirebaseService firebaseService;
        private  Log log;
        private WindowsFileSavePickerService windows_service;
        public ICommand Download_logs_Command{ get; }
        public Logs_ViewModel()
        {
            firebaseService = new FirebaseService();
            LogsCollection = new ObservableCollection<Log>();
            windows_service = new WindowsFileSavePickerService();
            log = new Log();
            Download_logs_Command = new Command(async () => await Download_logs(LogsCollection));

        }
        public async void List_LogsAsync()
        {
            LogsCollection.Clear();
            List<Log> listLogs = await firebaseService.ListLogs();
            foreach (Log log in listLogs)
            {
                LogsCollection.Add(log);
            }

        }
        public async Task<string> Download_logs(Collection<Log> logs)
        {
            log.GeneratePdfFromLogs(logs);
            
            return string.Empty;
        }

    }

}
