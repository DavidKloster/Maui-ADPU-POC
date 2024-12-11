using Android.App;
using Android.Nfc.CardEmulators;
using Android.OS;
using Android.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apduNDEF.Platforms.Android
{
    [Service(Exported = true, Permission = "android.permission.BIND_NFC_SERVICE")]
    [IntentFilter(new[] { "android.nfc.cardemulation.action.HOST_APDU_SERVICE" })]
    [MetaData("android.nfc.cardemulation.host_apdu_service", Resource = "@xml/apduservice")]
    public class HceService : HostApduService
    {
        private const string TAG = "YourHceService";
        private static readonly byte[] SELECT_APDU_HEADER = { 0x00, (byte)0xA4, 0x04, 0x00 };
        private static readonly byte[] RESPONSE_OK = { (byte)0x90, 0x00 };
        private static readonly byte[] RESPONSE_UNKNOWN = { (byte)0x00, (byte)0x00 };


        //public override byte[] ProcessCommandApdu(byte[] commandApdu, Bundle extras)
        //{
        //    if (commandApdu == null || commandApdu.Length < SELECT_APDU_HEADER.Length)
        //    {
        //        return RESPONSE_UNKNOWN;
        //    }

        //    // Check if the APDU is a SELECT AID command
        //    if (IsSelectAidApdu(commandApdu))
        //    {
        //        // Return the desired string payload
        //        string responseString = "Nice Payload";
        //        byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
        //        return ConcatArrays(responseBytes, RESPONSE_OK);
        //    }
        //    else
        //    {
        //        // Handle other APDU commands
        //        return RESPONSE_UNKNOWN;
        //    }
        //}

        public override byte[] ProcessCommandApdu(byte[]? commandApdu, Bundle? extras)
        {
            Log.Debug(TAG, $"Received APDU: {ByteArrayToHexString(commandApdu)}");

            if (commandApdu == null || commandApdu.Length < SELECT_APDU_HEADER.Length)
            {
                Log.Debug(TAG, "Invalid APDU received.");
                return RESPONSE_UNKNOWN;
            }

            if (IsSelectAidApdu(commandApdu))
            {
                Log.Debug(TAG, "SELECT AID command matched.");
                string responseString = "Nice Payload";
                byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
                return ConcatArrays(responseBytes, RESPONSE_OK);
            }
            else
            {
                Log.Debug(TAG, "Unknown APDU command.");
                return RESPONSE_UNKNOWN;
            }
        }

        private byte[] ConcatArrays(byte[] first, byte[] second)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));

            byte[] result = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, result, 0, first.Length);
            Buffer.BlockCopy(second, 0, result, first.Length, second.Length);
            return result;
        }


        public override void OnDeactivated(DeactivationReason reason)
        {
            switch (reason)
            {
                case DeactivationReason.LinkLoss:
                    // Handle NFC link loss
                    Log.Debug(TAG, "NFC link lost.");
                    break;
                case DeactivationReason.Deselected:
                    // Handle AID deselection
                    Log.Debug(TAG, "AID deselected.");
                    break;
                default:
                    // Handle other potential deactivation reasons
                    Log.Debug(TAG, "Deactivated for unknown reason.");
                    break;
            }
        }

        private bool IsSelectAidApdu(byte[] apdu)
        {
            if (apdu.Length < SELECT_APDU_HEADER.Length)
            {
                return false;
            }

            for (int i = 0; i < SELECT_APDU_HEADER.Length; i++)
            {
                if (apdu[i] != SELECT_APDU_HEADER[i])
                {
                    return false;
                }
            }

            return true;
        }

        private string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:X2}", b);
            }
            return hex.ToString();
        }
    }
}
