using DoubleA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        MainPage mainPage;
        public SettingsPage(ContentPage page)
        {
            InitializeComponent();

            mainPage = page as MainPage;
            if (mainPage.UserSettings.DefaultListSource == "MAL")
                rbMAL.IsChecked = true;
            else
                rbAnilist.IsChecked = true;
        }

        private void rbListSource_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMAL.IsChecked)
                mainPage.UserSettings.DefaultListSource = "MAL";
            else if (rbAnilist.IsChecked)
                mainPage.UserSettings.DefaultListSource = "Anilist";

            App.SettingsDatabase.UpdateSettingsAsync(mainPage.UserSettings);
        }

        private void btnReset_Clicked(object sender, EventArgs e)
        {
            App.SettingsDatabase.DeleteSettingsAsync();
        }
    }
}