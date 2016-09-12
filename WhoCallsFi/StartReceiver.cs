using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace WhoCallsFi
{

    /// <summary>
    /// Not currently working the service needs to be restarted after every boot up
    /// </summary>
    [BroadcastReceiver]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    class StartReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Intent-action: " + intent.Action, ToastLength.Long).Show();

            if ((intent.Action != null) &&
                        (intent.Action == Android.Content.Intent.ActionBootCompleted))
            {
                //Intent whoCallsServiceIntent = new Intent(context, typeof(WhoCallsService));
                //context.StartService(whoCallsServiceIntent);
                Android.Util.Log.Debug("WhoCallsFi StartReceiver", "in OnReceive");
            }
        }
    }
}