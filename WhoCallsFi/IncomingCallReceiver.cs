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
using System.Threading.Tasks;

namespace WhoCallsFi
{
    class IncomingCallReceiver : PhoneStateListener, INumberDataReceiver, System.ComponentModel.ISynchronizeInvoke
    {
        private Context mContext;
        private INumberDataSource mNumberDataSource;

        private List<NumberData> mReceivedNumberData = new List<NumberData>();

        //static public IncomingCallReceiver mInstance;
        private int taskId;



        public bool InvokeRequired
        {
            get
            {
                var currentTaskId = System.Threading.Tasks.TaskScheduler.Current.Id;
                return taskId != currentTaskId;                
            }
        }

        public IncomingCallReceiver(Context context_, INumberDataSource nds)
        {
            mContext = context_;
            mNumberDataSource = nds;
            taskId = System.Threading.Tasks.TaskScheduler.Current.Id;
            mNumberDataSource.DataReady += OnDataReady;
            //mInstance = this;
        }

        private void OnDataReady(object sender, DataReadyArgs e)
        {
            this.mReceivedNumberData.Add(e.numberData);
            //if (!this.InvokeRequired)
            //{
            //    ShowDialog(e.numberData);
            //}
            //else {
            //    ShowDialog(e.numberData);
            //    Log.Debug("IncomingCallReceiver", "Blow up");
            //}
        }

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            if (state == CallState.Ringing)
            {
                //Console.WriteLine("Incommming call detected from " + incomingNumber);
                Log.Debug("IncomingCallReceiver","Incommming call detected from " + incomingNumber);

                //AlertDialog.Builder alertDialog = new AlertDialog.Builder(this.mContext, 2);
                //alertDialog.SetTitle("Title");
                //alertDialog.SetMessage("message");
                //alertDialog.SetPositiveButton("Ok", delegate
                //{
                //});
                //alertDialog.SetNegativeButton("Close", delegate
                //{
                //});
                //AlertDialog alert = alertDialog.Create();
                //alert.Window.SetType(WindowManagerTypes.SystemAlert);
                //alert.Show();

                mNumberDataSource.GetNumberData(incomingNumber, this);
                WaitForResponce();
            }
        }

        /// <summary>
        /// For Testing
        /// </summary>
        /// <param name="incomingNumber"></param>
        public void simulateCallStateChanged(string incomingNumber) {
            mNumberDataSource.GetNumberData(incomingNumber, this);
            WaitForResponce();
        }

        private void WaitForResponce()
        {
            int i = 0;
            do
            {
                i++;
                Task.Delay(200);
            } while (mReceivedNumberData.Count()==0);

            Log.Debug("IncomingCallReceiver", i.ToString());

            ShowDialog(mReceivedNumberData.ElementAt(0));
        }

        public void ReceiveNumberData(NumberData nd) {
            mReceivedNumberData.Add(nd);
            //ShowDialog(nd);
        }

        public void ShowDialog(NumberData nd)
        {
            AlertDialog.Builder alertDialog = new AlertDialog.Builder(this.mContext, 2);
            alertDialog.SetTitle("Incoming call");

            string message = nd.number + "\n"
                            + nd.name + "\n"
                            + nd.address + "\n"
                            + nd.warning + "\n"
                            + "\nComments:\n";

            //foreach (var comment in nd.comments) {
            //    message += comment + "\n";
            //}

            alertDialog.SetMessage(message);

            alertDialog.SetPositiveButton("Ok", delegate {
                //base.OnCallStateChanged(state, incomingNumber); Cause of exception?
            });
            alertDialog.SetNegativeButton("Close", delegate {

            });
            AlertDialog alert = alertDialog.Create();
            alert.Window.SetType(WindowManagerTypes.SystemAlert);
            alert.Show();
        }

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }

        public object EndInvoke(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public object Invoke(Delegate method, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}