using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using RetailManagerWPFGUI.Models;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;

namespace RetailManagerWPFGUI.ViewModels
{
    public class UserDisplayViewModel : Screen
    {
        private StatusInfoViewModel _status;
        private IWindowManager _window;
        private IUserEndpoint _userEndpoint;

        public UserDisplayViewModel(IUserEndpoint userEndpoint, StatusInfoViewModel status, IWindowManager window)
        {
            _window = window;
            _userEndpoint = userEndpoint;
        }

        protected override async void OnViewLoaded(object view)// this will load the product when the screen is opened we cant do it in the ctor because u cant use await in ctor so we have to use this method 
        {
            base.OnViewLoaded(view);

            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStarupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";

                if (ex.Message == "Unauthorized")
                {
                    _status.UpdateMessage("Unatuhorized Access", "You go not have permission to interact with the sales");
                    _window.ShowDialog(_status, null, null);//thats null for context and settings

                }
                else
                {
                    _status.UpdateMessage("Fatal Exceptions", ex.Message);
                    _window.ShowDialog(_status, null, null);
                }

                TryClose();
            }


        }

       private  BindingList<UserModel> _users;

        public BindingList<UserModel> Users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
                NotifyOfPropertyChange(() => Users);
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();

            Users = new BindingList<UserModel>(userList);
        }
    }
}
