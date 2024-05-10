using System.ComponentModel;

namespace EasyPaperWork.Models
{
    public class User : INotifyPropertyChanged {
        private int _id; private string _name; private string _email; private string _password; private string _emailVerified;
        public int Id { get { return _id; }
            set { _id = value; OnPropertyChanged(nameof(_id)); } }
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(nameof(_name)); } }
        public string Email { get { return _email;} set { _email = value; OnPropertyChanged(nameof(_email)); } }
        public string Password { get { return _password; } set { _password = value; OnPropertyChanged(nameof(_password)); } } 
        public string EmailVerified { get { return _emailVerified; }set { _emailVerified = value; OnPropertyChanged(nameof(_emailVerified)); } }  
       
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
  

    }
}
