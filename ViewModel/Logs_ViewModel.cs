using EasyPaperWork.Models;
using EasyPaperWork.Services;
using System.Collections.ObjectModel;

namespace EasyPaperWork.ViewModel
{
    public class Logs_ViewModel
    {
        public ObservableCollection<Log> LogsCollection { get; set; }
        private FirebaseService firebaseService;

        public Logs_ViewModel()
        {
            firebaseService = new FirebaseService();
            LogsCollection = new ObservableCollection<Log>();

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

    }

}
