using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aruuz.Models;
using Aruuz.Controllers;
using MySql.Data.MySqlClient;
using System.Web.Routing;

namespace Aruuz.Website.Controllers
{
    public class CreateController : Controller
    {
        private bool isValid()
        {
            bool valid = false;
            string urlReferrer = "#@$@#%@$^$@#!@@#!";
            try
            {
                urlReferrer = Request.UrlReferrer.ToString();
            }
            catch
            {

            }
            MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select id from iplog where ip like '" + Request.UserHostAddress + "' and date <= '" + DateTime.Now + "' and date >= '" + DateTime.Now.Subtract(new TimeSpan(24, 0, 0)) + "';";
            MySqlDataReader dataReader = cmd.ExecuteReader();
            int count = dataReader.FieldCount;
            myConn.Close();

            if (count >= 100)
            {
                valid = false;
                MySqlConnection myConn3 = new MySqlConnection(TaqtiController.connectionString);
                myConn3.Open();
                MySqlCommand cmd3 = new MySqlCommand(TaqtiController.connectionString);
                cmd3 = myConn3.CreateCommand();
                cmd3.CommandText = "select max(id) as id from blackList;";
                MySqlDataReader dataReader3 = cmd3.ExecuteReader();
                int id3 = 0;
                while (dataReader3.Read())
                {
                    id3 = dataReader3.GetInt32(0);

                }
                myConn3.Close();
                MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();

                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "INSERT into blackList(ID,ip,referrer) VALUES (@id,@ip,@referrer)";
                cmd2.Parameters.AddWithValue("@id", id3 + 1);
                cmd2.Parameters.AddWithValue("@referrer", urlReferrer);
                cmd2.ExecuteNonQuery();
                myConn2.Close();
            }
            else
            {
                valid = true;
            }
            return valid;
        }
        public ActionResult Index()
        {
            Input inp = Session["inp"] as Input;
            if (inp == null)
            {
                return View(new Input());
            }
            else
            {
                return View(inp);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Words(Input data)
        {
            string urlReferrer = "#@$@#%@$^$@#!@@#!";
            MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select max(id) as id from iplog;";
            myConn.Open();
            MySqlDataReader dataReader = cmd.ExecuteReader();
            int id3 = 0;
            while (dataReader.Read())
            {
                id3 = dataReader.GetInt32(0);

            }
            myConn.Close();

            try
            {
                urlReferrer = Request.UrlReferrer.ToString();
            }
            catch
            {

            }

            MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
            MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
            myConn2.Open();

            cmd2 = myConn2.CreateCommand();
            cmd2.CommandText = "INSERT into iplog(ID,ip,date,data,referrer) VALUES (@id,@ip,@date,@data,@referrer)";
            cmd2.Parameters.AddWithValue("@id", id3 + 1);
            cmd2.Parameters.AddWithValue("@ip", (string)Request.UserHostAddress);
            cmd2.Parameters.AddWithValue("@date", DateTime.Now);
            cmd2.Parameters.AddWithValue("@data", data.text);
            cmd2.Parameters.AddWithValue("@referrer", urlReferrer);
            cmd2.ExecuteNonQuery();
            myConn2.Close();

            if (isValid())
            {

                Words wrd = new Words();
                wrd.word = Lines.Replace((data.text.Trim()));
                Scansion scn = new Scansion();
                Words word = new Words();
                word = scn.wordCode(wrd);
                List<string> str = new List<string>();
                for (int i = 0; i < word.code.Count; i++)
                {
                    str.Add(Meters.Rukn(word.code[i]));
                }
                word.taqti = str;
                return PartialView("_PartialWords", word);
            }
            else
            {
                return View();
            }
        }
        [ValidateAntiForgeryToken]
        public ActionResult Save(string text)
        {
            string prevline = "";
            string txt = "";
            if (!string.IsNullOrEmpty(text))
            {
                foreach (string line in text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (!prevline.Equals(line.Trim()))
                        {
                            txt += line.Trim() + "\n";
                            prevline = line.Trim();
                        }

                    }
                }
                Input inp = new Input();
                inp.text = txt;
                inp.isChecked = false;
                Session["inp"] = inp;
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        public ActionResult ReportWord(Input data)
        {
            if (string.IsNullOrEmpty(data.text))
            {
                MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select * from unassigned where word like '" + Araab.removeAraab(data.text) + "';";
                MySqlDataReader dataReader = cmd.ExecuteReader();

                if (!dataReader.HasRows) // look for existing entry in the unassigned table
                {
                    myConn.Close();
                    MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                    myConn2.Open();
                    MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                    cmd2 = myConn2.CreateCommand();
                    cmd2.CommandText = "INSERT into unassigned(word,assigned) VALUES (@word,@assigned);";
                    cmd2.Parameters.AddWithValue("@word", Araab.removeAraab(data.text));
                    cmd2.Parameters.AddWithValue("@assigned", false);

                    cmd2.ExecuteNonQuery();
                    myConn2.Close();

                }
                myConn.Close();
            }
            return View();
         }
        [ValidateAntiForgeryToken]
        public ActionResult PartialIndex(Input data)
        {
            if (!String.IsNullOrEmpty(data.text) && !String.IsNullOrEmpty(data.meter))
            {
                Input input = new Input();
                input.text = data.text;
                input.meter = data.meter;
                input.id = data.id;
                return RedirectToAction("PartialOutput", new RouteValueDictionary(input));
            }
            return View(data);
        }
        [ValidateAntiForgeryToken]
        public ActionResult PartialIndexExamples(Input data)
        {
            if (!String.IsNullOrEmpty(data.text) && !String.IsNullOrEmpty(data.meter))
            {
                Input input = new Input();
                input.text = data.text;
                input.meter = data.meter;
                input.id = data.id;
                input.isChecked = data.isChecked;
                return RedirectToAction("PartialOutputExamples", new RouteValueDictionary(input));
            }
            return View(data);
        }
        public ActionResult Output(int id)
        {
            return View(id);
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
                    return RedirectToAction("Index", "Create");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Create");
            }
        }
        [ValidateAntiForgeryToken]
        public ActionResult Result2(Input inp)
        {
            TaqtiController.fuzzy = true;
            string text = "";
            List<int> met = new List<int>();
            bool isChecked = false;
           
            text = inp.text;
            isChecked = inp.isChecked;
               
            List<scanPath> sp = new List<scanPath>();
            Scansion scn = new Scansion();
            scn.fuzzy = true;
            scn.freeVerse = false;
            scn.meter = met;
            scn.isChecked = isChecked;
            scn.errorParam = 2;
            foreach (string line in text.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    scn.addLine(new Lines(line.Trim()));
            }
            if(scn.numLines < 4)
            {
                scn.errorParam = 4;
            }
            List<scanOutputFuzzy> lst = new List<scanOutputFuzzy>();
            lst = scn.scanLinesFuzzy();

            if (lst.Count == 0)
            {
                scanOutputFuzzy sc = new scanOutputFuzzy();
                sc.identifier = -1;
                lst.Add(sc);
            }
            else
            {
                lst[0].identifier = -1;
            }

            //return RedirectToAction("Error", "Home");
            return PartialView("_PartialOutput", lst);
        }
        [ValidateAntiForgeryToken]
        public ActionResult Output2(int id)
        {
            TaqtiController.fuzzy = true;
            MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            MySqlDataReader dataReader;
            string text = "";
            string meters = "";
            int type = -1;
            List<int> met = new List<int>();
            bool isChecked = false;
            if (id > 0)
            {
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select * from InputData where id = \"" + id.ToString() + "\";";
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
                cmd.CommandText = "select * from poetry where id = \"" + (id + 65536).ToString() + "\";";
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    text = dataReader.GetString(4);
                    meters = dataReader.GetString(5);
                    type = dataReader.GetInt32(3);
                }
                char[] delimiters = new[] { ',', '،' };  // List of delimiters
                var subStrings = meters.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (var m in subStrings)
                {
                    List<int> a = Meters.meterIndex(m);
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
            }
            myConn.Close();

            List<scanPath> sp = new List<scanPath>();
            Scansion scn = new Scansion();
            scn.fuzzy = true;
            scn.freeVerse = false;
            scn.meter = met;
            scn.isChecked = isChecked;
            scn.errorParam = 2;
            foreach (string line in text.Split('\n'))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    scn.addLine(new Lines(line.Trim()));
            }
            List<scanOutputFuzzy> lst = new List<scanOutputFuzzy>();
            lst = scn.scanLinesFuzzy();

            if (lst.Count == 0)
            {
                scanOutputFuzzy sc = new scanOutputFuzzy();
                sc.identifier = id;
                lst.Add(sc);
            }
            else
            {
                lst[0].identifier = id;
            }

           //return RedirectToAction("Error", "Home");
            return PartialView("_PartialOutput", lst);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Input data)
        {
            if (!String.IsNullOrEmpty(data.text))
            {
                Input input = new Input();
                input.text = data.text;
                Session["inp"] = input;
                return RedirectToAction("Result");
            }

            return View(1);

        }
        public ActionResult PartialOutput(Input inp)
        {
            TaqtiController.fuzzy = true;
            string text = inp.text;
            bool isChecked = false;

            List<scanPath> sp = new List<scanPath>();
            Scansion scn = new Scansion();
            scn.fuzzy = true;
            scn.freeVerse = false;
            scn.errorParam = 6;
            scn.isChecked = isChecked;


            List<int> mets = new List<int>();

            char[] delimiters = new[] { ',', '،' };  // List of delimiters
            var subStrings = inp.meter.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m in subStrings)
            {
                List<int> a = Meters.meterIndex(m);
                if (a.Count != 0)
                {
                    foreach (var v in a)
                    {
                        mets.Add(v);
                    }
                }
                else
                {
                    mets.Add(-2);
                }
            }
            scn.meter = mets;


            foreach (string line in text.Split(new[] { '\n', '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    scn.addLine(new Lines(line.Trim()));
            }
            return PartialView("_AjaxPartial", scn.scanLineFuzzy(inp.id));
        }
        public ActionResult PartialOutputExamples(Input inp)
        {
            TaqtiController.fuzzy = false;
            TaqtiController.freeVerse = inp.isChecked;
            string text = inp.text;
            bool isChecked = inp.isChecked;

            List<scanPath> sp = new List<scanPath>();
            Scansion scn = new Scansion();
            scn.fuzzy = false;
            scn.freeVerse = isChecked;
            scn.errorParam = 0;
            scn.isChecked = isChecked;


            List<int> mets = new List<int>();

            char[] delimiters = new[] { ',', '،' };  // List of delimiters
            var subStrings = inp.meter.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            foreach (var m in subStrings)
            {
                List<int> a = Meters.meterIndex(m.Trim());
                if (a.Count != 0)
                {
                    foreach (var v in a)
                    {
                        mets.Add(v);
                    }
                }
                else
                {
                    mets.Add(-1);
                }
            }
            scn.meter = mets;


            foreach (string line in text.Split(new[] { '\n', '-' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!string.IsNullOrWhiteSpace(line))
                    scn.addLine(new Lines(line.Trim()));
            }
            return PartialView("_AjaxPartialExamples", scn.scanOneLine(inp.id));
        }
    }
}