using EasyPaperWork.Models;
using EasyPaperWork.Security;
using EasyPaperWork.Services;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace EasyPaperWork.ViewModel
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private FirebaseAuthServices firebaseAuthServices;
        private EncryptData encryptData;
        public ICommand RegistrationCommand { get; set; }
        public ICommand ButtonCommand1 { get; }
        public ICommand ButtonCommand2 { get; }
        public ICommand ButtonCommand3 { get; }
        public string EntryName { get; set; }
        public string EntryEmail { get; set; }
        public string EntryPassword1 { get; set; }
        public string EntryPassword2 { get; set; }
        private string AccountType { get; set; }

        private bool _EnterpriseAccount;
        public bool EnterpriseAccount
        {
            get { return _EnterpriseAccount; }
            set
            {
                if (_EnterpriseAccount != value)
                {
                    _EnterpriseAccount = value;
                    Debug.WriteLine("alterou");
                    OnPropertyChanged(nameof(EnterpriseAccount));
                }
                if (_EnterpriseAccount)
                {
                    Debug.WriteLine("true");
                    if (EmployeeAccount) { EmployeeAccount = false; }
                    if (PersonalAccount) { PersonalAccount = false; }
                    AccountType = "EnterpriseAccount";
                }
                else { Debug.WriteLine("false"); }
            }
        }
        private bool _EmployeeAccount;
        public bool EmployeeAccount
        {
            get { return _EmployeeAccount; }
            set
            {
                if (_EmployeeAccount != value)
                {
                    _EmployeeAccount = value;
                    Debug.WriteLine("alterou");
                    OnPropertyChanged(nameof(EmployeeAccount));
                }
                if (_EmployeeAccount)
                {
                    Debug.WriteLine("trueEmployee");

                    if (EnterpriseAccount) { EnterpriseAccount = false; }
                    if (PersonalAccount) { PersonalAccount = false; }
                    AccountType = "EmployeeAccount";
                }
                else { Debug.WriteLine("false"); }
            }
        }
        public bool _PersonalAccount;
        public bool PersonalAccount
        {
            get { return _PersonalAccount; }
            set
            {
                if (_PersonalAccount != value)
                {
                    _PersonalAccount = value;
                    OnPropertyChanged(nameof(PersonalAccount));
                    Debug.WriteLine("Alterou");
                }
                if (_PersonalAccount)
                {
                    Debug.WriteLine("Alteroupersonal");
                    if (EnterpriseAccount) { EnterpriseAccount = false; }
                    if (EmployeeAccount) { EmployeeAccount = false; }
                    AccountType = "PersonalAccount";
                }
                else { Debug.WriteLine("falsepersonal"); }
            }
        }
        FirebaseService FirebaseService { get; set; }
        FirebaseAuthServices _FirebaseAuthServices;

        public UserModel User;
        public RegistrationViewModel()
        {
            encryptData = new EncryptData();
            _FirebaseAuthServices = new FirebaseAuthServices();
            RegistrationCommand = new Command(Registration);
            User = new UserModel();

            FirebaseService = new FirebaseService();
        }
        public void Registration()
        {
            Debug.WriteLine("Botão cadastro clicado");
            Task task = SendUser();
        }

        private async Task SendUser()
        {   //To do : fazer mensagens de erro caso não aprove nos ifs
            if (string.IsNullOrEmpty(EntryName)) { await Application.Current.MainPage.DisplayAlert("Erro", "Nome necessário para registro", "ok"); }
            else
            {
                if (string.IsNullOrEmpty(EntryEmail)) { await Application.Current.MainPage.DisplayAlert("Erro", "Email necessário para registro", "ok"); }
                else
                {
                    if (EntryPassword1 != EntryPassword2) { await Application.Current.MainPage.DisplayAlert("Erro", "As senhas são diferentes", "ok"); }
                    else
                    {
                        if (string.IsNullOrEmpty(AccountType)) { await Application.Current.MainPage.DisplayAlert("Erro", "É necessário selecionar um tipo de conta para registro", "ok"); }
                        else
                        {
                            // User.DateUserCreated = DateTimeOffset.UtcNow;

                            string result = await _FirebaseAuthServices.RegiterUser(EntryEmail, EntryPassword1, EntryName);
                            switch (result)
                            {
                                case "EmailExists":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "Este Email já está cadastrado", "ok");
                                    break;
                                case "InvalidEmailAddress":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "Este Email é inválido", "ok");
                                    break;
                                case "WeakPassword":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "A senha deve ter mais de 6 caracteres", "ok");
                                    break;
                                case "MissingEmail":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "O Email não foi encontrado", "ok");
                                    break;
                                case "MissingPassword":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "A senha não foi encontrada", "ok");
                                    break;
                                case "error":
                                    await Application.Current.MainPage.DisplayAlert("Erro", "Um erro inesperado aconteceu, tente novamente ", "ok");
                                    break;
                                default:
                                    await Application.Current.MainPage.DisplayAlert("Sucesso", "Você foi cadastrado corretamente", "ok");
                                    break;
                            }
                            if (result == "UserCreated")
                            {
                                string id = await _FirebaseAuthServices.GetUidToken(EntryEmail, EntryPassword1);

                                User.Id = id;
                                User.Salt = encryptData.GenerateSaltString();

                                byte[] salt = encryptData.GetSaltBytes(User.Salt);
                                byte[] key = encryptData.GetKey(salt, EntryPassword1);
                                User.Email = encryptData.Encrypt(EntryEmail, key, salt);
                                User.Name = encryptData.Encrypt(EntryName, key, salt);
                                User.Password = encryptData.Encrypt(EntryPassword1, key, salt);
                                User.AccountType = encryptData.Encrypt(AccountType, key, salt);
                                await FirebaseService.AdicionarObjetoAsync("Users", User.Id, User);
                                result = " ";
                                await Shell.Current.GoToAsync("//Page_Login");

                            }
                        }
                    }
                }
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
