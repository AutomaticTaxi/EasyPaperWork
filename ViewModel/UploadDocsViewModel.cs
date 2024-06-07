using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EasyPaperWork.ViewModel
{
    public class UploadDocsViewModel
    {
        public ICommand DragStartingCommand { get; }
        public ICommand DroppingCommand { get; }
        public UploadDocsViewModel()
        {
            DragStartingCommand = new Command(OnDragStarting);
            DroppingCommand = new Command(OnDropping);
        }

        private void OnDragStarting(object obj)
        {
            // Lógica para o início do arrastar
        }

        private void OnDropping(object obj)
        {
            // Lógica para o evento de soltar
        }
    }
}
