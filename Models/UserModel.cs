using System.ComponentModel;
using System.Text.Json;

namespace EasyPaperWork.Models
{
    public class UserModel : INotifyPropertyChanged
    {
        private string _Id;
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }
        private string _Name;
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged(nameof(_Name));
            }
        }
        private string _Email;
        public string Email
        {
            get
            {
                return _Email;
            }
            set
            {
                _Email = value;
                OnPropertyChanged(nameof(Email));
            }
        }
        private string _Password;
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
     private DateTime _DateUserCreated { get; set; }
          public DateTime DateUserCreated
          {
              get
              {
                  return _DateUserCreated;
              }
              set
              {
                  _DateUserCreated = value;
                  OnPropertyChanged(nameof(DateUserCreated));
              }
          }
        private string _Salt { get; set; }
        public string Salt
        {
            get { return _Salt; }
            set
            {
                _Salt = value;
                OnPropertyChanged(nameof(Salt));
            }
        }
        public UserModel JsonToObject(string json)
        {
            UserModel model = new UserModel();

            model = JsonSerializer.Deserialize<UserModel>(json);
            return model;
        }

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
