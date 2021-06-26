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

        public UserDisplayViewModel(IUserEndpoint userEndpoint, StatusInfoViewModel status, IWindowManager window, string selectedUserRole)
        {
            _window = window;
            _selectedUserRole = selectedUserRole;
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

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();

            Users = new BindingList<UserModel>(userList);
        }

        private async Task LoadRoles()
        {
            var roles = await _userEndpoint.GetAllRoles();

            foreach (var role in roles)
            {
                //we check to see if the user has this role already
                if (UserRoles.IndexOf(role.Value) < 0)//index of will return the 1st occurrence of the item and if it finds nothink it will return -1
                {
                    AvailableRoles.Add(role.Value);
                } 
            }
        }

        //this is a event type becoz its connected to the view so caliburn turns this method into a event type a event type must have null even if theres a async we dont use task only void
        public async void AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser.Id, SelectedAvailableRole);

            UserRoles.Add(SelectedAvailableRole);//add to users roles
            AvailableRoles.Remove(SelectedAvailableRole);//remove from available roles
        }

        public async void RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser.Id, SelectedUserRole);

            AvailableRoles.Add(SelectedUserRole);//remove from available roles
            UserRoles.Remove(SelectedUserRole);//add to users roles
        }


        private BindingList<UserModel> _users;

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

        private UserModel _selectedUser;

        public UserModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                SelectedUserName = value.Email;
                UserRoles = new BindingList<string>(value.Roles.Select(r => r.Value).ToList());//the role is a key value pair but we only want thename of the role you only need a name to change a role
                LoadRoles();
                NotifyOfPropertyChange(() => SelectedUser);
            }
        }

        //everytime we get a change from a set user we update selectUserName
        private string _selectedUserName;

        public string SelectedUserName
        {
            get { return _selectedUserName; }
            set
            {
                _selectedUserName = value;
                NotifyOfPropertyChange(() => SelectedUserName);
            }
        }

        private BindingList<string> _userRoles = new BindingList<string>();

        public BindingList<string> UserRoles
        {
            get { return _userRoles; }
            set
            {
                _userRoles = value;
                NotifyOfPropertyChange(() => UserRoles);
            }
        }

        private BindingList<string> _availableRoles = new BindingList<string>();

        public BindingList<string> AvailableRoles
        {
            get { return _availableRoles; }
            set
            {
                _availableRoles = value;
                NotifyOfPropertyChange(() => AvailableRoles);
            }
        }

        /*use to be AvailableRoles here and on the view but coz caliburn and micro have a special naming convention the view cant cant fire of the event so we had to
         change it to SelectedAvailableRole one of the rules it has to be singular not plural coz the view is giving plural its expect a singular prop to come to it
         so it looks for a singular naming convention so we change the name of the prop*/
        private string _selectedAvailableRole;

        public string SelectedAvailableRole
        {
            get { return _selectedAvailableRole; }
            set
            {
                _selectedAvailableRole = value;
                NotifyOfPropertyChange(() => SelectedAvailableRole);
            }
        }

        //changed on both sides the view and here from SelectedRoleToRemove to SelectedUserRole
        private string _selectedUserRole;

        public string SelectedUserRole
        {
            get { return _selectedUserRole; }
            set
            {
                _selectedUserRole = value;
                NotifyOfPropertyChange(() => SelectedUserRole);
            }
        }

        private string _selectedRoleToAdd;

        public string SelectedRoleToAdd
        {
            get { return _selectedRoleToAdd; }
            set
            {
                _selectedRoleToAdd = value;
                NotifyOfPropertyChange(() => SelectedRoleToAdd);
            }
        }
    }
}
