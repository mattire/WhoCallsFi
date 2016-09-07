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
    class IncomingCallReceiver : PhoneStateListener, INumberDataReceiver
    {
        private Context mContext;
        private INumberDataSource mNumberDataSource;

        private List<NumberData> mReceivedNumberData = new List<NumberData>();
        
        private int taskId;

        private Handler mHandler = new Handler();

        public IncomingCallReceiver(Context context_, INumberDataSource nds)
        {
            mContext = context_;
            mNumberDataSource = nds;
            taskId = System.Threading.Tasks.TaskScheduler.Current.Id;
            //mNumberDataSource.DataReady += OnDataReady;
            //mInstance = this;
        }

        private void OnDataReady(object sender, DataReadyArgs e)
        {
            this.mReceivedNumberData.Add(e.numberData);
        }

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            if (state == CallState.Ringing)
            {
                Log.Debug("IncomingCallReceiver","Incommming call detected from " + incomingNumber);
                Toast.MakeText(mContext, "Started fetching number", ToastLength.Long).Show();
                mNumberDataSource.GetNumberData(incomingNumber, this);
                WaitForResponce();
            }
        }

        /// <summary>
        /// For Testing
        /// </summary>
        /// <param name="incomingNumber"></param>
        public void simulateCallStateChanged(string incomingNumber) {
            //OnCallStateChanged(CallState.Ringing, incomingNumber);
            Toast.MakeText(mContext, "Started fetching number", ToastLength.Long).Show();
            mNumberDataSource.GetNumberData(incomingNumber, this);
            WaitForResponce(); // dont fix with handler call
        }

        /// <summary>
        /// DO NOT FIX, with Handler code. Handler jams the UI, 
        /// and does not show the dialog on top of incoming call view
        /// </summary>
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

            // Do not remove, this commented code is here to remind what does not work
            //mHandler.Post(() => { ShowDialog(nd); });
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
            List<string> comments;
            comments = (nd.comments.Count() > 20) ? nd.comments.Take(20).ToList<string>() : nd.comments;

            foreach (var c in comments)
            {
                message += c + "\n";
            }

            alertDialog.SetMessage(message);

            alertDialog.SetPositiveButton("Got it", delegate {
                //base.OnCallStateChanged(state, incomingNumber); Cause of exception?
            });
            //alertDialog.SetNegativeButton("Close", delegate
            //{

            //});
            AlertDialog alert = alertDialog.Create();
            alert.Window.SetType(WindowManagerTypes.SystemAlert);
            alert.Show();
        }

    }
}