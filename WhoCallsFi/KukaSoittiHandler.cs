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

namespace WhoCallsFi
{

    class KukaSoittiHandler : INumberDataSource
    {
        //public event EventHandler<DataReadyArgs> DataReady;

        private string assembleUri(string number)
        {
            return "http://www.kukasoitti.com/" + number + ".html";
        }

        private async void readPage(string number, Action<string, string, INumberDataReceiver> callback, INumberDataReceiver receiver)
        {
            string uri = assembleUri(number);
            var hc = new HttpClient();
            var bArray = await hc.GetByteArrayAsync(uri);
            var str = System.Text.Encoding.Default.GetString(bArray);
            callback(number, str, receiver);
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
            Thread th = new Thread(() => this.readPage(number, HandleResponce, receiver));
            th.Start();
        }

        private void HandleResponce(string number, string str, INumberDataReceiver receiver) {
            NumberData nd = new NumberData();

            HtmlAgilityPack.HtmlDocument hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(str);

            var cntTxtDiv = hd.DocumentNode.Descendants("div").Where(div => div.GetAttributeValue("class", "") == "cnt-txt").ElementAt(0);

            string name = "", address = "", warning = "";
            List<string> comments = new List<string>();

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
                Console.WriteLine("Comment:");
                Console.WriteLine(ps.ElementAt(0).InnerText);
                Console.WriteLine(ps.ElementAt(1).InnerText);
                comments.Add(ps.ElementAt(0).InnerText + ":" + ps.ElementAt(1).InnerText);
            }

            nd.number= number;
            nd.name= name;
            nd.address= address;
            nd.warning= warning;
            nd.comments= comments;

            DataReadyArgs dra = new DataReadyArgs(nd);


            //DataReady?.Invoke(this, dra);

            receiver.ReceiveNumberData(nd);
        }

    }
}