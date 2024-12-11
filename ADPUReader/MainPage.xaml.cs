namespace ADPUReader
{
    public partial class MainPage : ContentPage
    {

        private readonly INfcReader _nfcReader;
        public MainPage()
        {
            InitializeComponent();
            _nfcReader = new NfcReaderService();
            MessagingCenter.Subscribe<object, string>(this, "NfcPayloadReceived", (sender, payload) =>
{
    // Update the UI with the received payload
    Device.BeginInvokeOnMainThread(() =>
    {
        SemanticScreenReader.Announce(payload);
            });
});
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            _nfcReader?.StartListening();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe from the message when the page is disappearing
            MessagingCenter.Unsubscribe<object, string>(this, "NfcPayloadReceived");
        }
    }

}
