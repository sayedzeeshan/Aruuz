using Aruuz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Web.Http.Cors;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Web.Routing;

//

namespace Aruuz.Controllers
{
    public class TaqtiController : Controller
    {
        MySqlConnection myConn;
        MySqlDataReader dataReader;
        public static string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mySqlConnection"].ConnectionString;

        //public static bool fuzzy = false;
        //public static bool isChecked = false;
        //public static bool freeVerse = false;
        static  public List<scanOutputApi> convert(List<scanOutput> lst2)
        {
            List<scanOutputApi> lst = new List<scanOutputApi>();

            foreach (var m in lst2)
            {
                scanOutputApi soa = new scanOutputApi();
                soa.originalLine = m.originalLine;
                soa.meterName = m.meterName;
                soa.feet = m.feet;
                soa.codes = m.wordTaqti;
                foreach (var n in m.words)
                {
                    soa.words.Add(n.word);
                }

                lst.Add(soa);
            }

            return lst;
        }
        public ActionResult Output(int id)
        {
            Input inp2 = new Input();
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            string text = "";
            string meters = "";
            string poet = "";
            string title = "";
            int type = -1;
            List<int> met = new List<int>();
            if (id < 0)
            {
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select * from poetry where id = @id;";
                cmd.Parameters.AddWithValue("@id", id + 65536);
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    text = dataReader.GetString(4);
                    meters = dataReader.GetString(5);
                    title = dataReader.GetString(2);
                    poet = dataReader.GetString(1);
                    type = dataReader.GetInt32(3);

                }

                inp2.text = poet + " " + title + " " + meters.Replace(","," ").Replace("،"," "); 
                inp2.id = id;
                inp2.text = text;
            }
            else
            {
                inp2.id = id;
            }
            myConn.Close();
            Scansion scn = new Scansion();

