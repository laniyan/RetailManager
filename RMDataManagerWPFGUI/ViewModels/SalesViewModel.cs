using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace RetailManagerWPFGUI.ViewModels
{
    public class SalesViewModel : Screen
    {
		private BindingList<string> _products;

		public BindingList<string> Products			
		{
			get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);//if we overrite the whole binding list this will trigger
            }
		}

        public string SubTotal
        {
            get
            {

                return "o";
            }
        }

        public string Tax
        {
            get
            {

                return "o";
            }
        }

        public string Total
        {
            get
            {

                return "o";
            }
        }

        private BindingList<string> _cart;

        public BindingList<string> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private string _itemQuantity;

		public string ItemQuantity
		{
			get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
            }
		}

        public bool CanAddToCart //this is  how caliburn wires things up this naming convent and it has to be a bool it could be a method returning aa bool but we use prop coz method cant NotifyOfPropertyChange (Tim Corey vid sales page 15 42:00)
        {
            get
            {
                bool output = false;

                //the logic to check if something is selected and if there is an item quantity

                return output;
            }
        }


        public void AddToCart()
        {

        }

        public bool CanRemoveFromCart 
        {
            get
            {
                bool output = false;

                //the logic to check if something is selected and if there is an item quantity

                return output;
            }
        }


        public void RemoveFromCart()
        {

        }

        public bool CanCheckOut
        {
            get
            {
                bool output = false;

                //the logic to check if something is selected and if there is an item quantity

                return output;
            }
        }


        public void CheckOut()
        {

        }

    }
}
