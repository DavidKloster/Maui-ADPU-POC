#if ANDROID
using apduNDEF.Platforms.Android;
#endif


namespace apduNDEF
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

       #if ANDROID
        NfcService nfc;

               public MainPage(NfcService service)
        {
            InitializeComponent();
            nfc = service;  
        }
        #else
               public MainPage()
        {
            InitializeComponent();
        }
        #endif
 

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void CounterBtn_Clicked(object sender, EventArgs e)
        {

#if ANDROID
               nfc?.StartService();
#endif
         
        }
    }

}
