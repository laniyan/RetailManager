using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RetailManagerWPFGUI.Annotations;


namespace RetailManagerWPFGUI.Models
{
    public class CartItemDisplayModel : INotifyPropertyChanged//this is from .net not caliburn and micro
    {
        public ProductDisplayModel Product { get; set; }

        private int _quantityInCart;

        public int QuantityInCart
        {
            get { return _quantityInCart; }
            set
            {
                _quantityInCart = value;
                CallPropertyChanged(nameof(QuantityInCart));
                CallPropertyChanged(nameof(DisplayText));
            }
        }


        public string DisplayText
        {
            get
            {
                return $"{Product.ProductName} ({QuantityInCart})";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;// now I just need to fire this event of when ever a prop changes

        public void CallPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
