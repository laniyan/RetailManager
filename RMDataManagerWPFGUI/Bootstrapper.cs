using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using RetailManagerWPFGUI.Views;
using RetailManagerWPFGUI.ViewModels;
using System.Windows.Controls;
using AutoMapper;
using RetailManagerWPFGUI.Helpers;
using RetailManagerWPFGUI.Models;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;

namespace RetailManagerWPFGUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container = new SimpleContainer();//this is our denpendency injection container this new everything up for us
        public Bootstrapper()
        {
            Initialize();//drives from the BootstrapperBase

            ConventionManager.AddElementConvention<PasswordBox>(
                PasswordBoxHelper.BoundPasswordProperty,
                "Password",
                "PasswordChanged");
        }

        private IMapper ConfigureAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //this we knw from the begin how to map a productModel to a ProductDisplay it uses reflection it does it once then store the info thats way its so fast
                //this creates a configuration
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });

            //create a mapper
            var output = config.CreateMapper();

            return output;
        }

        protected override void Configure()
        {
            //Add mapper to dependency injection system
            _container.Instance(ConfigureAutomapper());//this is a singleton

            _container
                .Instance(_container) /*when every we ask for a simple container instance it will return this instance (_container) the container hold an instance of itself to pass out when ever u
            ask for a container the reason is we may want to get this container in order to manipulate something or change something or get info out of it besides from our ctor
            so we can use this to new up a class of itself i.e LogInModel logIn = new LogInModel();*/
                .PerRequest<IProductEndpoint, ProductEndpoint>()
                .PerRequest<ISaleEndpoint, SaleEndpoint>();

            _container.Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IConfigHelper, ConfigHelper>()
                .Singleton<IAPIHelper, APIHelper>();/*we want caliburn to handle 1st the window manager handling of bringing windows in and out
            thats is important 2nd is event aggregator this is where we can pass event messaging tru out our app so that one piece can raise an event and different pieces can listen for it and do 
            somethink with it essentially it's how we tire our app together instead of passing data back and forward tru ctor and public methods out event aggregator handles it for us its like a central
            clearinghouse for all events, these are rapid in a singleton beoz we only want one instance of this during the lifecycle of our app (Tim Corey vid 7 dependency inj 10:00 ) we want too connect
            to events we need them all in one place it would be mad it they were all in diff instance they wont talk to each other
            IMPORTANT INFO typically ur app shouldnt be very careful not to use singletons unless u really need it dont use it unless u cant find another way there not gr8 on memory usage and not 
            optimsied for oop in general we want per request which is just a new instance each time create the instance use the instance throw it away*/


            /*connecting all our viewModels to our views we use reflection it is powerful nut it told not to use coz its slow but only slow when u do it 10000 times it aint really slow doing it once
             its only going to run once on start up that when this method is hit the performance will take a lil longer on start up becoz of this it is tiny*/
            GetType().Assembly.GetTypes()//where using reflection get type for our current instance get current assembly thats running and get all the types where 
                .Where(type => type.IsClass).Where(type => type.Name.EndsWith("ViewModel")).ToList()//then put them all in a list so all classes that end with ViewModels
                .ForEach(ViewModelType => _container.RegisterPerRequest(ViewModelType, ViewModelType.ToString(), ViewModelType));//then for each of them reg using args of type, key and type
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            //we have ovrrided this method from BootstrapperBase
            DisplayRootViewFor<ShellViewModel>();//what this does is say on start up I want you to launch shell view model as our base view
            //the viewModel will then lunch the view based upon caliburn and micro wiring them together (we set latter)
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}
