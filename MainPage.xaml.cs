using Products3.Services;
using static System.Net.Mime.MediaTypeNames;

namespace Products3
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            MessagingService.SubscribeToUrlReceivedMessage(this, url =>
            {
                CounterBtn.Text = url; // Mostrar la URL en la etiqueta
                System.Diagnostics.Debug.WriteLine("url obtenido: " + url);
                SemanticScreenReader.Announce(CounterBtn.Text);
            });
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
