using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Aruuz.Models;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using Aruuz.Models;

namespace Aruuz.Controllers
{
    public class ExamplesController : Controller
    {
        //
        // GET: /Examples/
        MySqlConnection myConn;
        MySqlDataReader dataReader;
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["mySqlConnection"].ConnectionString;

        public ActionResult Index(int? page)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            int maxCount = 1;
            int maxPages = 1;
            int residue = 0;
            cmd.CommandText = "select count(id) from Poetry;";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                maxCount = dataReader.GetInt32(0);
            }
            myConn.Close();
            maxPages = maxCount / 15;
            residue = maxCount - maxPages * 15;
            if (residue > 0)
            {
                maxPages = maxPages + 1;
            }
            if (page == null || page == 1)
            {
                MySqlConnection myConn2;
                MySqlDataReader dataReader2;
                myConn2 = new MySqlConnection(TaqtiController.connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "select * from Poetry order by id DESC limit 0,15";
                dataReader2 = cmd2.ExecuteReader();
                int typeId = -1; ;
                while (dataReader2.Read())
                {
                    Poetry p = new Poetry();
                    p.id = dataReader2.GetInt32(0);
                    p.poet = dataReader2.GetString(1);
                    p.title = dataReader2.GetString(2);
                    p.text = dataReader2.GetString(4);
                    typeId = dataReader2.GetInt32(3);
                    p.meters = dataReader2.GetString(5);
                    if (typeId == 0)
                    {
                        p.type = "غزل";
                    }
                    else if (typeId == 1)
                    {
                        p.type = "نظم";
                    }
                    else if (typeId == 2)
                    {
                        p.type = "رباعی";
                    }
                    else if (typeId == 3)
                    {
                        p.type = "قطعہ";
                    }
                    else if (typeId == 4)
                    {
                        p.type = "آزاد نظم";
                    }
                    else if (typeId == 5)
                    {
                        p.type = "شعر";
                    }
                    p.maxpages = maxPages;
                    p.currentPage = 1;
                    pt.Add(p);
                }
                myConn2.Close();

            }
            else
            {
                MySqlConnection myConn2;
                MySqlDataReader dataReader2;
                myConn2 = new MySqlConnection(TaqtiController.connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "select * from Poetry order by id DESC limit @init,@count";
                if (page == maxPages && residue > 0)
                {
                    cmd2.Parameters.AddWithValue("@init", (page - 1) * 15);
                    cmd2.Parameters.AddWithValue("@count", residue);
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@init", (page - 1) * 15);
                    cmd2.Parameters.AddWithValue("@count", 15);
                }
               
                dataReader2 = cmd2.ExecuteReader();
                int typeId = -1; ;
                while (dataReader2.Read())
                {
                    Poetry p = new Poetry();
                    p.id = dataReader2.GetInt32(0);
                    p.poet = dataReader2.GetString(1);
                    p.title = dataReader2.GetString(2);
                    typeId = dataReader2.GetInt32(3);
                    p.text = dataReader2.GetString(4);
                    p.meters = dataReader2.GetString(5);
                    if (typeId == 0)
                    {
                        p.type = "غزل";
                    }
                    else if (typeId == 1)
                    {
                        p.type = "نظم";
                    }
                    else if (typeId == 2)
                    {
                        p.type = "رباعی";
                    }
                    else if (typeId == 3)
                    {
                        p.type = "قطعہ";
                    }
                    else if (typeId == 4)
                    {
                        p.type = "آزاد نظم";
                    }
                    else if (typeId == 5)
                    {
                        p.type = "شعر";
                    }
                    p.maxpages = maxPages;
                    p.currentPage = (int)page;
                    pt.Add(p);
                }
                myConn2.Close();
            }
            return View(pt);

        }
        public ActionResult Poets(string poet)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            if (poet.Equals("غالب"))
            {
                cmd.CommandText = "select * from Poetry where poet like '" + "اسد اللہ خان غالب" + "' order by secid";
            }
            else
            {
                cmd.CommandText = "select * from Poetry where poet like '" + poet + "' order by secid";

            }
            dataReader = cmd.ExecuteReader();
            int typeId = -1; 
            while (dataReader.Read())
            {
                Poetry p = new Poetry();
                p.id = dataReader.GetInt32(0);
                p.poet = dataReader.GetString(1);
                p.title = dataReader.GetString(2);
                typeId = dataReader.GetInt32(3);
                p.meters = dataReader.GetString(5);
                if (typeId == 0)
                {
                    p.type = "غزل";
                }
                else if (typeId == 1)
                {
                    p.type = "نظم";
                }
                else if (typeId == 2)
                {
                    p.type = "رباعی";
                }
                else if (typeId == 3)
                {
                    p.type = "قطعہ";
                }
                else if (typeId == 4)
                {
                    p.type = "آزاد نظم";
                }
                else if (typeId == 5)
                {
                    p.type = "شعر";
                }

                pt.Add(p);
            }
            myConn.Close();


            return View(pt);
        }
        public ActionResult Filter()
        {
            List<Poets> pt = new List<Poets>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select distinct poet from Poetry;";
            dataReader = cmd.ExecuteReader();
            Poets p2 = new Poets();
            p2.poet = "تمام";
            pt.Add(p2);
            while (dataReader.Read())
            {
                Poets p = new Poets();
                p.poet = dataReader.GetString(0);
                pt.Add(p);
            }
           
            myConn.Close();

            foreach (var p in pt)
            {
                using (MySqlConnection connection = new MySqlConnection(TaqtiController.connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        if (p.poet.Equals("تمام")) {
                            command.CommandText = "select distinct type from Poetry;";
                        }
                        else
                        {
                            command.CommandText = "select distinct type from Poetry where poet like @poet;";
                            command.Parameters.AddWithValue("@poet", p.poet);
                        }
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int typeId;
                                typeId = reader.GetInt32(0);
                                if (typeId == 0)
                                {
                                    p.types.Add("غزل");
                                }
                                else if (typeId == 1)
                                {
                                   p.types.Add("نظم");
                                }
                                else if (typeId == 2)
                                {
                                    p.types.Add("رباعی");
                                }
                                else if (typeId == 3)
                                {
                                    p.types.Add("قطعہ");
                                }
                                else if (typeId == 4)
                                {
                                    p.types.Add("آزاد نظم");
                                }
                                else if (typeId == 5)
                                {
                                    p.types.Add("شعر");
                                }
                            }
                        }
                    }
                    connection.Close();
                }
                using (MySqlConnection connection = new MySqlConnection(TaqtiController.connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        if (p.poet.Equals("تمام"))
                        {
                            command.CommandText = "select distinct meterID from Poetry;";
                        }
                        else
                        {
                            command.CommandText = "select distinct meterID from Poetry where poet like @poet;";
                            command.Parameters.AddWithValue("@poet", p.poet);
                        }
                        
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string meterId;
                                meterId = reader.GetString(0);
                                char[] delimiters = new[] { ',', '،' };  // List of delimiters
                                var subStrings = meterId.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var t in subStrings)
                                {
                                    p.meters.Add(t);
                                    List<int> lst = Aruuz.Models.Meters.meterIndex(t.Trim());
                                    string Afail = "";
                                    if (lst.Count > 0)
                                    {
                                        Afail = Aruuz.Models.Meters.Afail(Aruuz.Models.Meters.meters[lst.First()]);
                                    }
                                    else
                                    {
                                        Afail = Aruuz.Models.Meters.AfailHindi(t);
                                    }
                                    p.wazn.Add(Afail);
                                }
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return View(pt);
        }
        public ActionResult Meters(string meter)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Poetry where meterID like @met order by id DESC";
            cmd.Parameters.AddWithValue("@met", "%" + meter.Replace("_", "/").Trim() + "%");
            dataReader = cmd.ExecuteReader();
            int typeId = -1; 
            while (dataReader.Read())
            {
                Poetry p = new Poetry();
                p.id = dataReader.GetInt32(0);
                p.poet = dataReader.GetString(1);
                p.title = dataReader.GetString(2);
                typeId = dataReader.GetInt32(3);
                p.meters = dataReader.GetString(5);
                if (typeId == 0)
                {
                    p.type = "غزل";
                }
                else if (typeId == 1)
                {
                    p.type = "نظم";
                }
                else if (typeId == 2)
                {
                    p.type = "رباعی";
                }
                else if (typeId == 3)
                {
                    p.type = "قطعہ";
                }
                else if (typeId == 4)
                {
                    p.type = "آزاد نظم";
                }
                else if (typeId == 5)
                {
                    p.type = "شعر";
                }
                pt.Add(p);
            }
            myConn.Close();

