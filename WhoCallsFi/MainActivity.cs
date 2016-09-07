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
using System.Threading;

namespace WhoCallsFi
{
    class WhoCallsServiceConnection : Java.Lang.Object, IServiceConnection
    {
        MainActivity activity;
        private bool runStart;

        public WhoCallsServiceConnection(MainActivity activity, bool runStart = false)
        {
            this.activity = activity;
            this.runStart = runStart;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var callServiceBinder = service as WhoCallsServiceBinder;
            if (callServiceBinder != null)
            {
                activity.binder = callServiceBinder;
                activity.isBound = true;
                activity.binder.GetWhoCallsService().ServiceStarted += activity.MainActivity_ServiceStarted;
                activity.binder.GetWhoCallsService().ServiceStopped += activity.MainActivity_ServiceStopped;
                if (runStart) {
                    activity.StartService(new Intent(activity, typeof(WhoCallsService)));
                }
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
        private TextView txtView;

        private IEnumerable<string> GetActiveServices()
        {
            var manager = (ActivityManager)GetSystemService(ActivityService);
            
            return manager.GetRunningServices(int.MaxValue).Select(
                service => service.Service.ClassName).ToList();
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.Clear();
            MenuInflater.Inflate(Resource.Layout.Menu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                //case Resource.Id.test:
                //    //open other view (activity)
                //    return true;
                case Resource.Id.about:
                    showMenuDialog();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void showMenuDialog()
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("Caller info app");
            alertDialog.SetMessage("Author: Matti Reijonen\n"+
                                   "(c) Matkalla Työkkäriin Industries\n"+
                                   "7th September 2016\n\n" +
                                   "Version: 0.5.2\n" +
                                   "Code name: Mämmikoura\n\n" +
                                   "savvy?");
            alertDialog.SetPositiveButton("Ok, then", delegate { });
            AlertDialog alert = alertDialog.Create();
            alert.Window.SetType(WindowManagerTypes.ApplicationAttachedDialog);
            alert.Show();
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

            txtView = FindViewById<TextView>(Resource.Id.textView1);

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
                if (isBound == false)
                {
                    var whoCallsServiceIntent1 = new Intent("com.mti.WhoCallsService");
                    whoCallsServiceConnection = new WhoCallsServiceConnection(this, true);
                    BindService(whoCallsServiceIntent1, whoCallsServiceConnection, Bind.AutoCreate);
                    Thread.Sleep(1000);
                }
                else
                {
                    StartService(new Intent(this, typeof(WhoCallsService)));
                }
            };

            btnStop.Click += delegate {
                if (txtView.Text == "Service is not active") {
                    Toast.MakeText(this, "SERVICE ALREADY STOPPED", ToastLength.Long).Show();
                } else {
                    txtView.Text = "Service will close on app close";
                    UnbindService(whoCallsServiceConnection);
                    isBound = false;
                    StopService(new Intent(this, typeof(WhoCallsService)));
                    whoCallsServiceConnection.Dispose();
                    whoCallsServiceConnection = null;
                }
            };

            btnSimulate.Click += delegate{
                if (txtView.Text == "Service is active")
                {
                    if (isBound == false)
                    {
                        var whoCallsServiceIntent1 = new Intent("com.mti.WhoCallsService");
                        whoCallsServiceConnection = new WhoCallsServiceConnection(this);
                        BindService(whoCallsServiceIntent1, whoCallsServiceConnection, Bind.AutoCreate);
                    }
                    else {
                        var str = editTxt.Text;
                        binder.GetWhoCallsService().SimulateCall(str);
                    }
                }
                else
                {
                    Toast.MakeText(this, "YOU MUST START SERVICE FIRST!!", ToastLength.Long).Show();
                }
            };

            var whoCallsServiceIntent = new Intent("com.mti.WhoCallsService");
            whoCallsServiceConnection = new WhoCallsServiceConnection(this);
            BindService(whoCallsServiceIntent, whoCallsServiceConnection, Bind.AutoCreate);
        }

        protected override void OnStart()
        {
            base.OnStart();

        }

        public void MainActivity_ServiceStopped(object sender, EventArgs e)
        {
            txtView.Text = "Service is not active";
        }

        public void MainActivity_ServiceStarted(object sender, EventArgs e)
        {
            txtView.Text = "Service is active";
        }
    }
}

