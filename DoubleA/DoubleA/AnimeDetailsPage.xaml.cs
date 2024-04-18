using DoubleA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoubleA
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AnimeDetailsPage : ContentPage
    {
        private AnimeDetailed shownAnime;
        public AnimeDetailsPage(AnimeDetailed anime)
        {
            InitializeComponent();
            shownAnime = anime;
        }
    }
}