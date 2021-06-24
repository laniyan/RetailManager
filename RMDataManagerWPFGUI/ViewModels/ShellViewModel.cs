using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.EventModels;
using RMDesktopUI.Library.Models;

namespace RetailManagerWPFGUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
       
        private IEventAggregator _event;
        private SalesViewModel _salesVM;

        private ILoggedInUserModel _user;
        // private SimpleContainer _container; no longer need coz of Ioc

        private IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user, IAPIHelper apiHelper
            /*SimpleContainer container no longer need coz of Ioc*/)
        {
            
           // ActivateItem(_loginVM);//this will active our login in our shell page 
            _event = events;
            //Listen to events
            _event.Subscribe(this);/* in the agr u put who is subcribing to the events this means the current instance of this class
            the reason why we tell it who is subcribing is coz the event aggregator has to say ok im remmbering who subscribing so that when a event happens it will send that
            event to every subcriber even if ur not listening to that particular type so now it send every broadcast it gets to here when we see a bc if it not LogonEvent 
            we leave it coz that all we handel (Tim Corey vid Event agg 16 23:00)*/

            _salesVM = salesVM;

            // _container = container; no longer need coz of Ioc

            /* we no longer need _container we can use caliburn Ioc.Get() instead does the same thing with less denpendency
            ActivateItem(_container.GetInstance<LoginViewModel>());this gets a new instance of LoginViewModel and activate it everytime the shell ctor is called
            and wipes out an old existing ones with all credentials so if anyone calls log in page again they will get a new one and u have to call login page from
             shellveiw thats how we've set it up shellview will control all the views in the system*/

            _user = user;

            _apiHelper = apiHelper;

            ActivateItem(IoC.Get<LoginViewModel>());//the same as up top just much better way from caliburn 
        }

        public bool IsLoggedIn
        {
            get
            {
                bool output = false;

                if (string.IsNullOrWhiteSpace(_user.Token) == false)
                {
                    output = true;
                }

                return output;
            }
        }

        public void ExitApplication()
        {
            TryClose();
        }

        public void UserManagement()
        {
            ActivateItem(IoC.Get<UserDisplayViewModel>());
        }

        public void LogOut()
        {
            _user.ResetUserModel();
            _apiHelper.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public void Handle(LogOnEvent message)// this handles the log on event so when its fired this is executed
        {
            ActivateItem(_salesVM);// this close the log in page and activate our sales page becoz we can only have 1 item active at a time wen we have the conductor<object>
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
