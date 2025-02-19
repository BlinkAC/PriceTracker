using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Products3.Viewmodels;

namespace Products3.Views
{
    public class BasePage : ContentPage
    {
        protected override async void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            await (this.BindingContext as BaseViewModel).Initialize();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
