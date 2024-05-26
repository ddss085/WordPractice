using WordPractice_MAUI.Views;

namespace WordPractice_MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new AppShell();
            MainPage = new FlyoutPage
            {
                Flyout = new MenuPage(),
                Detail = new NavigationPage(new MainPage())
            };
        }
    }
}
