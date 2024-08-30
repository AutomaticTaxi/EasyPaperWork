using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Auth.Repository;
using Firebase.Auth.Requests;
using FirebaseAdmin.Auth;
using Microsoft.Maui.Graphics.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyPaperWork.Services
{
    public class FirebaseAuthServices
    {
      
        private FirebaseAuthClient _authClient;
        public User user;
        private string UserTokenID;
        private UserCredential userCredential;
        public string _UserTokenID
        {
            get { return UserTokenID; }
            set
            {
                UserTokenID = value;
            }
        }
        public FirebaseAuthServices()
        {
            

            var config = new FirebaseAuthConfig
            {
                ApiKey = "AIzaSyCIHw3fP1XoNiuIZK6eNs0LIwi1SDDAyao",
                AuthDomain = "easypaperwork-firebase.firebaseapp.com",
                Providers = new Firebase.Auth.Providers.FirebaseAuthProvider[]
                      {
                    new EmailProvider()
                      },
                UserRepository = new FileUserRepository("Users")
            };
            try
            {
                _authClient = new FirebaseAuthClient(config);
            }catch(Exception ex) {Debug.WriteLine(ex.ToString());}
           
        }
        public async Task<string> SignInButton_Click(string email, string password)
        {
            try
            {

                userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                // Aqui você pode lidar com o usuário logado (userCredential.User)
                // Exemplo de como obter um token de ID

                var idToken = userCredential.User.Uid.ToString();
                _UserTokenID = idToken;
                Debug.WriteLine("ID Token: " + idToken.ToString());
                Debug.WriteLine("Logado com sucesso");
                return "success";

                

            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                if (ex.Reason == Firebase.Auth.AuthErrorReason.InvalidEmailAddress)
                {
                    Debug.WriteLine("EmailExists");
                    return "EmailExists";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.MissingEmail)
                {
                    Debug.WriteLine("MissingEmail");
                    return "MissingEmail";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.MissingPassword)
                {
                    Debug.WriteLine("MissingPassword");
                    return "MissingPassword";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.WrongPassword)
                {
                    Debug.WriteLine("WrongPassword");
                    return "WrongPassword";
                }
                return "error";

            }
        }
            public async Task<string> GetUidToken(string email, string password)
            {
                try
                {

                    userCredential = await _authClient.SignInWithEmailAndPasswordAsync(email, password);

                    // Aqui você pode lidar com o usuário logado (userCredential.User)
                    // Exemplo de como obter um token de ID

                    var idToken = userCredential.User.Uid.ToString();
                    _UserTokenID = idToken;
                    Debug.WriteLine("ID Token: " + idToken.ToString());
                    Debug.WriteLine("Logado com sucesso");
                    return idToken.ToString();

              

                }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                if (ex.Reason == Firebase.Auth.AuthErrorReason.InvalidEmailAddress)
                {
                    Debug.WriteLine("InvalidEmailAddress");
                    return "InvalidEmailAddress";
                }
                else if (ex.Reason == Firebase.Auth.AuthErrorReason.UnknownEmailAddress)
                {
                    Debug.WriteLine("UnknownEmailAddress");
                    return "UnknownEmailAddress";
                }
                else if(ex.Reason == Firebase.Auth.AuthErrorReason.MissingEmail)
                {
                    Debug.WriteLine("MissingEmail");
                    return "MissingEmail";
                }
                else if(ex.Reason == Firebase.Auth.AuthErrorReason.MissingPassword)
                {
                    Debug.WriteLine("MissingPassword");
                    return "MissingPassword";
                }
                else if(ex.Reason == Firebase.Auth.AuthErrorReason.WrongPassword)
                {
                    Debug.WriteLine("WrongPassword");
                    return "WrongPassword";
                }
                else if (ex.Reason == Firebase.Auth.AuthErrorReason.TooManyAttemptsTryLater)
                {
                    Debug.WriteLine("TooManyAttemptsTryLater");
                    return "TooManyAttemptsTryLater";
                }
                else { return "error"; }
                

            }
        }

        public async Task<string> RegiterUser(string email, string password, string name)
        {
            try
            {
                await _authClient.CreateUserWithEmailAndPasswordAsync(email, password, name);

                return "UserCreated";
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
           {

                if (ex.Reason == Firebase.Auth.AuthErrorReason.EmailExists)
                {
                    Debug.WriteLine("EmailExists");
                    return "EmailExists";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.InvalidEmailAddress)
                {
                    Debug.WriteLine("InvalidEmailAddress");
                    return "InvalidEmailAddress";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.WeakPassword)
                {
                    Debug.WriteLine("WeakPassword");
                    return "WeakPassword";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.MissingEmail)
                {
                    Debug.WriteLine("MissingEmail");
                    return "MissingEmail";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.MissingPassword)
                {
                    Debug.WriteLine("MissingPassword");
                    return "MissingPassword";
                }
              

                return "error";
            }
        }
        public async Task<string> ChangingUserPassword(string email)
        {
            try
            {

                 await _authClient.ResetEmailPasswordAsync(email);
                return "Succsses";

            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                if (ex.Reason == Firebase.Auth.AuthErrorReason.MissingEmail)
                {
                    Debug.WriteLine("MissingEmail");
                    return "MissingEmail";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.InvalidEmailAddress)
                {
                    Debug.WriteLine("InvalidEmailAddress");
                    return "InvalidEmailAddress";
                }
                if (ex.Reason == Firebase.Auth.AuthErrorReason.ResetPasswordExceedLimit)
                {
                    Debug.WriteLine("ResetPasswordExceedLimit");
                    return "ResetPasswordExceedLimit";
                }
                return "error";

            }
        }



        public void SignOutButton_Click()
        {
            // Para deslogar o usuário
            _authClient.SignOut();

        }


   
    }
}
