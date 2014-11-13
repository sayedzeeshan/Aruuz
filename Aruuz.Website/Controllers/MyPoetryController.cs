using Aruuz.Controllers;
using Aruuz.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Aruuz.Website.Controllers
{
    public class MyPoetryController : Controller
    {
        // GET: MyPoetry
        public ActionResult Index(int? page)
        {
            List<Publish> pt = new List<Publish>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            int maxCount = 1;
            int maxPages = 1;
            int residue = 0;
            cmd.CommandText = "select count(id) from mypoetry where publish = '1';";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                maxCount = dataReader.GetInt32(0);
            }
            myConn.Close();
            maxPages = maxCount / 18;
            residue = maxCount - maxPages * 18;
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
                cmd2.CommandText = "select * from mypoetry  where publish = '1' order by id DESC limit 0,18";
                dataReader2 = cmd2.ExecuteReader();
                while (dataReader2.Read())
                {
                    Publish p = new Publish();
                    p.id = dataReader2.GetInt32(0);
                    p.text = dataReader2.GetString(4);
                    p.name = dataReader2.GetString(1);
                    p.title = dataReader2.GetString(3);
                    try
                    {
                        p.url = dataReader2.GetString(2);
                    }
                    catch
                    {

                    }
                    try
                    {
                        p.date = dataReader2.GetDateTime(6);
                    }
                    catch
                    {

                    }
                    p.mozun = dataReader2.GetInt32(8);
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
                cmd2.CommandText = "select * from mypoetry  where publish = '1' order by id DESC limit @init,@count";
                if (page == maxPages)
                {
                    cmd2.Parameters.AddWithValue("@init", (page - 1) * 18);
                    cmd2.Parameters.AddWithValue("@count", residue);
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@init", (page - 1) * 18);
                    cmd2.Parameters.AddWithValue("@count", 18);
                }

                dataReader2 = cmd2.ExecuteReader();
                while (dataReader2.Read())
                {
                    Publish p = new Publish();
                    p.id = dataReader2.GetInt32(0);
                    p.text = dataReader2.GetString(4);
                    p.name = dataReader2.GetString(1);
                    p.title = dataReader2.GetString(3);
                    try
                    {
                        p.url = dataReader2.GetString(2);
                    }
                    catch
                    {

                    }
                    try
                    {
                        p.date = dataReader2.GetDateTime(6);
                    }
                    catch
                    {

                    }
                
                    p.mozun = dataReader2.GetInt32(8);
                    p.maxpages = maxPages;
                    p.currentPage = (int)page;
                    pt.Add(p);
                }
                myConn2.Close();
            }
            return View(pt);

        }
        public ActionResult Poetry(int id)
        {
            string taqtiObject = "";
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from mypoetry where id = @id";
            cmd.Parameters.AddWithValue("@id",id);
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
            
            return View(p);
        }
        [ValidateAntiForgeryToken]
        public ActionResult Publish(Publish data)
        {
            MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            int percentage = 0;
            if (data.percentage > 79)
            {
                percentage = 1;
            }
            if (string.IsNullOrEmpty(data.url))
            {
                cmd.CommandText = "INSERT into mypoetry(name,title,text,date,mozun) VALUES (@name,@title,@text,@date,@mozun);";
                cmd.Parameters.AddWithValue("@name", (string)data.name);
                cmd.Parameters.AddWithValue("@title", (string)data.title);
                cmd.Parameters.AddWithValue("@text", (string)data.text);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@mozun", percentage);

            }
            else
            {
                cmd.CommandText = "INSERT into mypoetry(name,url,title,text,date,mozun) VALUES (@name,@url,@title,@text,@date,@mozun);";
                cmd.Parameters.AddWithValue("@name", (string)data.name);
                cmd.Parameters.AddWithValue("@url", (string)data.url);
                cmd.Parameters.AddWithValue("@title", (string)data.title);
                cmd.Parameters.AddWithValue("@text", (string)data.text);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);
                cmd.Parameters.AddWithValue("@mozun", percentage);


            }
            cmd.ExecuteNonQuery();
            myConn.Close();
            int id = 0;
            myConn.Open();
            cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select max(id) from mypoetry;";
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                id   = dataReader.GetInt32(0);
            }
            myConn.Close();
            return PartialView("_Publish", id);
        }
    }
}