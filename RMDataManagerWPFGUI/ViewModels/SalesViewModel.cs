using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Caliburn.Micro;
using RetailManagerWPFGUI.Models;
using RMDesktopUI.Library.Api;
using RMDesktopUI.Library.Helpers;
using RMDesktopUI.Library.Models;

namespace RetailManagerWPFGUI.ViewModels
{
    public class SalesViewModel : Screen
    {
        private IProductEndpoint _productEndpoint;
        private IConfigHelper _configHelper;
        private ISaleEndpoint _saleEndpoint;
        private IMapper _mapper;

        public SalesViewModel(IProductEndpoint productEndpoint, IConfigHelper configHelper, ISaleEndpoint saleEndpoint, IMapper mapper)
        {
            _productEndpoint = productEndpoint;
            _configHelper = configHelper;
            _saleEndpoint = saleEndpoint;
            _mapper = mapper;
        }

        protected override async void OnViewLoaded(object view)// this will load the product when the screen is opened we cant do it in the ctor because u cant use await in ctor so we have to use this method 
        {
            base.OnViewLoaded(view);
            await LoadProducts();

        }

        private async Task LoadProducts()
        {
            var productList = await _productEndpoint.GetAllProduct();
            var products = _mapper.Map<List<ProductDisplayModel>>(productList);/*even dough I didnt map a list of productDisplay auto mapper is intelligent enough to say okay list 
            well a list of productDisplay it takes the productList and says it knows what a productDisplay is there for each product list is a productModel so it converts each one
            to a productDisplay its going to map each on individually and put the results into a list and return that list to the product var*/
            Products = new BindingList<ProductDisplayModel>(products);
        }

		private BindingList<ProductDisplayModel> _products;

		public BindingList<ProductDisplayModel> Products			
		{
			get { return _products; }
            set
            {
                _products = value;
                NotifyOfPropertyChange(() => Products);//if we overrite the whole binding list this will trigger
            }
		}

        private ProductDisplayModel _selectedProduct;

        public ProductDisplayModel SelectedProduct 
        {
            get { return _selectedProduct; }
            set
            {
                _selectedProduct = value;
                NotifyOfPropertyChange(() => SelectedProduct);
                NotifyOfPropertyChange(() => CanAddToCart);
            }
        }

        private CartItemDisplayModel _selectedCartItem;

        public CartItemDisplayModel SelectedCartItem
        {
            get { return _selectedCartItem; }
            set
            {
                _selectedCartItem = value;
                NotifyOfPropertyChange(() => SelectedCartItem);
                NotifyOfPropertyChange(() => CanRemoveFromCart);
            }
        }

        public string SubTotal
        {
            get
            {
                decimal subTotal = CalculateSubTotal();

                return $"{subTotal:C}";
            }
        }

        private decimal CalculateSubTotal()
        {
            decimal subTotal = 0;

            foreach (var item in Cart)
            {
                subTotal += item.Product.RetailPrice;
            }

            return subTotal;
        }

        public string Tax
        {
            get
            {
                decimal taxAmount = CalculateTax();

                return $"{taxAmount:C}";
            }
        }

        private decimal CalculateTax()
        {
            decimal taxAmount = 0;
            decimal taxRate = _configHelper.GetTaxRate()/100;

            //new way of doing calcu does exact same thing as bottom (foreach) but its clear
            taxAmount = Cart.Where(c => c.Product.IsTaxable)
                .Sum(c => c.Product.RetailPrice * c.QuantityInCart * taxRate);

            /*foreach (var item in Cart)
            {
                if (item.Product.IsTaxable)
                {
                    taxAmount += (item.Product.RetailPrice * item.QuantityInCart * taxRate);
                }
            }*/

            return taxAmount;
        }

        public string Total
        {
            get
            {
                decimal total = CalculateSubTotal() + CalculateTax();

                return $"{total:c}";
            }
        }

        private BindingList<CartItemDisplayModel> _cart = new BindingList<CartItemDisplayModel>();

        public BindingList<CartItemDisplayModel> Cart
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
            CartItemDisplayModel existingItem = Cart.FirstOrDefault(c => c.Product == SelectedProduct);//check to see if we already have the item in the basket

            if (existingItem != null)
            {
                existingItem.QuantityInCart += ItemQuantity; //if we have item in basket then just change the quantity amount 
                //Hack
                //Cart.Remove(existingItem);//remove the old version
                //Cart.Add(existingItem);//update it with the new
            }
            else
            {
                CartItemDisplayModel item = new CartItemDisplayModel()
                {
                    Product = SelectedProduct,
                    QuantityInCart = ItemQuantity
                };
                Cart.Add(item);
            }
            
            SelectedProduct.QuantityInStock -= ItemQuantity;/*stops you from adding the product multiple times and going over the quantity now if u add the product 5x each with quantity 6 and the total
                                                             quantity of the product is 20 it will stop u on the last one coz your available quantity has gone down to 2 */
            ItemQuantity = 1;
            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanRemoveFromCart 
        {
            get
            {
                bool output = false;

                //the logic to check if something is selected and if there is an item quantity
                if (SelectedCartItem != null && SelectedCartItem?.Product.QuantityInStock > 0)
                {
                    output = true;
                }

                return output;
            }

        }


        public void RemoveFromCart()
        {
            SelectedCartItem.QuantityInCart -= 1;

            if (SelectedCartItem.QuantityInCart > 1)
            {
                SelectedCartItem.Product.QuantityInStock += 1;//to put the quantity in the stock
            }
            else
            {
                Cart.Remove(SelectedCartItem);
            }

            NotifyOfPropertyChange(() => SubTotal);
            NotifyOfPropertyChange(() => Tax);
            NotifyOfPropertyChange(() => Total);
            NotifyOfPropertyChange(() => CanCheckOut);
        }

        public bool CanCheckOut
        {
            get
            {
                bool output = Cart.Count > 0; //vs said do it this way which is much better belwo is the old way and my way

                /* bool output = false;

                 if (Cart.Count > 0)
                 {
                     output = true;
                 }*/

               // bool o = Cart.Count > 0 ? true : false;

                //the logic to check if something is selected and if there is an item quantity

                return output;
            }
        }


        public async Task CheckOut()
        {
            SaleModel sale = new SaleModel();

            foreach (var item in Cart)
            {
                sale.SaleDetails.Add(new SaleDetailModel
                {
                    ProductId = item.Product.Id,
                    Quantity = item.QuantityInCart
                });
            }

            await _saleEndpoint.PostSale(sale);
        }

    }
}
