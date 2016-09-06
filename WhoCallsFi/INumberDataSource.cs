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

namespace WhoCallsFi
{
    public class DataReadyArgs : EventArgs
    {
        public NumberData numberData;
        public DataReadyArgs(NumberData nd) { numberData = nd; }
    }


    public class NumberData {
        public string          number;
        public string          name;
        public string          address;
        public string          warning;
        public List<string>    comments;
    }

    /// <summary>
    /// Async interface
    /// </summary>
    public interface INumberDataSource
    {
        event EventHandler<DataReadyArgs> DataReady;

        // async
        void GetNumberData(string number, INumberDataReceiver receiver);

        // sync
        void SyncGetNumberData(string number, INumberDataReceiver receiver);
    }

    public interface INumberDataReceiver
    {
        void ReceiveNumberData(NumberData nd);
    }
}