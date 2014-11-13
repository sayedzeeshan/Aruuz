using Aruuz.Controllers;
using Aruuz.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aruuz.Website.Controllers
{
    public class ResourcesController : Controller
    {
        //
        // GET: /Resources/
        public ActionResult Index()
        {
            List<Resources> pt = new List<Resources>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Resources order by date DESC";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                Resources p = new Resources();
                p.id = dataReader.GetInt32(0);
                p.title = dataReader.GetString(1);
                p.text = dataReader.GetString(2);
                p.author = dataReader.GetString(3);
                p.date = dataReader.GetDateTime(4);
                p.category = dataReader.GetString(5);
                p.website = dataReader.GetString(6);
                pt.Add(p);
            }
            myConn.Close();


            return View(pt);
        }
        public ActionResult Article(int id)
        {
            Resources pt = new Resources();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Resources where id = '" + id + "';";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                pt.id = dataReader.GetInt32(0);
                pt.title = dataReader.GetString(1);
                pt.text = dataReader.GetString(2);
                pt.author = dataReader.GetString(3);
                pt.date = dataReader.GetDateTime(4);
                pt.category = dataReader.GetString(5);
                pt.website = dataReader.GetString(6);
                pt.keywords = dataReader.GetString(7);
            }
            myConn.Close();
            return View(pt);
        }
        public ActionResult MetersList()
        {
            List<MetersList> lst = findMeter.findMeters();
            return View(lst);
        }
	}
}