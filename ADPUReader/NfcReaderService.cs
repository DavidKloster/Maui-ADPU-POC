using Android.Nfc.Tech;
using Android.Nfc;
using Android.OS;
using System;
using System.Text;
using System.Threading.Tasks;
using Android.App;

namespace ADPUReader
{
    public class NfcReaderService : Java.Lang.Object, INfcReader, NfcAdapter.IReaderCallback
    {
        private readonly NfcAdapter _nfcAdapter;
        private readonly Activity _activity;

        public NfcReaderService()
        {
            _activity = Platform.CurrentActivity ?? throw new InvalidOperationException("Current Activity is null.");
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(_activity);
            if (_nfcAdapter == null)
            {
                throw new InvalidOperationException("NFC Adapter is not available on this device.");
            }
        }

        public void StartListening()
        {
            if (_nfcAdapter.IsEnabled)
            {
                var options = new Bundle();
                _nfcAdapter.EnableReaderMode(
                    _activity,
                    this,
                    NfcReaderFlags.NfcA | NfcReaderFlags.SkipNdefCheck,
                    options
                );
            }
        }

        public void StopListening()
        {
            _nfcAdapter?.DisableReaderMode(_activity);
        }

        public async void OnTagDiscovered(Tag tag)
        {
            IsoDep isoDep = IsoDep.Get(tag);
            if (isoDep != null)
            {
                try
                {
                    isoDep.Connect();

                    // Build SELECT APDU command with corrected AID
                    byte[] command = BuildSelectApdu("F1A2B3C4D5E6"); // Ensure AID is 6 bytes
                    System.Diagnostics.Debug.WriteLine("Sending APDU: " + BitConverter.ToString(command).Replace("-", " "));

                    byte[] response = await isoDep.TransceiveAsync(command);
                    System.Diagnostics.Debug.WriteLine("Received Response: " + BitConverter.ToString(response).Replace("-", " "));

                    // Extract the response payload
                    string payload = Encoding.UTF8.GetString(response.Take(response.Length - 2).ToArray()); // Exclude status bytes
                    System.Diagnostics.Debug.WriteLine("Payload: " + payload);

                    // Notify other parts of the app
                    MessagingCenter.Send<object, string>(this, "NfcPayloadReceived", payload);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error in OnTagDiscovered: " + ex.Message);
                }
                finally
                {
                    isoDep.Close();
                }
            }
        }

        private byte[] BuildSelectApdu(string aid)
        {
            byte[] aidBytes = HexStringToByteArray(aid);
            if (aidBytes.Length != 6)
            {
                throw new ArgumentException("AID must be 6 bytes long. Provided AID: " + aid);
            }

            // Build the APDU command
            byte[] apdu = new byte[6 + aidBytes.Length];
            apdu[0] = 0x00; // CLA
            apdu[1] = 0xA4; // INS
            apdu[2] = 0x04; // P1
            apdu[3] = 0x00; // P2
            apdu[4] = (byte)aidBytes.Length; // Lc
            Array.Copy(aidBytes, 0, apdu, 5, aidBytes.Length);
            apdu[5 + aidBytes.Length] = 0x00; // Le

            return apdu;
        }

        private byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string must have an even number of characters.");
            }

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}
