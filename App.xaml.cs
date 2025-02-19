
namespace Products3
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            //var window = base.CreateWindow(activationState);
            // Configurar ventana si es necesario
            return new Window(new AppShell());
            // return new Window(new AppShell());
        }
    }
}
