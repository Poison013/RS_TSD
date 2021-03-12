using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RS_TSD
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Basic.B_Authorization());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
