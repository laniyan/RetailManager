using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RetailManagerWPFGUI.Helpers;

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
            get { return _userName;  }
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
            var results = await _apiHelper.Authenticate(UserName, Password);
        }
    }
}
