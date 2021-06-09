using System;
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

        private ProductModel _selectedProduct;

        public ProductModel SelectedProduct 
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }


        public string SubTotal
        {
            get
            {
                decimal subTotal = 0;
                foreach (var item in Cart)
                {
                    subTotal += item.Product.RetailPrice;
                }
                return $"{subTotal.ToString("C")}";
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

        private BindingList<CartItemModel> _cart = new BindingList<CartItemModel>();

        public BindingList<CartItemModel> Cart
        {
            get { return _cart; }
            set
            {
                _cart = value;
                NotifyOfPropertyChange(() => Cart);
            }
        }

        private int _itemQuantity = 1;

		public int ItemQuantity
		{
			get { return _itemQuantity; }
            set
            {
                _itemQuantity = value;
                NotifyOfPropertyChange(() => ItemQuantity);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
		}

        public bool CanAddToCart //this is  how caliburn wires things up this naming convent and it has to be a bool it could be a method returning aa bool but we use prop coz method cant NotifyOfPropertyChange (Tim Corey vid sales page 15 42:00)
        {
            get
            {
                bool output = false;

                //the logic to check if something is selected and if there is an item quantity
                if (ItemQuantity > 0 && SelectedProduct?.QuantityInStock >= ItemQuantity)
                {
                    output = true;
                }
                return output;
            }
        }


        public void AddToCart()
        {
            CartItemModel existingItem = Cart.FirstOrDefault(c => c.Product == SelectedProduct);//check to see if we already have the item in the basket

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity; //if we have item in basket then just change the quantity amount 
                Cart.Remove(existingItem);//remove the old version
                Cart.Add(existingItem);//update it with the new
            }
            else
            {
                CartItemModel item = new CartItemModel()
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            
            SelectedProduct.QuantityInStock -= ItemQuantity;/*stops you from adding the product multiple times and going over the quantity now if u add the product 5x each with quantity 6 and the total
                                                             quantity of the product is 20 it will stop u on the last one coz your available quantity has gone down to 2 */
            ItemQuantity = 0;
            NotifyOfPropertyChange(() => SubTotal);
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
            NotifyOfPropertyChange(() => SubTotal);
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
