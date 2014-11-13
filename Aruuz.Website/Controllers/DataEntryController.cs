using Aruuz.Controllers;
using Aruuz.Models;
using Aruuz.Website.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Aruuz.Website.Controllers
{
    public class DataEntryController : Controller
    {
        MySqlConnection myConn;
        MySqlDataReader dataReader;
        static Input inp = new Input();

        //
        // GET: /DataEntry/
        public ActionResult Index()
        {
            if (!String.IsNullOrEmpty(User.Identity.Name))
                return RedirectToAction("Poetry");
            else
                return RedirectToAction("Error");
        }
        public ActionResult Error()
        {
            return View();
        }
        [Authorize]
        public ActionResult Poetry()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Poetry(Poetry pt)
        {
            if (User.Identity.Name.Equals("Usama") || User.Identity.Name.Equals("Mahdi") || User.Identity.Name.Equals("zeesh") || User.Identity.Name.Equals("raza")) 
            {
                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from Poetry;";
                dataReader = cmd.ExecuteReader();
                int id3 = 0;
                while (dataReader.Read())
                {
                    id3 = dataReader.GetInt32(0);
                }
                myConn.Close();
                MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();

                string type = "";

                if (pt.type.Equals("غزل"))
                {
                    type = "0";
                }
                else if (pt.type.Equals("نظم"))
                {
                    type = "1";
                }
                else if (pt.type.Equals("رباعی"))                {
                    type = "2";
                }
                else if (pt.type.Equals("قطعہ"))
                {
                    type = "3";
                }
                else if (pt.type.Equals("آزاد نظم"))
                {
                    type = "4";
                }
                else if (pt.type.Equals("شعر"))
                {
                    type = "5";
                }     
                    cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "INSERT into Poetry(ID,poet,type,title,text,meterID) VALUES (@id,@poet,@type,@title,@text,@meterID)";
                cmd2.Parameters.AddWithValue("@id", id3 + 1);
                cmd2.Parameters.AddWithValue("@poet", (string)pt.poet);
                cmd2.Parameters.AddWithValue("@type", (string)type);
                cmd2.Parameters.AddWithValue("@title", (string)pt.title);
                cmd2.Parameters.AddWithValue("@text", (string)pt.text);
                cmd2.Parameters.AddWithValue("@meterID", (string)pt.meters);

                cmd2.ExecuteNonQuery();
                myConn2.Close();
                // TempData["Inp"] = data;

                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from dataEntry;";
                dataReader = cmd.ExecuteReader();
                int id4 = 0;
                while (dataReader.Read())
                {
                    id4 = dataReader.GetInt32(0);
                }
                myConn.Close();
                myConn2 = new MySqlConnection(TaqtiController.connectionString);
                cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();

                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "INSERT into dataEntry(ID,name,whichtable,whichid,time) VALUES (@id,@name,@table,@tableid,@time)";
                cmd2.Parameters.AddWithValue("@id", id4 + 1);
                cmd2.Parameters.AddWithValue("@name", (string)User.Identity.Name);
                cmd2.Parameters.AddWithValue("@table", (string)"Poetry");
                cmd2.Parameters.AddWithValue("@tableid", id3);
                cmd2.Parameters.AddWithValue("@time", DateTime.Now);
                cmd2.ExecuteNonQuery();
                myConn2.Close();
               



                return RedirectToAction("Poetry");
            }
            else
            {
                return RedirectToAction("Error");

            }
        }
        [Authorize]
        public ActionResult Word()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Word(WordExeception wd)
        {
            if (User.Identity.Name.Equals("zeesh"))
            {
                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from Exceptions;";
                dataReader = cmd.ExecuteReader();
                int id3 = 0;
                while (dataReader.Read())
                {
                    id3 = dataReader.GetInt32(0);
                }
                myConn.Close();
                MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();
                cmd2 = myConn2.CreateCommand();

                if(!String.IsNullOrEmpty(wd.taqti2))
                {
                    cmd2.CommandText = "INSERT into Exceptions(ID,word,taqti,taqti2) VALUES (@id,@word,@taqti1,@taqti2)";
                    cmd2.Parameters.AddWithValue("@id", id3 + 1);
                    cmd2.Parameters.AddWithValue("@word", (string)wd.word);
                    cmd2.Parameters.AddWithValue("@taqti1", (string)wd.taqti);
                    cmd2.Parameters.AddWithValue("@taqti2", (string)wd.taqti2);
                }
                else
                {
                    cmd2.CommandText = "INSERT into Exceptions(ID,word,taqti) VALUES (@id,@word,@taqti1)";
                    cmd2.Parameters.AddWithValue("@id", id3 + 1);
                    cmd2.Parameters.AddWithValue("@word", (string)wd.word);
                    cmd2.Parameters.AddWithValue("@taqti1", (string)wd.taqti);
                }

                cmd2.ExecuteNonQuery();
                myConn2.Close();



                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from dataEntry;";
                dataReader = cmd.ExecuteReader();
                int id4 = 0;
                while (dataReader.Read())
                {
                    id4 = dataReader.GetInt32(0);
                }
                myConn.Close();
                myConn2 = new MySqlConnection(TaqtiController.connectionString);
                cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();
                
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "INSERT into dataEntry(ID,name,whichtable,whichid,time) VALUES (@id,@name,@table,@tableid,@time)";
                cmd2.Parameters.AddWithValue("@id", id4 + 1);
                cmd2.Parameters.AddWithValue("@name", (string)User.Identity.Name);
                cmd2.Parameters.AddWithValue("@table", (string)"Exceptions");
                cmd2.Parameters.AddWithValue("@tableid", id3);
                cmd2.Parameters.AddWithValue("@time", DateTime.Now);
                cmd2.ExecuteNonQuery();
                myConn2.Close();

                return RedirectToAction("Word");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        [Authorize]
        public ActionResult Unassigned()
        {
            WordExeception wd = new WordExeception();
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from unassigned where assigned = false order by id desc;";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                wd.id = dataReader.GetInt32(0);
               wd.word = dataReader.GetString(1);
            }
            myConn.Close();
            return View(wd);
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Unassigned(WordExeception wd)
        {
            if (User.Identity.Name.Equals("zeesh"))
            {
                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from Exceptions;";
                dataReader = cmd.ExecuteReader();
                int id3 = 0;
                while (dataReader.Read())
                {
                    id3 = dataReader.GetInt32(0);
                }
                myConn.Close();
                MySqlConnection myConn2 = new MySqlConnection(TaqtiController.connectionString);
                MySqlCommand cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();
                cmd2 = myConn2.CreateCommand();

                if (!String.IsNullOrEmpty(wd.taqti2))
                {
                    if (wd.add)
                    {
                        cmd2.CommandText = "INSERT into Exceptions(ID,word,taqti,taqti2) VALUES (@id,@word,@taqti1,@taqti2)";
                        cmd2.Parameters.AddWithValue("@id", id3 + 1);
                        cmd2.Parameters.AddWithValue("@word", (string)wd.word);
                        cmd2.Parameters.AddWithValue("@taqti1", (string)wd.taqti);
                        cmd2.Parameters.AddWithValue("@taqti2", (string)wd.taqti2);
                        cmd2.ExecuteNonQuery();

                    }
                }
                else
                {
                    if (wd.add)
                    {
                        cmd2.CommandText = "INSERT into Exceptions(ID,word,taqti) VALUES (@id,@word,@taqti1)";
                        cmd2.Parameters.AddWithValue("@id", id3 + 1);
                        cmd2.Parameters.AddWithValue("@word", (string)wd.word);
                        cmd2.Parameters.AddWithValue("@taqti1", (string)wd.taqti);
                        cmd2.ExecuteNonQuery();

                    }
                }

                myConn2.Close();





                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from dataEntry;";
                dataReader = cmd.ExecuteReader();
                int id4 = 0;
                while (dataReader.Read())
                {
                    id4 = dataReader.GetInt32(0);
                }
                myConn.Close();
                myConn2 = new MySqlConnection(TaqtiController.connectionString);
                cmd2 = new MySqlCommand(TaqtiController.connectionString);
                myConn2.Open();

                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "INSERT into dataEntry(ID,name,whichtable,whichid,time) VALUES (@id,@name,@table,@tableid,@time)";
                cmd2.Parameters.AddWithValue("@id", id4 + 1);
                cmd2.Parameters.AddWithValue("@name", (string)User.Identity.Name);
                cmd2.Parameters.AddWithValue("@table", (string)"Exceptions");
                cmd2.Parameters.AddWithValue("@tableid", id3);
                cmd2.Parameters.AddWithValue("@time", DateTime.Now);
                cmd2.ExecuteNonQuery();
                myConn2.Close();

                MySqlConnection myConn3 = new MySqlConnection(TaqtiController.connectionString);
                MySqlCommand cmd3 = new MySqlCommand(TaqtiController.connectionString);

                myConn3.Open();

                cmd3 = myConn3.CreateCommand();
                cmd3.CommandText = "UPDATE unassigned set assigned = @assigned where id = @id;";
                cmd3.Parameters.AddWithValue("@assigned", true);
                cmd3.Parameters.AddWithValue("@id", (int)wd.id);
                cmd3.ExecuteNonQuery();
                myConn3.Close();


                return RedirectToAction("Unassigned");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRole()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignRole(UserRole role)
        
        {
            using (var context = new ApplicationDbContext())
            {
                if (ModelState.IsValid)
                {

                    var roleStore = new RoleStore<IdentityRole>(context);
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    var userStore = new UserStore<ApplicationUser>(context);

                    var userManager = new UserManager<ApplicationUser>(userStore);
                    userManager.AddToRole("usamasarsari786", "Admin");

                }
                return View();
            }
        }

    }


}