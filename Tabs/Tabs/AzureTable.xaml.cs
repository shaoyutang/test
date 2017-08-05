using System.Collections.Generic;
using Xamarin.Forms;
using Tabs.DataModels;

namespace Tabs
{
    public partial class AzureTable : ContentPage
    {

        public AzureTable()
        {
            InitializeComponent();

        }

        async void Handle_ClickedAsync(object sender, System.EventArgs e)
        {
            List<easytable> notHotDogInformation = await AzureManager.AzureManagerInstance.GetInformation();

             Tag.ItemsSource = notHotDogInformation;
        }

    }
}