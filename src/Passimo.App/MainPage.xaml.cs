using Passimo.Resources.Localization;
using Passimo.Cryptography.Extensions;
using System.Security;

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

            var secret = "Secret";
            var secureString = new SecureString();
            foreach (var c in secret)
            {
                secureString.AppendChar(c);
            }

            using var access = secureString.Access();
            var recoveredSecret = string.Empty;
            access.GetValue(ref recoveredSecret);

            CounterBtn.Text = recoveredSecret;
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