            if (pt.Count == 0)
            {
                return RedirectToAction("MeterNotFound","Examples");
            }
            else
            {
                return View(pt);
            }
        }
        public ActionResult MeterNotFound()
        {
            return View();
        }
        public ActionResult Poetry(int id,string searchString)
        {

            myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            Poetry p = new Poetry();
            int typeId = -1;

            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from poetry where id = @id;";
            cmd.Parameters.AddWithValue("@id", id + 65536);
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                p.searchString = searchString;
                p.id = dataReader.GetInt32(0);
                p.poet = dataReader.GetString(1);
                p.title = dataReader.GetString(2);
                typeId = dataReader.GetInt32(3);
                p.text = dataReader.GetString(4);
                p.meters = dataReader.GetString(5);

                if (typeId == 0)
                {
                    p.type = "غزل";
                }
                else if (typeId == 1)
                {
                    p.type = "نظم";
                }
                else if (typeId == 2)
                {
                    p.type = "رباعی";
                }
                else if (typeId == 3)
                {
                    p.type = "قطعہ";
                }
                else if (typeId == 4)
                {
                    p.type = "آزاد نظم";
                }
                else if (typeId == 5)
                {
                    p.type = "شعر";
                }

            }

            myConn.Close();

            return View(p);
               
        }
        public ActionResult Search(string searchString)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Poetry where text like @search  order by id DESC";
            cmd.Parameters.AddWithValue("@search","%"+searchString+"%");
            dataReader = cmd.ExecuteReader();
            int typeId = -1; ;
            while (dataReader.Read())
            {
                Poetry p = new Poetry();
                p.id = dataReader.GetInt32(0);
                p.poet = dataReader.GetString(1);
                p.title = dataReader.GetString(2);
                p.text = dataReader.GetString(4);
                typeId = dataReader.GetInt32(3);
                p.meters = dataReader.GetString(5);

                 //string pattern = searchString;
                string pattern = "^.*" + searchString + ".*$";
                Regex rg = new Regex(pattern,RegexOptions.Multiline);
                string k = "";
                int limit = 30;
                foreach (Match m in rg.Matches(p.text))
                {
                    k += m.Value.Replace(searchString, "<b>" + searchString + "</b>") + "<br>";
                    //if(m.Index > limit)
                    //{
                    //    k += "... " + p.text.Substring(m.Index - limit, limit + searchString.Length).Replace(searchString,"<b>" + searchString + "</b>") + " ... ";
                    //}
                    //else if(p.text.Length > limit + m.Index)
                    //{
                    //    k +=  p.text.Substring(0, searchString.Length + limit ).Replace(searchString, "<b>" + searchString + "</b>") + " ... ";
                   
                    //}
                    //else
                    //{
                    //    k += "... " + p.text.Substring(0, m.Index + searchString.Length).Replace(searchString, "<b>" + searchString + "</b>") + " ... ";
                   
                    //}
                }

                p.text = k;
                if (typeId == 0)
                {
                    p.type = "غزل";
                }
                else if (typeId == 1)
                {
                    p.type = "نظم";
                }
                else if (typeId == 2)
                {
                    p.type = "رباعی";
                }
                else if (typeId == 3)
                {
                    p.type = "قطعہ";
                }
                else if (typeId == 4)
                {
                    p.type = "آزاد نظم";
                }
                else if (typeId == 5)
                {
                    p.type = "شعر";
                }


                p.searchString = searchString;
                pt.Add(p);
            }
            myConn.Close();


            return View(pt);
        }
    }
}
