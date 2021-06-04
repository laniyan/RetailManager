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
using RetailManagerWPFGUI.Helpers;

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

        protected override void Configure()
        {
            _container.Instance(_container);/*when every we ask for a somple container instance it will return this instance (_container) the container hold an instance of itself to pass out when ever u
            ask for a container the reason is we may want to get this container in order to manipulate something or change something or get info out of it besides from our ctor */

            _container.Singleton<IWindowManager, WindowManager>().Singleton<IEventAggregator, EventAggregator>().Singleton<IAPIHelper, APIHelper>();/*we want caliburn to handle 1st the window manager handling of bringing windows in and out
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
