using System;
using Xamarin.Forms;

namespace AnnexioWebApp
{
    public partial class LoaderView : ContentView
    {
        public LoaderView()
        {
            InitializeComponent();
        }

        public event EventHandler<EventArgs> ReloadLastPageEvent;
        private void OnReloadLastPageEvent()
        {
            ReloadLastPageEvent?.Invoke(this, EventArgs.Empty);
        }

        private void ReloadButton_Clicked(object sender, System.EventArgs e)
        {
            OnReloadLastPageEvent();
        }
    }
}
