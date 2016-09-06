/*
 Author: Matti Reijonen
 */


using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using System.Linq;

namespace WhoCallsFi
{
    class WhoCallsServiceConnection : Java.Lang.Object, IServiceConnection
    {
        MainActivity activity;

        public WhoCallsServiceConnection(MainActivity activity)
        {
            this.activity = activity;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var callServiceBinder = service as WhoCallsServiceBinder;
            if (callServiceBinder != null)
            {
                activity.binder = callServiceBinder;
                activity.isBound = true;
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            activity.isBound = false;
        }
    }


    [Activity(Label = "WhoCallsFi", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        public Messenger callMessenger;
        public bool isBound = false;
        private WhoCallsServiceConnection whoCallsServiceConnection;
        internal WhoCallsServiceBinder binder;

        private IEnumerable<string> GetActiveServices()
        {
            var manager = (ActivityManager)GetSystemService(ActivityService);

            return manager.GetRunningServices(int.MaxValue).Select(
                service => service.Service.ClassName).ToList();
        }

        protected override void OnDestroy()
        {
            if (whoCallsServiceConnection != null) {
                UnbindService(whoCallsServiceConnection);
            }
            base.OnDestroy();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            Button btnStart = FindViewById<Button>(Resource.Id.btnStart);
            Button btnStop = FindViewById<Button>(Resource.Id.btnStop);

            var txtView = FindViewById<TextView>(Resource.Id.textView1);

            Button btnSimulate = FindViewById<Button>(Resource.Id.btnSimulate);
            EditText editTxt = FindViewById<EditText>(Resource.Id.editText1);

            var services = GetActiveServices();
            foreach (var s in services)
            {
                Console.WriteLine(s);
            }

            //WhoCallsService
            if (services.Any(s => s.EndsWith("WhoCallsService")))
            {
                txtView.Text = "Service is active";
            }
            else
            {
                txtView.Text = "Service is not active";
            }

            btnStart.Click += delegate {
                StartService(new Intent(this, typeof(WhoCallsService)));

            };

            btnStop.Click += delegate {
                StopService(new Intent(this, typeof(WhoCallsService)));
            };

            btnSimulate.Click += delegate{
                if (isBound == false)
                {
                    var whoCallsServiceIntent = new Intent("com.xamarin.WhoCallsService");
                    whoCallsServiceConnection = new WhoCallsServiceConnection(this);
                    BindService(whoCallsServiceIntent, whoCallsServiceConnection, Bind.AutoCreate);
                }
                else {
                    var str = editTxt.Text;
                    binder.GetWhoCallsService().SimulateCall(str);
                }

            };

        }
    }
}

