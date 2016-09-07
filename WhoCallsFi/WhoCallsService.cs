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

namespace WhoCallsFi
{
    [Service]
    [IntentFilter(new String[] { "com.xamarin.WhoCallsService" })]
    public class WhoCallsService : Service
    {
        public event EventHandler ServiceStarted;
        public event EventHandler ServiceStopped;


        WhoCallsServiceBinder binder;
        private IncomingCallReceiver mICR;
        private TelephonyManager mTelmngr;
        public bool mServiceOn;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("WhoCallsService", "WhoCallsService started");

            //StartServiceInForeground();
            
            mICR = new IncomingCallReceiver(this, new KukaSoittiHandler());
            mTelmngr = (TelephonyManager)base.GetSystemService(TelephonyService);
            mTelmngr.Listen(mICR, PhoneStateListenerFlags.CallState);

            mServiceOn = true;
            //DoWork(); // need thread to keep alive

            ServiceStarted.Invoke(this, null);

            //return StartCommandResult.NotSticky;
            return StartCommandResult.Sticky;
        }

        

        public void SimulateCall(string number) {
            mICR.simulateCallStateChanged(number);
        }

        private void DoWork()
        {
            var t = new Thread(() => {

                while (true) {
                    Thread.Sleep(14000);
                    if (!mServiceOn) {
                        break;
                    }
                }

                Log.Debug("HelloService", "Stopping foreground");
                StopForeground(true);                
                StopSelf();
            });

            t.Start();

        }

        public override void OnDestroy()
        {
            ServiceStopped.Invoke(this, null);
            mServiceOn = false;
            base.OnDestroy();
            mTelmngr.Dispose();
            mICR.Dispose();
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
            Toast.MakeText(this, "HelloService is running in the foreground", ToastLength.Long);
            var notification = new Notification(Resource.Drawable.Icon, "*WhoCallsFi service started");
            StartForeground((int)NotificationFlags.ForegroundService, notification);
        }

        internal void CloseService()
        {
            //StopForeground(true);
            StopSelf();
            OnDestroy();
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