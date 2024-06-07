using EasyPaperWork.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using EasyPaperWork.Services;
using System.Diagnostics;

namespace EasyPaperWork.ViewModel
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        public ICommand RegistrationCommand { get; set;}
        public ICommand ButtonCommand1 { get; }
        public ICommand ButtonCommand2 { get; }
        public ICommand ButtonCommand3 { get; }
        public string EntryName { get; set; }
        public string EntryEmail { get; set; }
        public string EntryPassword1 { get; set; }
        public string EntryPassword2 { get; set; }
        public string AccountType {  get; set; }   
        private Color _buttonBackgroundColor1;
        private Color _buttonTextColor1;
        private Color _buttonBackgroundColor2;
        private Color _buttonTextColor2;
        private Color _buttonBackgroundColor3;
        private Color _buttonTextColor3;
        FirebaseService FirebaseService { get; set; }
        
        public UserModel User { get; set; }
        public RegistrationViewModel()
        {
            RegistrationCommand = new Command(Registration);
            User = new UserModel();
            ButtonBackgroundColor1 = Colors.White;
            ButtonTextColor1 = Colors.Black;
            ButtonBackgroundColor2 = Colors.White;
            ButtonTextColor2 = Colors.Black;
            ButtonBackgroundColor3 = Colors.White;
            ButtonTextColor3 = Colors.Black;
            ButtonCommand1 = new Command(OnButtonClicked1);
            ButtonCommand2 = new Command(OnButtonClicked2);
            ButtonCommand3 = new Command(OnButtonClicked3);
           FirebaseService = new FirebaseService();
        }
        public void Registration()
        {
            Debug.WriteLine("Botão cadastro clicado");
            Task task = SendUser();
        }

        private async Task SendUser()
        {   //To do : fazer mensagens de erro caso não aprove nos ifs
            if (!string.IsNullOrEmpty(EntryName)) { User.Name = EntryName; }
            if (!string.IsNullOrEmpty(EntryEmail)) { User.Email = EntryEmail; }
            if (EntryPassword1 == EntryPassword2) { User.Passoword = EntryPassword1; }
            if (AccountType != null) { User.AccountType = AccountType; }
           // User.DateUserCreated = DateTimeOffset.UtcNow;
            User.Id = Guid.NewGuid();
            await FirebaseService.AdicionarObjetoAsync("Users",Convert.ToString(User.Id), User);
            Debug.WriteLine("enviado para a clase fire base");

        }
       

        public Color ButtonBackgroundColor1
        {
            get => _buttonBackgroundColor1;
            set
            {
                if (_buttonBackgroundColor1 != value)
                {
                    _buttonBackgroundColor1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color ButtonTextColor1
        {
            get => _buttonTextColor1;
            set
            {
                if (_buttonTextColor1 != value)
                {
                    _buttonTextColor1 = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color ButtonBackgroundColor2
        {
            get => _buttonBackgroundColor2;
            set
            {
                if (_buttonBackgroundColor2 != value)
                {
                    _buttonBackgroundColor2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color ButtonTextColor2
        {
            get => _buttonTextColor2;
            set
            {
                if (_buttonTextColor2 != value)
                {
                    _buttonTextColor2 = value;
                    OnPropertyChanged();
                }
            }
        }
        public Color ButtonBackgroundColor3
        {
            get => _buttonBackgroundColor3;
            set
            {
                if (_buttonBackgroundColor3 != value)
                {
                    _buttonBackgroundColor3 = value;
                    OnPropertyChanged();
                }
            }
        }

        public Color ButtonTextColor3
        {
            get => _buttonTextColor3;
            set
            {
                if (_buttonTextColor3 != value)
                {
                    _buttonTextColor3 = value;
                    OnPropertyChanged();
                }
            }
        }

        private void OnButtonClicked1()
        {
            
            ButtonBackgroundColor1 =  Colors.Gray;
            ButtonTextColor1 = Colors.White;
            ButtonBackgroundColor2 = Colors.White;
            ButtonTextColor2 = Colors.Black;
            ButtonBackgroundColor3 = Colors.White;
            ButtonTextColor3 = Colors.Black;
            AccountType = "EnterpriseAccount";
        }
        private void OnButtonClicked2()
        {
            ButtonBackgroundColor1 = Colors.White;
            ButtonTextColor1 = Colors.Black;
            ButtonBackgroundColor2 = Colors.Gray;
            ButtonTextColor2 = Colors.White;
            ButtonBackgroundColor3 = Colors.White;
            ButtonTextColor3 = Colors.Black;
            AccountType = "EmployeeaAccount";
            
       
        }
        private void OnButtonClicked3()
        {
            ButtonBackgroundColor1 = Colors.White;
            ButtonTextColor1 = Colors.Black;
            ButtonBackgroundColor2 = Colors.White;
            ButtonTextColor2 = Colors.Black;
            ButtonBackgroundColor3 = Colors.Gray;
            ButtonTextColor3 = Colors.White;
            AccountType = "PersonalAccount";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