            return View(inp2);
        }
        [ValidateAntiForgeryToken]
        public ActionResult Output2(int id)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            bool isChecked = false;
            MySqlCommand cmd = new MySqlCommand(connectionString);
            string text = "";
            string meters = "";
            string poet = "";
            string taqtiObject = "";
            int type = -1;
            List<int> met = new List<int>();
            if (id > 0)
            {
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select * from InputData where id = @id;";
                cmd.Parameters.AddWithValue("@id",id);
                dataReader = cmd.ExecuteReader();


                while (dataReader.Read())
                {
                    text = dataReader.GetString(1);
                    isChecked = (bool)dataReader.GetBoolean(2);
                }
            }
            else
            {
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select * from poetry where id = @id;";
                cmd.Parameters.AddWithValue("@id", id + 65536);

                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    text = dataReader.GetString(4);
                    meters = dataReader.GetString(5);
                    poet = dataReader.GetString(1);
                    type = dataReader.GetInt32(3);
                    try
                    {
                        taqtiObject = dataReader.GetString(7);
                    }
                    catch
                    {

                    }
                }
                char[] delimiters = new[] { ',','،'};  // List of delimiters
                var subStrings = meters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach(var m in subStrings)
                {
                    List<int> a = Meters.meterIndex(m.Trim());
                    if(a.Count!=0)
                    {
                        foreach(var v in a)
                        {
                            met.Add(v);
                        }
                    }
                    else
                    {
                        met.Add(-1);
                    }
                }
            }
            myConn.Close();
            Scansion scn = new Scansion();

            if (type == 4)
            {
                scn.fuzzy = false;
                scn.freeVerse = true;
                scn.isChecked = true;
                scn.errorParam = 2;
                scn.meter = met;
            }
            else
            {
                scn.fuzzy = false;
                scn.freeVerse = isChecked;
                scn.isChecked = false;
                scn.errorParam = 2;
                scn.meter = met;
            }
            List<scanOutput> lst = new List<scanOutput>();
            if (string.IsNullOrEmpty(taqtiObject))
            {
                foreach (string line in text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        scn.addLine(new Lines(line.Trim()));
                }
                lst = scn.scanLines();

                if (lst.Count == 0)
                {
                    scanOutput sc = new scanOutput();
                    sc.identifier = id;
                    sc.poet = poet;
                    lst.Add(sc);
                }
                else
                {
                    lst[0].identifier = id;
                    lst[0].poet = poet;
                }
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringWriter textWriter = new StringWriter();
                ser.Serialize(textWriter, lst);
                MySqlConnection myConn2 = new MySqlConnection(connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "update poetry set taqtiObject = @object where id = @id;";
                cmd2.Parameters.AddWithValue("@id", id + 65536);

                cmd2.Parameters.AddWithValue("@object",(string)textWriter.ToString());
                cmd2.ExecuteNonQuery();
                myConn2.Close();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringReader reader = new StringReader(taqtiObject);
                lst = (List<scanOutput>)ser.Deserialize(reader);
            }
            
             return PartialView("_PartialOutputTaqti",lst);
        }
        public ActionResult Poetry(int id)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            string text = "";
            string meters = "";
            string poet = "";
            string taqtiObject = "";
            string title = "";
            int type = -1;
            List<int> met = new List<int>();

            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from poetry where id = @id;";
            cmd.Parameters.AddWithValue("@id", id + 65536);

            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                text = dataReader.GetString(4);
                meters = dataReader.GetString(5);
                poet = dataReader.GetString(1);
                type = dataReader.GetInt32(3);
                text = dataReader.GetString(4);
                title = dataReader.GetString(2);

                try
                {
                    taqtiObject = dataReader.GetString(7);
                }
                catch
                {

                }
            }
            char[] delimiters = new[] { ',', '،' };  // List of delimiters
            var subStrings = meters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m in subStrings)
            {
                List<int> a = Meters.meterIndex(m.Trim());
                if (a.Count != 0)
                {
                    foreach (var v in a)
                    {
                        met.Add(v);
                    }
                }
                else
                {
                    met.Add(-1);
                }
            }

            myConn.Close();
            Scansion scn = new Scansion();

            if (type == 4)
            {
                scn.fuzzy = false;
                scn.freeVerse = true;
                scn.isChecked = true;
                scn.errorParam = 2;
                scn.meter = met;
            }
            else
            {
                scn.fuzzy = false;
                scn.freeVerse = false;
                scn.isChecked = false;
                scn.errorParam = 2;
                scn.meter = met;
            }
            List<scanOutput> lst = new List<scanOutput>();
            if (string.IsNullOrEmpty(taqtiObject))
            {
                foreach (string line in text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        scn.addLine(new Lines(line.Trim()));
                }
                lst = scn.scanLines();

                if (lst.Count == 0)
                {
                    scanOutput sc = new scanOutput();
                    sc.identifier = id;
                    sc.poet = poet;
                    sc.title = title;
                    lst.Add(sc);
                }
                else
                {
                    lst[0].identifier = id;
                    lst[0].poet = poet;
                    lst[0].title =  " - " + poet + " - " + title + " - " + meters.Replace(",", " ").Replace("،", " "); ;
                }
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringWriter textWriter = new StringWriter();
                ser.Serialize(textWriter, lst);
                MySqlConnection myConn2 = new MySqlConnection(connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "update poetry set taqtiObject = @object where id = @id;";
                cmd2.Parameters.AddWithValue("@object", (string)textWriter.ToString());
                cmd2.Parameters.AddWithValue("@id", id + 65536);
                cmd2.ExecuteNonQuery();
                myConn2.Close();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringReader reader = new StringReader(taqtiObject);
                lst = (List<scanOutput>)ser.Deserialize(reader);
                lst[0].identifier = id;
                lst[0].poet = poet;
                lst[0].title = " - " + poet + " - " + title + " - " + meters.Replace(",", " ").Replace("،", " "); ;
            }
            return View(lst);
        }
        public ActionResult Poetry2(int id)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            string text = "";
            string meters = "";
            string poet = "";
            string taqtiObject = "";
            string title = "";
            int type = -1;
            List<int> met = new List<int>();

            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from poetry where id = @id;";
            cmd.Parameters.AddWithValue("@id",id + 65536);
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                text = dataReader.GetString(4);
                meters = dataReader.GetString(5);
                poet = dataReader.GetString(1);
                type = dataReader.GetInt32(3);
                text = dataReader.GetString(4);
                title = dataReader.GetString(2);

                try
                {
                    taqtiObject = dataReader.GetString(7);
                }
                catch
                {

                }
            }
            char[] delimiters = new[] { ',', '،' };  // List of delimiters
            var subStrings = meters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m in subStrings)
            {
                List<int> a = Meters.meterIndex(m.Trim());
                if (a.Count != 0)
                {
                    foreach (var v in a)
                    {
                        met.Add(v);
                    }
                }
                else
                {
                    met.Add(-1);
                }
            }

            myConn.Close();
            Scansion scn = new Scansion();

            if (type == 4)
            {
                scn.fuzzy = false;
                scn.freeVerse = true;
                scn.isChecked = true;
                scn.errorParam = 2;
                scn.meter = met;
            }
            else
            {
                scn.fuzzy = false;
                scn.freeVerse = false;
                scn.isChecked = false;
                scn.errorParam = 2;
                scn.meter = met;
            }
            List<scanOutput> lst = new List<scanOutput>();
            if (string.IsNullOrEmpty(taqtiObject))
            {
                foreach (string line in text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        scn.addLine(new Lines(line.Trim()));
                }
                lst = scn.scanLines();

                if (lst.Count == 0)
                {
                    scanOutput sc = new scanOutput();
                    sc.identifier = id;
                    sc.poet = poet;
                    sc.title = title;
                    lst.Add(sc);
                }
                else
                {
                    lst[0].identifier = id;
                    lst[0].poet = poet;
                    lst[0].title = " - " + poet + " - " + title + " - " + meters.Replace(",", " ").Replace("،", " "); ;
                }
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringWriter textWriter = new StringWriter();
                ser.Serialize(textWriter, lst);
                MySqlConnection myConn2 = new MySqlConnection(connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "update poetry set taqtiObject = @object where id = @id;";
                cmd2.Parameters.AddWithValue("@object", (string)textWriter.ToString());
                cmd2.Parameters.AddWithValue("@id",id + 65536)
                cmd2.ExecuteNonQuery();
                myConn2.Close();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringReader reader = new StringReader(taqtiObject);
                lst = (List<scanOutput>)ser.Deserialize(reader);
                lst[0].identifier = id;
                lst[0].poet = poet;
                lst[0].title = " - " + poet + " - " + title + " - " + meters.Replace(",", " ").Replace("،", " "); ;
            }
            return PartialView("_Poetry",lst);
        }
        public ActionResult MyPoetry2(int id)
        {
            string taqtiObject = "";
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from mypoetry where id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            dataReader = cmd.ExecuteReader();
            Publish p = new Publish();
            while (dataReader.Read())
            {

                p.id = dataReader.GetInt32(0);
                p.text = dataReader.GetString(4);
                p.name = dataReader.GetString(1);
                p.title = dataReader.GetString(3);
                try
                {
                    taqtiObject = dataReader.GetString(5);
                }
                catch
                {

                }
                try
                {
                    p.url = dataReader.GetString(2);
                }
                catch
                {

                }
            }
            myConn.Close();
            List<int> met = new List<int>();

            Scansion scn = new Scansion();

            scn.fuzzy = false;
            scn.freeVerse = false;
            scn.isChecked = false;
            scn.errorParam = 2;
            scn.meter = met;

            List<scanOutput> lst = new List<scanOutput>();
            if (string.IsNullOrEmpty(taqtiObject))
            {
                foreach (string line in p.text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        scn.addLine(new Lines(line.Trim()));
                }
                lst = scn.scanLines();

                if (lst.Count == 0)
                {
                    scanOutput sc = new scanOutput();
                    sc.identifier = id;
                    sc.poet = p.name;
                    sc.title = p.title;
                    sc.text = p.text;
                    sc.url = p.url;
                    lst.Add(sc);
                }
                else
                {
                    lst[0].identifier = id;
                    lst[0].poet = p.name;
                    lst[0].title = p.title;
                    lst[0].text = p.text;
                    lst[0].url = p.url;
                }
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringWriter textWriter = new StringWriter();
                ser.Serialize(textWriter, lst);
                MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "update mypoetry set taqtiObject = @object where id = @id;";
                cmd2.Parameters.AddWithValue("@object", (string)textWriter.ToString());
                cmd2.Parameters.AddWithValue("@id", p.id);
                cmd2.ExecuteNonQuery();
                myConn2.Close();
            }
            else
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<scanOutput>));
                StringReader reader = new StringReader(taqtiObject);
                lst = (List<scanOutput>)ser.Deserialize(reader);
                lst[0].identifier = id;
                lst[0].poet = p.name;
                lst[0].title = p.title;
                lst[0].text = p.text;
                lst[0].url = p.url;
            }
            return PartialView("_Poetry", crunch(lst));
        }
        List<scanOutput> crunch(List<scanOutput> list)
        {
            List<scanOutput> crunchedLst = new List<scanOutput>();
            List<string> lst = new List<string>();
            foreach (var item in list)
            {
                if (lst.Count == 0)
                {
                    lst.Add(item.meterName);
                }
                bool flag = true;
                foreach (var m in lst)
                {
                    if (m.Equals(item.meterName))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    lst.Add(item.meterName);
                }
            }

            List<int> meteridList = new List<int>();
            foreach (var item in list)
            {
                if (meteridList.Count == 0)
                {
                    meteridList.Add(item.id);
                }
                bool flag = true;
                foreach (var m in meteridList)
                {
                    if (m == item.id)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    meteridList.Add(item.id);
                }
            }

            //id count calculation
            double[] scoreIdList = new double[meteridList.Count];
            int[] itemIdOrder = new int[meteridList.Count];


            for (int m = 0; m < meteridList.Count; m++)
            {
                double count = 0.0d;
                foreach (var x in list)
                {
                    if (meteridList[m] == x.id)
                    {
                        count += 1.0d;
                    }
                }
                scoreIdList[m] = 100.0 - count;
                itemIdOrder[m] = m;
            }

            Array.Sort(scoreIdList, itemIdOrder);


            //average meter score calculation
            double[] scoreList = new double[lst.Count];
            int[] itemOrder = new int[lst.Count];


            for (int m = 0; m < lst.Count; m++)
            {
                double count = 0.0d;
                foreach (var x in list)
                {
                    if (lst[m].Equals(x.meterName))
                    {
                        count += 1.0d;
                    }
                }
                scoreList[m] = 100.0 - count;
                itemOrder[m] = m;
            }

            Array.Sort(scoreList, itemOrder);

            List<string> finalList = new List<string>();

            for (int i = 0; i < lst.Count; i++)
            {
                finalList.Add(lst[itemOrder[i]]);
            }

            List<int> idList = new List<int>();
            for (int i = 0; i < finalList.Count; i++)
            {
                foreach (var m in list)
                {
                    if (m.meterName.Equals(finalList[i]))
                    {
                        idList.Add(m.id);
                        break;
                    }
                }
            }
            List<string> concatList = new List<string>();
            List<string> concatListFinal = new List<string>();
            for (int i = 0; i < 1; i++)
            {
                int count = 0;
                for (int j = 0; j < idList.Count; j++)
                {
                    if ((meteridList[itemIdOrder[i]] == idList[j]) && (idList[j] != -1))
                    {
                        idList[j] = -1;
                        count++;
                        if (count == 1)
                        {
                            concatListFinal.Add(finalList[j]);

                        }
                        else
                        {
                            concatListFinal.Add(finalList[j]);
                        }
                    }
                }

            }
            foreach (var item in list)
            {
                foreach (var v in concatListFinal)
                {
                    if (item.meterName.Equals(v))
                    {
                        crunchedLst.Add(item);
                    }
                }
            }
            return crunchedLst;
        }
        public ActionResult Result()
        {
            Input inp = Session["inp"] as Input;
            try
            {
                if (!string.IsNullOrEmpty(inp.text))
                {
                    return View(inp);
                }
                else
                {
                    return RedirectToAction("Index", "Taqti");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Taqti");
            }
        }
        [ValidateAntiForgeryToken]
        public ActionResult Result2(Input inp)
        {
            string poet = "";
            int type = -1;
            List<int> met = new List<int>();
            Scansion scn = new Scansion();

            if (type == 4)
            {
                scn.fuzzy = false;
                scn.freeVerse = true;
                scn.isChecked = true;
                scn.errorParam = 2;
                scn.meter = met;
            }
            else
            {
                scn.fuzzy = false;
                scn.freeVerse = inp.isChecked;
                scn.isChecked = false;
                scn.errorParam = 2;
                scn.meter = met;
            }

            foreach (string line in inp.text.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    scn.addLine(new Lines(line.Trim()));
            }
            List<scanOutput> lst = new List<scanOutput>();
            lst = scn.scanLines();

            if (lst.Count == 0)
            {
                scanOutput sc = new scanOutput();
                sc.identifier = -2;
                sc.poet = poet;
                lst.Add(sc);
            }
            else
            {
                lst[0].identifier = -2;
                lst[0].poet = poet;
                lst[0].numLines = scn.numLines;
            }
            return PartialView("_PartialOutputTaqti", lst);
        }
        public ActionResult Index()
        {
            Input inp = Session["inp"] as Input;
            if(inp == null)
            {
                return View(new Input());
            }
            else
            {
                return View(inp);
            }
        }
        public ActionResult Json()
        {
            return View();
        }
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Input data)
        {
            if (!String.IsNullOrEmpty(data.text))
            {
                Input input = new Input();
                input.text = data.text;
                input.isChecked = data.isChecked;
                Session["inp"] = input;
                return RedirectToAction("Result");
            }
            return View(data);
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Words(Input data)
        {

            Words wrd = new Words();
            wrd.word = Lines.Replace((data.text.Trim()));
            Scansion scn = new Scansion();
            return PartialView("_WordsLookup", scn.wordCode(wrd));
        }
        [ValidateAntiForgeryToken]
        public ActionResult Info(int id)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            string text = "";
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Info where id = @id;";
            cmd.Parameters.AddWithValue("@id",  (int)id);
            dataReader = cmd.ExecuteReader();

            Info inf = new Info();

            while (dataReader.Read())
            {
                inf.title = dataReader.GetString(1);
                inf.body = dataReader.GetString(2);
            }

            myConn.Close();

            return PartialView("_PartialInfo",inf);
        }
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public void Report(Report data)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select max(id) as id from report;";
            dataReader = cmd.ExecuteReader();
            int id3 = 0;
            while (dataReader.Read())
            {
                id3 = dataReader.GetInt32(0);
            }
            myConn.Close();
            myConn.Open();

            string comments = data.comments;
            Input inp = Session["inp"] as Input;
            if (inp != null)
            {
                comments += "\n ------------- \n" + inp.text;
            }


            cmd = myConn.CreateCommand();
            cmd.CommandText = "INSERT into report(id,inputid,name,email,comments) VALUES (@id,@inid,@name,@email,@comments)";
            cmd.Parameters.AddWithValue("@id", id3 + 1);
            cmd.Parameters.AddWithValue("@inid", (string)data.inputID);
            cmd.Parameters.AddWithValue("@name", (string)data.name);
            cmd.Parameters.AddWithValue("@email", (string)data.email);
            cmd.Parameters.AddWithValue("@comments", (string)comments);
            cmd.ExecuteNonQuery();
           
            myConn.Close();
        }
        [ValidateAntiForgeryToken]
        public void Like(string url)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from likeDislike where id like @url";
            cmd.Parameters.AddWithValue("@url", url);
            dataReader = cmd.ExecuteReader();
            string id3 = "";
            int likes = 0;
            int dislikes = 0;
            while (dataReader.Read())
            {
                id3 = dataReader.GetString(0);
                likes = dataReader.GetInt32(1);
                dislikes = dataReader.GetInt32(2);
            }
            myConn.Close();

            if (String.IsNullOrEmpty(id3))
            {
                myConn.Open();
                cmd = myConn.CreateCommand();
                cmd.CommandText = "INSERT into likeDislike(id,likes,dislikes) VALUES (@id,@likes,@dislikes)";
                cmd.Parameters.AddWithValue("@id", url);
                cmd.Parameters.AddWithValue("@likes", (int)1);
                cmd.Parameters.AddWithValue("@dislikes", (int)0);
                cmd.ExecuteNonQuery();

                myConn.Close();
            }
            else
            {
                myConn.Open();
                cmd = myConn.CreateCommand();
                cmd.CommandText = "update likeDislike set id=@id, likes@likes, dilikes = @dislikes where id like @url;";
                cmd.Parameters.AddWithValue("@id", url);
                cmd.Parameters.AddWithValue("@likes", (int)likes + 1);
                cmd.Parameters.AddWithValue("@dislikes", (int)dislikes);
                cmd.Parameters.AddWithValue("@url", url);

                cmd.ExecuteNonQuery();

                myConn.Close();
            }
        }
        [ValidateAntiForgeryToken]
        public void Dislike(string url)
        {
            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from likeDislike where id like '" + url + "';";
            dataReader = cmd.ExecuteReader();
            string id3 = "";
            int likes = 0;
            int dislikes = 0;
            while (dataReader.Read())
            {
                id3 = dataReader.GetString(0);
                likes = dataReader.GetInt32(1);
                dislikes = dataReader.GetInt32(2);
            }
            myConn.Close();

            if (String.IsNullOrEmpty(id3))
            {
                myConn.Open();
                cmd = myConn.CreateCommand();
                cmd.CommandText = "INSERT into likeDislike(id,likes,dislikes) VALUES (@id,@likes,@dislikes)";
                cmd.Parameters.AddWithValue("@id", url);
                cmd.Parameters.AddWithValue("@likes", (int)0);
                cmd.Parameters.AddWithValue("@dislikes", (int)1);
                cmd.ExecuteNonQuery();

                myConn.Close();
            }
            else
            {
                myConn.Open();
                cmd = myConn.CreateCommand();
                cmd.CommandText = "update likeDislike set likes= @likes, dislikes = @dislikes where id like '" + url + "';";
                cmd.Parameters.AddWithValue("@id", url);
                cmd.Parameters.AddWithValue("@likes", (int)likes);
                cmd.Parameters.AddWithValue("@dislikes", (int)dislikes + 1);
                cmd.ExecuteNonQuery();

                myConn.Close();
            }
        }
    }
}
