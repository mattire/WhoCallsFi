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
using System.Net.Http;
using System.Threading;
using Android.Util;

namespace WhoCallsFi
{

    class KukaSoittiHandler : INumberDataSource
    {
        private Context mContext;

        //public event EventHandler<DataReadyArgs> DataReady;

        public KukaSoittiHandler(Context c) {
            mContext = c;
        }

        private string assembleUri(string number)
        {
            return "http://www.kukasoitti.com/" + number + ".html";
        }

        private async void readPage(string number, Action<string, string, INumberDataReceiver> callback, INumberDataReceiver receiver)
        {

            try
            {
                string uri = assembleUri(number);

                var startFetching = DateTime.Now;

                var hc = new HttpClient();
                Android.Util.Log.Debug("KukaSoittiHandler", uri);
                var bArray = await hc.GetByteArrayAsync(uri);
                var str = System.Text.Encoding.Default.GetString(bArray);

                var endFetching = DateTime.Now;
                var diff = endFetching - startFetching;
                Log.Debug("KukaSoittiHandler time to data", diff.Seconds.ToString());

                callback(number, str, receiver);

            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Android.Util.Log.Error("KukaSoittiHandler", ex.Message);
                callback(number, "Exception:" + ex.Message, receiver);
                
            }
        }

        private async System.Threading.Tasks.Task<string> syncReadPage(string number)
        {
            string uri = assembleUri(number);
            var hc = new HttpClient();
            var bArray = await hc.GetByteArrayAsync(uri);
            var str = System.Text.Encoding.Default.GetString(bArray);
            return str;
        }

        //INumberDataSource
        public void GetNumberData(string number, INumberDataReceiver receiver)
        {
            Thread th = new Thread(() => readPage(number, HandleResponce, receiver));
            th.Start();
        }

        private void HandleResponce(string number, string str, INumberDataReceiver receiver) {
            //Toast.MakeText(mContext, "Parsing number data", ToastLength.Long);

            var startParsing = DateTime.Now;

            NumberData nd = new NumberData();
            List<string> comments = new List<string>();

            if (str.StartsWith("Exception:")) {
                nd.number = number;
                nd.warning = str.Substring(10);
                nd.comments = comments;
                receiver.ReceiveNumberData(nd);
            }

            HtmlAgilityPack.HtmlDocument hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(str);

            var cntTxtDiv = hd.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", "") == "cnt-txt").ElementAt(0);

            string name = "", address = "", warning = "";
            

            // #text, h1, br, p, p
            if (cntTxtDiv.ChildNodes.ElementAt(3).Name == "p" && cntTxtDiv.ChildNodes.ElementAt(4).Name == "p")
            {
                name = cntTxtDiv.ChildNodes.ElementAt(3).InnerText;
                address = cntTxtDiv.ChildNodes.ElementAt(4).InnerText;
                Console.WriteLine(name + " " + address);
            }

            // #text, h1, br, p, p, p
            if (cntTxtDiv.ChildNodes.ElementAt(5).Name == "p" && cntTxtDiv.ChildNodes.ElementAt(5).Attributes["style"] != null)
            {
                warning = cntTxtDiv.ChildNodes.ElementAt(5).InnerText;
                warning = warning.Replace("&auml;", "ä");
                warning = warning.Replace("&ouml;", "ö");
                Console.WriteLine(warning);
            }

            // disqus part
            var disqus = hd.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("id", "") == "disqus_thread");
            var divs = disqus.ElementAt(0).Descendants("div");
            foreach (var div in divs)
            {
                var ps = div.Descendants("p");
                Android.Util.Log.Debug("WhoCallsFi KukaSoittiHandler", "comment:");
                //Console.WriteLine("Comment:");
                Android.Util.Log.Debug("WhoCallsFi KukaSoittiHandler", ps.ElementAt(0).InnerText);
                Android.Util.Log.Debug("WhoCallsFi KukaSoittiHandler", ps.ElementAt(1).InnerText);
                //Console.WriteLine(ps.ElementAt(1).InnerText);
                comments.Add(ps.ElementAt(0).InnerText + ":" + ps.ElementAt(1).InnerText);
            }

            nd.number= number;
            nd.name= name;
            nd.address= address;
            nd.warning= warning;
            nd.comments= comments;

            DataReadyArgs dra = new DataReadyArgs(nd);

            //DataReady?.Invoke(this, dra);

            var endParsing = DateTime.Now;
            var diff = endParsing - startParsing;
            Log.Debug("KukaSoittiHandler - time parsing", diff.Seconds.ToString());

            receiver.ReceiveNumberData(nd);
        }

    }
}