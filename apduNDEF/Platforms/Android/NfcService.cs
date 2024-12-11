using Android.Content;
using apduNDEF.Platforms.Android;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apduNDEF.Platforms.Android
{
    public class NfcService 
    {
        public void StartService()
        {
            var context = Platform.AppContext;
            var intent = new Intent(context, typeof(HceService));
            context.StartService(intent);
        }
    }
}
