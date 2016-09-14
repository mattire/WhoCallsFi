/*
 Author: Matti Reijonen
 */


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
using Android.Telephony;
using Android.Util;
using System.Threading;
using Android.Service.Notification;

namespace WhoCallsFi
{
    [Service]
    [IntentFilter(new string[] { "com.mti.WhoCallsService" })] // can't use resources strings in attributes
    public class WhoCallsService : Service
    {
        public event EventHandler ServiceStarted;
        public event EventHandler ServiceStopped;
        const int notificationId = 0;

        WhoCallsServiceBinder binder;
        private IncomingCallReceiver mICR;
        private TelephonyManager mTelmngr;
        public bool mServiceOn;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("WhoCallsService", "WhoCallsService started");

            //StartServiceInForeground();
            
            mICR = new IncomingCallReceiver(this, new KukaSoittiHandler(this));
            mTelmngr = (TelephonyManager)base.GetSystemService(TelephonyService);
            
            mTelmngr.Listen(mICR, PhoneStateListenerFlags.CallState);

            mServiceOn = true;
            //DoWork(); // need thread to keep alive

            ServiceStarted.Invoke(this, null);

            ShowNotification();
            return StartCommandResult.Sticky;
        }

        private void ShowNotification()
        {
            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle("WhoCallsFi")
                .SetContentText("WhoCallsFi service is running")
                .SetSmallIcon(Resource.Drawable.Icon);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);

        }

        public void SimulateCall(string number) {
            mICR.simulateCallStateChanged(number);
        }

        public override void OnDestroy()
        {
            ServiceStopped.Invoke(this, null);
            mServiceOn = false;
            base.OnDestroy();
            if(mTelmngr!=null)
                mTelmngr.Dispose();
            if(mICR!=null)
                mICR.Dispose();
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Cancel(WhoCallsService.notificationId);
            Log.Debug("WhoCallsService", "WhoCallsService stopped");
        }

        public override IBinder OnBind(Intent intent)
        {
            binder = new WhoCallsServiceBinder(this);
            return binder;
        }

        void StartServiceInForeground()
        {
            var pendingIntent = PendingIntent.GetActivity(this, 0, new Intent(this, typeof(WhoCallsService)), 0);
            //Toast.MakeText(this, "Service is running in the foreground", ToastLength.Long);
            var notification = new Notification(Resource.Drawable.Icon, "WhoCallsFi running");
            //var id = ((StatusBarNotification)notification).Id;
            StartForeground((int)NotificationFlags.ForegroundService, notification);
        }



    }

    public class WhoCallsServiceBinder : Binder
    {
        WhoCallsService service;

        public WhoCallsServiceBinder(WhoCallsService service)
        {
            this.service = service;
        }

        public WhoCallsService GetWhoCallsService()
        {
            return service;
        }
    }

}