using Aruuz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Aruuz.Controllers;
using System.Web.Mvc;
using System.Web;
using MySql.Data.MySqlClient;

namespace Aruuz.Website.Controllers
{
    public class DefaultController : ApiController
    {
       

        public IHttpActionResult GetTaqti(string text)
        {
            //string text1 = "نقش فریادی ہے کس کی شوخی تحریر کا";
           // try
            {
                MySqlConnection myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                cmd.CommandText = "select max(id) as id from InputDataAPI;";
                MySqlDataReader dataReader = cmd.ExecuteReader();
                int id3 = 0;
                while (dataReader.Read())
                {
                    id3 = dataReader.GetInt32(0);
                }
                myConn.Close();
                myConn.Open();

                cmd = myConn.CreateCommand();
                cmd.CommandText = "INSERT into InputDataAPI(ID,text,isChecked,ip) VALUES (@id,@text,@ischecked,@ip)";
                cmd.Parameters.AddWithValue("@id", id3 + 1);
                cmd.Parameters.AddWithValue("@text", (string)text);
                cmd.Parameters.AddWithValue("@ischecked", false);
                cmd.Parameters.AddWithValue("@ip", HttpContext.Current.Request.UserHostAddress);
                //cmd.Parameters.AddWithValue("@referrer", HttpContext.Current.Request.UrlReferrer);
                cmd.ExecuteNonQuery();

                myConn.Close();

                List<int> met = new List<int>();

                List<scanOutput> lst = new List<scanOutput>();
                Scansion scn = new Scansion();
                scn.fuzzy = false;
                scn.freeVerse = false;
                scn.isChecked = false;
                scn.errorParam = 2;
                scn.freeVerse = false;
                TaqtiController.isChecked = false;
                TaqtiController.fuzzy = false;
                scn.meter = met;


                foreach (string line in text.Split('\n'))
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        scn.addLine(new Lines(line.Trim()));
                }

                lst = scn.scanLines();
               


                if (lst.Count == 0)
                {
                    return Json(new { Result = "ERROR", Message = "کوئی مانوس بحر نہیں ملی" });
                }
                else
                {
                    //return Ok(new JavaScriptSerializer().Serialize(TaqtiController.convert(lst).ToArray()));
                    // }

                    //return TaqtiController.convert(lst);

                    //return Json(TaqtiController.convert(lst).Select(d => new { originalLine = d.originalLine, words = d.words, codes = d.codes, meterName = d.meterName, feet = d.feet}));
                    return Json(TaqtiController.convert(lst).First());

                }

               // return (new JavaScriptSerializer().Serialize(TaqtiController.convert(lst).ToArray()));
            }
           /* catch
            {
                return NotFound();

            }*/
           
        }

        private HttpWebResponse Json(List<scanOutputApi> list, JsonRequestBehavior jsonRequestBehavior)
        {
            throw new NotImplementedException();
        }

    }
}
