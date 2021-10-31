using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;


namespace UrlShortenerTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private UrlApiClient _apiClient = new UrlApiClient("http://localhost:5000/");
        private static readonly Regex _onlyDigitRegex = new Regex("[^0-9]+");
        //regex that matches disallowed text

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private async void GetOriginalButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!NonEmptyString(shortUrlTextBox.Text))
            {
                return;
            }

            var originalUrlResponse = await _apiClient.AnonymousAsync(shortUrlTextBox.Text);
            longUrlTextBox.Text = originalUrlResponse.LongUrl;
        }

        private async void ShortenUrlButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!NonEmptyString(longUrlTextBox.Text))
            {
                return;
            }

            string ttlText = ttlTextBox.Text;

            TimeSpan ttl = null;

            if (NonEmptyString(ttlText) && Int32.Parse(ttlText) > 0)
            {
                ttl = new TimeSpan
                {
                    Hours = Int32.Parse(ttlText)
                };
            }

            LongUrlCommand longUrlCommand = new ()
            {
                CustomAlias = NonEmptyString(customAliasTextBox.Text) ? customAliasTextBox.Text : null,
                OriginalUrl = longUrlTextBox.Text,
                TimeToLive = ttl
            };

            var shortUrlResponse  = await _apiClient.ShortenUrlAsync(longUrlCommand);

            shortUrlTextBox.Text = shortUrlResponse.ShortUrl;
        }

        private static bool NonEmptyString(string text)
        {
            return !string.IsNullOrWhiteSpace(text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _onlyDigitRegex.IsMatch(e.Text);
        }
    }
}
