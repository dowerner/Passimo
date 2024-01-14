using Passimo.Resources.Localization;

namespace Passimo
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-DE");
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
            LocalizationTestLabel.Text = AppResources.PasswordContainer_Description;
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
