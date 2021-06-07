using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RetailManagerWPFGUI.Helpers;
using RMDesktopUI.Library.Api;

namespace RetailManagerWPFGUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName;
        private string _password;
        private IAPIHelper _apiHelper;

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);//keep real time check on anything being typed in this field
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);//keep real time check on anything being typed in this field
                NotifyOfPropertyChange(() => CanLogIn);

            }
        }

        private bool _IsErrorVisible;

        public bool IsErrorVisible
        {
            get
            {
                bool output = false;

                if (ErrorMessage?.Length > 0)
                {
                    output = true;
                }
                return output;
            }

        }

        private string _errorMessage;           

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
               
            }
        }



        public bool CanLogIn
        {
            get
            {

                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }

                return output;

            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";//every call we make clears our error message
                var results = await _apiHelper.Authenticate(UserName, Password);

                // Capture more info about the user
                await _apiHelper.GetLoggedInUserInfo(results.Access_Token);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            
        }
    }
}
