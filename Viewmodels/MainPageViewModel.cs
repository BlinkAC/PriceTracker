using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using Products3.Services;

namespace Products3.Viewmodels
{
    public partial class MainPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string rata = "alooooooooooooooooo";

        public MainPageViewModel()
        {
            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                Rata = url; // Mostrar la URL en la etiqueta
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
                //SemanticScreenReader.Announce(CounterBtn.Text);
            });
        }

        public ICommand Button1Command { get; }

        [RelayCommand]
        public void OnButton1Clicked()
        {
            Rata = "clickeado bato";
        }
        public override async Task Initialize() => await Task.Delay(0);
    }
}
