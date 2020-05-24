using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using System;

using System.Collections;
using System.Data;
using System.Text.RegularExpressions;

using System.Xml;
using KoalaGo.Data;

namespace FireUpdate
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            setQAfires();
            getFiresNSW();
        }

        public static void setQAfires()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("https://www.qfes.qld.gov.au/data/alerts/bushfireAlert.xml");
            XmlNodeList nlist = doc.ChildNodes;

            XmlNodeList elemList = doc.GetElementsByTagName("entry");
            String result = "";
            ArrayList arr = new ArrayList();
            DataTable table = new DataTable();
            table.Columns.Add("lat", typeof(string));
            table.Columns.Add("lon", typeof(string));
            table.Columns.Add("date", typeof(string));

            table.Columns.Add("alerttype", typeof(string));
            table.Columns.Add("reported", typeof(string));
            table.Columns.Add("status", typeof(string));
            table.Columns.Add("details", typeof(string));
            table.Columns.Add("state", typeof(string));
            DataTable dt = new DataTable();

            DataManager manager = new DataManager();
            foreach (XmlNode xn in elemList)
            {
                XmlDocument content = new XmlDocument();

                XmlNode n = xn.LastChild;

                XmlNodeList tlist = xn.ChildNodes;

                String lat = n.InnerText.Split(' ')[0];
                String lon = n.InnerText.Split(' ')[1];
                DateTime s = Convert.ToDateTime(tlist[5].InnerText.ToString());

                String date1 = tlist[5].InnerText.Substring(0, 10);

                String desc = tlist[1].InnerText;

                string[] lines = Regex.Split(desc, "\n");

                String alerttype = lines[0].Split(':')[1];
                String reported = lines[1].Split(':')[1];
                String status = lines[2].Split(':')[1];
                String details = lines[3].Split(':')[1];
                manager.addFire(lat, lon, date1, alerttype, status, details, "QLD");
            }
            result = JsonConvert.SerializeObject(table);
        }

        public static void getFiresNSW()
        {
            XmlDocument doc = new XmlDocument();

            doc.Load("https://www.rfs.nsw.gov.au/feeds/majorIncidents.xml");
            XmlNodeList nlist = doc.ChildNodes;

            XmlNodeList elemList = doc.GetElementsByTagName("item");
            String result = "";
            ArrayList arr = new ArrayList();
            DataTable table = new DataTable();
            table.Columns.Add("lat", typeof(string));
            table.Columns.Add("lon", typeof(string));
            table.Columns.Add("date", typeof(string));

            table.Columns.Add("alerttype", typeof(string));
            table.Columns.Add("location", typeof(string));
            table.Columns.Add("council", typeof(string));
            table.Columns.Add("status", typeof(string));
            table.Columns.Add("responsibleAgency", typeof(string));
            table.Columns.Add("state", typeof(string));
            DataManager manager = new DataManager();
            foreach (XmlNode xn in elemList)
            {
                try
                {
                    XmlDocument content = new XmlDocument();

                    XmlNode n = xn.LastChild;

                    XmlNodeList tlist = xn.ChildNodes;

                    String lat = n.InnerText.Split(' ')[0];
                    String lon = n.InnerText.Split(' ')[1];
                    String date1 = tlist[5].InnerText.Substring(0, 10);
                    String desc = tlist[2].InnerText;

                    DateTime s = Convert.ToDateTime(tlist[4].InnerText.ToString());
                    string[] lines = Regex.Split(desc, "<br />");

                    String alerttype = lines[0].Split(':')[1];
                    String location = lines[1].Split(':')[1];
                    String council = lines[2].Split(':')[1];
                    String status = lines[3].Split(':')[1];
                    String responsibleAgency = lines[7].Split(':')[1];
                    manager.addFire(lat, lon, s.ToString("yyyy-MM-dd"), alerttype, status, location, "NSW");
                }
                catch (Exception ex) { }
                //// String title = nexts[4].InnerText;
                //    arr.Add(JsonConvert.SerializeObject("lat:" + lat + ",lon:" + lon + ",time:" + time1));
            }
            result = JsonConvert.SerializeObject(table);
        }
    }
}