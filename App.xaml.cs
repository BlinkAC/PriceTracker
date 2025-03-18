
using Products3.Interfaces;
using Products3.States;

namespace Products3
{
    public partial class App : Application
    {
        private readonly State _state;
        private readonly IProductsDatabase _database;
        public App(State state, IProductsDatabase database)
        {
            InitializeComponent();
            _state = state;
            _database = database;
            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            var userino = await _database.GetUserInfo();
            if (userino == null)
            {
                userino = new Models.User.UserLocalData()
                {
                    DisplayName = string.Empty
                };
            }
            _state.CurrentUserInfo.Set(userino);

        }
        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    //var window = base.CreateWindow(activationState);
        //    // Configurar ventana si es necesario
        //    return new Window(new AppShell());
        //    // return new Window(new AppShell());
        //}
    }
}
