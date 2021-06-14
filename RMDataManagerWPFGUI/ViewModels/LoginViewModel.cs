using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RetailManagerWPFGUI.Helpers;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.EventModels;

namespace RetailManagerWPFGUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName = "laniyan@hotmail.com";//hardcode login coz we know it works and we dont want to keep entering everytime we test the code
        private string _password = "1234Qwer;";
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events )
        {
            _apiHelper = apiHelper;
            _events = events;
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

                //fire event
                _events.PublishOnUIThread(new LogOnEvent());/* we use PublishOnUIThread becoz this makes sure if the thread on top slips of or goes into a background thread etc it makes doubly 
                                             sure this event will be listen to on the UI thread other Ui can use it without having any cross threading isseus 
                                             we can pass a string in the args to be publish but thats way to generic to knw wot it represents alot of people may be looking 4 a string
                                             so instead we create a class with the discreption of what we doing with it it will be empty so we pass an empty instance in now who every
                                             looking for that we defo only be looking for that that is more strongly typed we use it like a key/trigger looking for the instance of this 
                                             class where anyone could be looking for a string value (breakdown Tim Corey vid event agg 16 15:35 )*/
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            
        }
    }
}
