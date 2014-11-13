using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Aruuz.Controllers;

namespace Aruuz.Models
{
    public class findMeter
    {

        public static bool find(string meterName)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Poetry where meterID like '%" + meterName + "%' order by id DESC";
            dataReader = cmd.ExecuteReader();
            int typeId = -1; ;
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

            if (pt.Count > 0)
                return true;
            else
                return false;
        }
    }
    public class Alphabets
    {
        static public string[] letters = { "آ","ا", "ب", "پ", "ت", "ٹ", "ث", "ج", "چ", "ح", "خ", "د", "ڈ", "ذ", "ر", "ڑ", "ز", "ژ", "س", "ش", "ص", "ض", "ط", "ظ", "ع", "غ", "ف", "ق", "ک", "گ", "ل", "م", "ن", "و", "ہ", "ء", "ی", "ے"};
        static public int assignValue(string name)
        {
            if(name.Length > 0)
            {
                for(int i = 0; i<letters.Count(); i++)
                {
                    if (name[0].Equals(letters[i]))
                        return i;
                }
            }
            else
            {
                return -1;
            }
            return 40;
        }
    }
    public class WordExeception
    {
        [Required]
        public string word { get; set; }
        [Required]
        public string taqti { get; set; }
        public string taqti2 { get; set; }
    }
    public class UserRole
    {
        [Required]
        public string id { get; set; }
        [Required]
        public string role { get; set;}
    }
    public class Info
    {
        public int id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
    public class Report
    {
        public int id { get; set; }
        public string inputID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string comments { get; set; }
    }
    public class Resources
    {
        public int id { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public string author { get; set; }
        public string website { get; set; }
        public DateTime date { get; set; }
        public string category { get; set;}
        public string keywords { get; set; }
    }
    public class Poetry
    {
        public int id { get; set; }
        [Required]     
        public string title { get; set; }
        [Required]
        public string poet { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string text { get; set; }
        [Required]
        public string meters { get; set; }
        public string searchString { get; set; }
    }
    public class Input
    {
        public string text { get; set; }
        public bool isChecked { get; set; }
        public string meter { get; set; }
        public int id;
    }
    public class Words
    {

        public List<int> id;
        public string word { get; set; }
        public List<string> language;
        public List<string> taqti;
        public List<string> taqtiWordGraft;
        public List<string> code;
        public List<string> muarrab;
        public List<string> breakup;
        public int length;
        public List<bool> isVaried;
        public bool error = false;
        public Words()
        {
            code = new List<string>();
            language = new List<string>();
            taqti = new List<string>();
            taqtiWordGraft = new List<string>();
            muarrab = new List<string>();
            breakup =  new List<string>();
            isVaried = new List<bool>();
            id = new List<int>();
        }
        public Words(Words wrd)
        {
            id = wrd.id;
            word = wrd.word;
            language = wrd.language;
            taqti = wrd.taqti;
            code = wrd.code;
            muarrab = wrd.muarrab;
            length = wrd.length;
            isVaried = wrd.isVaried;
            error = wrd.error;
        }

    }
    public class scanPath
    {
        public List<codeLocation> location;
        public List<int> meters;
        public scanPath()
        {
            location = new List<codeLocation>();
            meters = new List<int>();
        }
    }
    public class codeLocation
    {
        public string code = "";
        public int wordRef = -1;
        public int codeRef = -1;
        public string word = "";
        public int fuzzy = 0;
        public codeLocation()
        {

        }
    }
    public class Araab
    {
        static public string[] characters = { "\u0651", "\u0650", "\u0652", "\u0656", "\u0658", "\u0670", "\u064B", "\u064D", "\u064E", "\u064F", "\u0654" };
        // shadd, zer, jazr, khari zer, noon ghunna, khari zabbar, do zabar, do zer, zabar, paish, izafat
        static public int length = 11;
        static public string removeAraab(string wrdInput)
        {
            string word = wrdInput;
            if (!String.IsNullOrEmpty(word))
            {
                for (int i = 0; i < length; i++)
                {
                    word = word.Replace(characters[i], "");
                }
                return word;
            }
            else
            {
                return "";
            }
        }
    }
    public class Meters
    {
        static public int numMeters = 128;
        static public int numVariedMeters = 0;
        static public int numRubaiMeters = 12;
        static public int numSpecialMeters = 6;
        static public int[] id = 
        {13,14,15,16,17,2,2,4,4,4,4,18,19,3,3,20,21,22,23,5,5,5,24,25,26,27,6,6,6,6,30,31,32,33,34,35,35,35,35,36,40,41,42,43,44,45,46,47,48,49,50
        ,51,52,53,54,55,56,57,58,59,60,61,62,63,7,103,64,65,8,8,8,8,9,9,9,9,10,10,66,67,68,69,70,71,72,73,74,75,76,77,1,1,1,1,11,11,78,79,80,81,12,12,12,12,12
        ,82,83,84,85,86,87,88,89,90,91,92,93,94,95,36,96,97,98,99,100,101,102,103};

        static public int[] usage =
        {1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
1,
1,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
1,
0,
1,
0,
1,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
0,
0,
0,
0,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1,
1
        };

        static public string[] meters = {"-===/-===/-===/-===",
"-===/-===/-===/-==",
"-=-=/-=-=/-=-=/-=-=",
"=-=/-===+=-=/-===",
"-=-=/-===/-=-=/-===",
"==-/-==-/-==-/-===",
"==-/-===+==-/-===",
"==-/-==-/-==-/-==",
"===/==-/-==-/-==",
"==-/-===/==-/-==",
"==-/-==-/-===/==",
"-===/-===/-===",
"-===/-===/-==",
"==-/-=-=/-==",
"===/=-=/-==",
"=-=/-=-=+=-=/-=-=",
"-===/-==",
"-===/-==+-===/-==",
"==-=/==-=/==-=/==-=",
"=--=/=--=/=--=/=--=",
"=--=/-=-=+=--=/-=-=",
"-=-=/=--=+-=-=/=--=",
"==-=/==-=/==-=",
"=--=/=--=/=--=",
"=-==/=-==/=-==/=-==",
"=-==/=-==/=-==/=-=",
"=-==/--==/--==/--=",
"--==/--==/--==/--=",
"=-==/--==/--==/==",
"--==/--==/--==/==",
"--=-/=-==+--=-/=-==",
"==-/=-==+==-/=-==",
"--==/--==/--==/--==",
"=-==/=-==/=-==",
"=-==/=-==/=-=",
"=-==/--==/--=",
"=-==/--==/==",
"--==/--==/--=",
"--==/--==/==",
"--==/--==/--==",
"-==/-==/-==/-==",
"-==/-==/-==/-==/-==/-==/-==/-==",
"-==/-==/-==/-=",
"=-/-=-/-=-/-==",
"=-/-=-/-=-/-=",
"=-/-=-/-=-/-=-/-=-/-=-/-=-/-=",
"=-/-=-/-=-/-=-/-=-/-=-/-=-/-==",
"-==/-==/-==",
"-==/-==/-=",
"==/-==/==/-==",
"=-=/=-=/=-=/=-=",
"--=/--=/--=/--=",
"--=/--=/--=/--=/--=/--=/--=/--=",
"=-=/=-=/=-=/--=",
"=-=/=-=/=-=",
"=-=/-=/=-=/-=",
"--=-=/--=-=/--=-=/--=-=",
"--=-=/--=-=/--=-=",
"-=--=/-=--=/-=--=/--=-=",
"-=--=/-=--=/-=--=",
"-=--=/-=--=/-==",
"-===/=-==/-===/=-==",
"-==-/=-=-/-==-/=-=",
"==-/=-==/==-/=-==",
"==-/=-=-/-==-/=-=",
"==-/=-==/==-/=-=",
"==-/=-=-/-===",
"==-=/=-==/==-=/=-==",
"-=-=/--==/-=-=/--==",
"-=-=/===/-=-=/--==",
"-=-=/--==/-=-=/===",
"-=-=/===/-=-=/===",
"-=-=/--==/-=-=/--=",
"-=-=/===/-=-=/--=",
"-=-=/--==/-=-=/==",
"-=-=/===/-=-=/==",
"-=-=/--==/-=-=",
"-=-=/===/-=-=",
"==-=/===-/==-=/===-",
"=--=/=-=+=--=/=-=",
"=--=/=-=-/=--=/=",
"=--=/=-=/=--=",
"===-/==-=/===-/==-=",
"=-=-/=--=/=-=-/=--=",
"==-=/==-=/===-",
"=--=/=--=/=-=",
"==-=/==-=/-==",
"=-==/==-=/=-==/==-=",
"=-==/==-=/=-==",
"--==/-=-=/--==",
"=-==/-=-=/--=",
"--==/-=-=/--=",
"=-==/-=-=/==",
"--==/-=-=/==",
"=-==/-=-=/=",
"--==/-=-=/=",
"-===/-==/-===",
"-==/-===/-==/-=-=",
"-==/-=-=/-==/-=-=",
"=-==/=-=/=-==/=-=",
"--==/--=/--==/--=",
"--==/==/--==/--=",
"===/--=/--==/--=",
"--==/--=/===/--=",
"--==/--=/--==/==",
"=-==/--=/=-==/--=",
"==-=/=-=/==-=/=-=",
"-=-=/--=/-=-=/--=",
"-===/-===/=-==",
"==-/-==-/=-==",
"=-==/=-==/==-=",
"--==/--==/-=-=",
"=-==/-===/-===",
"=-=-/-==-/-==",
"-=-==/-=-==/-=-==/-=-==",
"=-=/-===",
"=-=/-=-=",
"-===/-===",
"-=-=/-=-=/-=-=/-=",
"=-==/--==/--==",
"-===/-===",
"=-==/=-==",
"=-==/=-=",
"-==/-==",
"--=-=/--=-=",
"-==/-===",
"=-==/=-=",
"-===/-===/-===/-===/-===/-===/-===/-==="
                                        };
        static public string[] meterNames = {
            "ہزج مثمن سالم",
"ہزج مثمن محذوف",
"ہزج مثمن مقبوض",
"ہزج مثمن اشتر",
"ہزج مثمن مقبوض سالم",
"ہزج مثمن اخرب مکفوف سالم",
"ہزج مثمن اخرب سالم", 
"ہزج مثمن اخرب مکفوف محذوف", 
"ہزج مثمن اخرب مکفوف محذوف",
"ہزج مثمن اخرب مکفوف محذوف",
"ہزج مثمن اخرب مکفوف محذوف",
"ہزج مسدس سالم",
"ہزج مسدس محذوف",
"ہزج مسدس اخرب مقبوض محذوف", 
"ہزج مسدس اخرم اشتر محذوف",
"ہزج مربع اشتر مقبوض مضاعف",
"ہزج مربع محذوف",
"ہزج مربع محذوف مضاعف", 
"رجز مثمن سالم",
"رجز مثمن مطوی",
"رجز مثمن مطوی مخبون", 
"رجز مثمن مخبون مطوی",
"رجز مسدس سالم",
"رجز مسدس مطوی",
"رمل مثمن سالم",
"رمل مثمن محذوف",
"رمل مثمن سالم مخبون محذوف", 
"رمل مثمن سالم مخبون محذوف",
"رمل مثمن مخبون محذوف مقطوع",
"رمل مثمن مخبون محذوف مقطوع",
"رمل مثمن مشکول",
"رمل مثمن مشکول مسکّن",
"رمل مثمن مخبون",
"رمل مسدس سالم",
"رمل مسدس محذوف",
"رمل مسدس مخبون محذوف", 
"رمل مسدس مخبون محذوف مسکن", 
"رمل مسدس مخبون محذوف", 
"رمل مسدس مخبون محذوف مسکن", 
"رمل مسدس مخبون", 
"متقارب مثمن سالم",
"متقارب مثمن سالم مضاعف", 
"متقارب مثمن محذوف",
"متقارب مثمن اثرم مقبوض", 
"متقارب مثمن اثرم مقبوض محذوف", 
"متقارب مثمن اثرم مقبوض مضاعف",
"متقارب مثمن اثرم مقبوض محذوف مضاعف", 
"متقارب مسدس سالم",
"متقارب مسدس محذوف",
"متقارب مربع اثلم سالم مضاعف", 
"متدارک مثمن سالم",
"متدارک مثمن مخبون",
"متدارک مثمن مخبون مضاعف", 
"متدارک مثمن سالم مقطوع",
"متدارک مسدس سالم",
"متدارک مربع مخلع مضاعف", 
"کامل مثمن سالم",
"کامل مسدس سالم",
"وافر مثمن سالم",
"وافر مسدس سالم",
"وافر مسدس مقطوف", 
"مضارع مثمن سالم",
"مضارع مثمن مکفوف محذوف", 
"مضارع مثمن اخرب",
"مضارع مثمن اخرب مکفوف محذوف", 
"مضارع مثمن اخرب محذوف",
"مضارع مسدس اخرب مکفوف",
"مجتث مثمن سالم",
"مجتث مثمن مخبون",
"مجتث مثمن مخبون",
"مجتث مثمن مخبون",
"مجتث مثمن مخبون",
"مجتث مثمن مخبون محذوف", 
"مجتث مثمن مخبون محذوف",
"مجتث مثمن مخبون محذوف مسکن", 
"مجتث مثمن مخبون محذوف مسکن",
"مجتث مسدس مخبون",
"مجتث مسدس مخبون",
"منسرح مثمن سالم",
"منسرح مثمن مطوی مکسوف", 
"منسرح مثمن مطوی منحور",
"منسرح مسدس مطوی مکسوف",
"مقتضب مثمن سالم",
"مقتضب مثمن مطوی",
"سریع مسدس سالم",
"سریع مسدس مطوی مکسوف", 
"سریع مسدس مخبون مکسوف ",
"خفیف مثمن سالم",
"خفیف مسدس سالم",
"خفیف مسدس مخبون",
"خفیف مسدس مخبون محذوف", 
"خفیف مسدس مخبون محذوف",
"خفیف مسدس مخبون محذوف مقطوع", 
"خفیف مسدس مخبون محذوف مقطوع",
"خفیف مسدس سالم مخبون محجوف",
"خفیف مسدس مخبون محجوف",
"طویل مثمن سالم",
"طویل مثمن سالم مقبوض", 
"طویل مثمن مقبوض",
"مدید مثمن سالم",
"مدید مثمن مخبون",
"مدید مثمن مخبون",
"مدید مثمن مخبون",
"مدید مثمن مخبون",
"مدید مثمن مخبون",
"مدید مثمن سالم مخبون", 
"بسیط مثمن سالم",
"بسیط مثمن مخبون",
"قریب مسدس سالم",
"قریب مسدس اخرب مکفوف", 
"جدید مسدس سالم",
"جدید مسدس مخبون",
"مشاکل مسدس سالم",
"مشاکل مسدس مکفوف محذوف", 
"جمیل مثمن سالم",
"ہزج مربع اشتر",
"ہزج مربع اشتر مقبوض",
"ہزج مربع سالم",
"ہزج مثمن مقبوض محذوف",
"رمل مسدس مخبون",
"ہزج مربع سالم",
"رمل مربع سالم",
"ہزج مربع محذوف",
"متقارب مربع سالم",
"کامل مربع سالم",
"طویل مربع سالم",
"مدید مربع سالم",
"ہزج مثمن سالم مضاعف"
 };

        static public string[] metersVaried = {"--==/-=-=/==","--==/-=-=/--=","--==/--==/==","--==/--==/--=","--==/--==/--==/==",
                                "--==/--==/--==/--=","--==/--==/--=="}; // Meters 14-19 have variations in which the first long syllable could be replaced by one short sylaable
        static public string[] rubaiMeters = {"==-/-==-/-==-/-=","==-/-==-/-===/=","==-/-=-=/-===/=","==-/-=-=/-==-/-=",
                                "===/=-=/-==-/-=","===/=-=/-===/=","==-/-===/===/=","==-/-===/==-/-=",
                                "===/===/==-/-=","===/===/===/=","===/==-/-===/=","===/==-/-==-/-="};
        static public string[] specialMeters = { "=(=)/=(=)/=(=)/=(=)/=(=)/=(=)/=(=)/=", "=(=)/=(=)/=(=)/=(=)/=(=)/=", "=(=)/=(=)/=(=)/=(=)/=(=)/=(=)/=(=)/==", "=(=)/=(=)/=(=)/=", "(=)=/(=)=/(=)=/(=)=/(=)=/(=)=/(=)=/(=)=", "(=)=/(=)=/(=)=/(=)=/(=)=/(=)=", "(=)=/(=)=/(=)=/(=)" };
        static public string[] feet = {"===",
"==-=",
"==-",
"==",
"=-==",
"=-=-",
"=-=",
"=--=",
"=-",
"=",
"-===",
"-==-",
"-==",
"-=-=",
"-=-",
"-=",
"--==",
"--=-=",
"--=-",
"--=",
"-=-==",
"===-",
"-=--=",
"==-=-",
"=-==-",
"=--=-",
"-===-",
"-=-=-",
"--==-",
"--=-=-",
"-=-==-",
"-=--=-"};
        static public string[] feetNames = { "مفعولن",
"مستفعلن",
 "مفعول", 
 "فِعْلن", 
 "فاعلاتن",
 "فاعلاتُ", 
 "فاعلن",
 "مفتَعِلن", 
 "فِعْل", 
 "فِع", 
 "مفاعیلن",
  "مفاعیل",
  "فعولن",
  "مفاعلن",
  "فعول", 
  "فعل",
  "فَعِلاتن", 
  "متَفاعلن",
  "فَعِلات", 
  "فَعِلن",
  "مَفاعلاتن",
  "مفعولاتُ",
  "مفاعِلَتن",
  "مستفعلان",
  "فاعلاتان",
   "مفتَعِلان",
  "مفاعیلان",
  "مفاعلان", 
  "فَعِلاتان",
  "متَفاعلان",
  "مَفاعلاتان",
  "مفاعِلَتان"};

       
        static public string[] metersVeriedNames = { "خفیف مسدّس مخبون محذوف مقطوع", "خفیف مسدّس مخبون محذوف", "رمل مسدّس مخبون محذوف مقطوع", "رمل مسدّس مخبون محذوف",
                                                       "رمل مثمّن مخبون محذوف مقطوع", "رمل مثمّن مخبون محذوف","رمل مسدس مخبون" };
        static public string[] rubaiMeterNames = { "ہزج مثمّن اخرب مکفوف مجبوب", "ہزج مثمّن اخرب مکفوف ابتر", "ہزج مثمّن اخرب مقبوض ابتر", "ہزج مثمّن اخرب مقبوض مکفوف مجبوب",
                                     "ہزج مثمّن اخرم اشتر مکفوف مجبوب", "ہزج مثمّن اخرم اشتر ابتر", "ہزج مثمّن اخرب اخرم ابتر", "ہزج مثمّن اخرب مجبوب","ہزج مثمّن اخرم اخرب مجبوب",
                                     "ہزج مثمّن اخرم ابتر","ہزج مثمّن اخرم اخرب ابتر","ہزج مثمّن اخرم اخرب مکفوف مجبوب"};
        static public string[] specialMeterNames = { "بحرِ ہندی/ متقارب مثمن مضاعف", "بحرِ ہندی/ متقارب مسدس مضاعف", "بحرِ ہندی/ متقارب اثرم مقبوض محذوف مضاعف", "بحرِ ہندی/ متقارب مربع مضاعف", "بحرِ زمزمہ/ متدارک مثمن مضاعف", "بحرِ زمزمہ/ متدارک مسدس مضاعف", "بحرِ زمزمہ/ متدارک مربع مضاعف" };
        static public string Afail(string meter)
        {
            string ft = "";
            foreach (string s in meter.Split('+'))
            {
                foreach (string foot in s.Split('/'))
                {
                    for (int i = 0; i < feet.Count(); i++)
                    {
                        if (foot.Equals(feet[i]))
                        {
                            ft += " " + feetNames[i];
                        }
                    }
                }
            }
            return ft;
        }
        static public List<Feet> Afail2(string meter, string code)
        {
            string mete = "",met1 = "", met2 = "", met3 = "", met4 = "";
            bool flag1 = false, flag2 = false, flag3 = false, flag4 = false;
            List<Feet> ft = new List<Feet>();

            met1 = meter.Replace("/", "");
            met2 = met1.Replace("+", "") + "-";
            met3 = met1.Replace("+", "-") + "-";
            met4 = met1.Replace("+", "-");
            met1 = met1.Replace("+", "");

            #region original meter
            if (met1.Length == code.Length)
            {
                for (int j = 0; j < met1.Length; j++)
                {
                    char met = met1[j];
                    char cd = code[j];
                    if (met == '-')
                    {
                        if (cd == '-' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag1 = true;
                            break;
                        }
                    }
                    else if (met == '=')
                    {
                        if (cd == '=' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag1 = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag1 = true;
            }
            
            #endregion
            #region Variation1
            met1 = met2;
            if (met1.Length == code.Length)
            {
                for (int j = 0; j < met1.Length; j++)
                {
                    char met = met1[j];
                    char cd = code[j];
                    if (met == '-')
                    {
                        if (cd == '-' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    else if (met == '=')
                    {
                        if (cd == '=' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag2 = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag2 = true;
            }
            
            #endregion
            #region Variation2
            met1 = met3;
            if (met1.Length == code.Length)
            {
                for (int j = 0; j < met1.Length; j++)
                {
                    char met = met1[j];
                    char cd = code[j];
                    if (met == '-')
                    {
                        if (cd == '-' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    else if (met == '=')
                    {
                        if (cd == '=' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag3 = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag3 = true;
            }
           
            #endregion
            #region Variation3
            met1 = met4;
            if (met1.Length == code.Length)
            {
                for (int j = 0; j < met1.Length; j++)
                {
                    char met = met1[j];
                    char cd = code[j];
                    if (met == '-')
                    {
                        if (cd == '-' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag4 = true;
                            break;
                        }
                    }
                    else if (met == '=')
                    {
                        if (cd == '=' || cd == 'x')
                        {

                        }
                        else
                        {
                            flag4 = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                flag4 = true;
            }
            
            #endregion

            if (!flag1)
            {
                mete = meter;
            }
            if (!flag2)
            {
                mete = meter + "-";
            }
            if (!flag3)
            {
                mete = meter.Replace("+", "-+") + "-";
            }
            if (!flag4)
            {
                mete = meter.Replace("+", "-+");
            }

            foreach (string s in mete.Split('+'))
            {
                foreach (string foot in s.Split('/'))
                {
                    for (int i = 0; i < feet.Count(); i++)
                    {
                        if (foot.Equals(feet[i]))
                        {
                            Feet temp = new Feet();
                            temp.foot = feetNames[i];
                            temp.code = feet[i];
                            ft.Add(temp);
                        }
                    }
                }
            }

            return ft;
        }
        static public string Rukn(string code)
        {
            string ft = "";
            code = code.Replace("x", "=");
            for (int i = 0; i < feet.Count(); i++)
            {
                if (code.Equals(feet[i]))
                {
                    ft = feetNames[i];
                    return ft;
                }
            }
            return ft;
        }
        static public string RuknCode(string name)
        {
            string ft = "";
            for (int i = 0; i < feetNames.Count(); i++)
            {
                if (name.Trim().Equals(feet[i]))
                {
                    ft = feet[i];
                    return ft;
                }
            }
            return ft;
        }
        static public List<int> meterIndex(string metName)
        {
            List<int> lst = new List<int>();
            for (int i = 0; i < numMeters; i++)
            {
                if(meterNames[i].Equals(metName))
                {
                    lst.Add(i);
                }
            }
            for (int i = 0; i < numRubaiMeters; i++)
            {
                if (rubaiMeterNames[i].Equals(metName))
                {
                    lst.Add(i + numMeters + numVariedMeters);
                }
            }
            return lst;
        }
    }
    public class Lines
    {
        public List<Words> wordsList;
        public string originalLine = "";
        public Lines(string line)
        {
            line = line.Replace(",", "").Replace("\"", "").Replace("*", "").Replace("'", "").Replace("-", "").Replace("۔", "").Replace("،", "").Replace("?", "").Replace("!", "").Replace("ؔ", "").Replace("؟", "").Replace("‘", "").Replace("(", "").Replace(")", "").Replace("؛", "").Replace(";", "").Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "").Replace("\uFEFF", "");
            originalLine = line;
            wordsList = new List<Words>();
            char[] delimiters = new[] { ',', ' '};  // List of delimiters

            foreach (string s in  originalLine.Split(delimiters, StringSplitOptions.RemoveEmptyEntries))
            {
                Words wrd = new Words();
                wrd.word = Replace(s.Trim());
                wrd.length = Araab.removeAraab(wrd.word).Length;
                if (wrd.length > 0)
                    wordsList.Add(wrd);
            }
        }
        public static string Replace(string word)
        {
            if(word.Length > 0)
            {
                if(word[word.Length-1] == 'ئ')
                {
                    word = word.Remove(word.Length - 1, 1);
                    word += "یٔ";
                }
                if (word.Length >= 2)
                {
                    word = new Regex("(\u0627)(\u0653)").Replace(word, "آ");
                    word = new Regex("(\u06C2)").Replace(word, "\u06C1\u0654");
                }
            }
            return word;
        }
    }
}