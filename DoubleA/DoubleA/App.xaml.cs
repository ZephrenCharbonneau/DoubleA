using DoubleA.Data;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
    public partial class App : Application
    {
        static SettingsDB settingsDatabase;

        public static SettingsDB SettingsDatabase
        {
            get
            {
                if (settingsDatabase == null)
                {
                    String settingsFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "Settings.db3");
                    settingsDatabase = new SettingsDB(settingsFilePath);
                }
                return settingsDatabase;
            }
        }
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
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
