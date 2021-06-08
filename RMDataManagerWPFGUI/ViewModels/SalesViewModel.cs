﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Models;

namespace RetailManagerWPFGUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;

        public SalesViewModel(IProductEndpoint productEndpoint)
        {
            _productEndpoint = productEndpoint;
        }

        protected override async void OnViewLoaded(object view)// this will load the product when the screen is opened we cant do it in the ctor because u cant use await in ctor so we have to use this method 
        {
            base.OnViewLoaded(view);
            await LoadProducts();

        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAllProduct();
            Products = new BindingList<ProductModel>(productList);
        }

		private BindingList<ProductModel> _products;

		public BindingList<ProductModel> Products			
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

        private int _itemQuantity;

		public int ItemQuantity
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
