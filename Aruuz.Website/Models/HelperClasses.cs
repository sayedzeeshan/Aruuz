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
    public class Feet
    {
        public string foot;
        public string code;
        public string words;
    }
    public class scanOutputApi
    {
        public string originalLine;
        public List<string> words;
        public List<string> codes;
        public string meterName;
        public string feet;

        public scanOutputApi()
        {
            words = new List<string>();
            codes = new List<string>();
        }
    }
    public class scanOutput
    {
        public string originalLine;
        public List<Words> words;
        public List<Feet> feetList;
        public List<string> wordTaqti;
        public List<string> wordMuarrab;
        public string feet;
        public string meterName;
        public int id = 0;
        public int identifier = -1;
        public string poet = "";
        public string title = "";
        public string text = "";
        public string url = "";
        public int numLines = 0;

        public scanOutput()
        {
            words = new List<Words>();
            feetList = new List<Feet>();
            wordTaqti = new List<string>();
            wordMuarrab = new List<string>();
        }
    }


    public class scanOutputFuzzy
    {
        public string originalLine;
        public List<Words> words;
        public List<bool> error;
        public List<string> wordTaqti;
        public List<string> orignalTaqti;
        public string feet;
        public string meterName;
        public List<string> meterSyllables;
        public List<string> codeSyllables;
        public Input inp;
        public int score;
        public int id = 0;
        public int identifier = -1;
        public bool hidden = false;

        public scanOutputFuzzy()
        {
            words = new List<Words>();
            wordTaqti = new List<string>();
            meterSyllables = new List<string>();
            codeSyllables = new List<string>();
            orignalTaqti = new List<string>();
            inp = new Input();
            score = 10;
        }
    }
    public class Poets{
        public string poet;
        public List<string> meters;
        public List<string> types;
        public List<string> wazn;
        public Poets()
        {
            meters = new List<string>();
            types = new List<string>();
            wazn = new List<string>();
        }
    }
    public class Pagination
    {
        public int currentPage { get; set; }
        public int maxPages { get; set; }
        public string baseUrl { get; set; }
    }
    public class Publish
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public int mozun { get; set; }
        public int percentage { get; set; }
        public int maxpages { get; set; }
        public int currentPage { get; set; }
        public DateTime date { get; set; }
    }
    public class findMeter
    {
        public static List<Publish> returnMyPoetry()
        {
            List<Publish> pt = new List<Publish>();
            //MySqlConnection myConn;
            //MySqlDataReader dataReader;
            //myConn = new MySqlConnection(TaqtiController.connectionString);
            //myConn.Open();
            //MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            //cmd = myConn.CreateCommand();
            //cmd.CommandText = "select * from mypoetry where publish = '1' and mozun = '1' order by id DESC  limit 0,4;";
            //dataReader = cmd.ExecuteReader();
            //while (dataReader.Read())
            //{
            //    Publish p = new Publish();
            //    p.id = dataReader.GetInt32(0);
            //    p.text = dataReader.GetString(4);
            //    p.name = dataReader.GetString(1);
            //    p.title = dataReader.GetString(3);
            //    try
            //    {
            //        p.url = dataReader.GetString(2);
            //    }
            //    catch
            //    {

            //    }
            //    pt.Add(p);
            //    p.mozun = dataReader.GetInt32(8);
            //}
            //myConn.Close();
            return pt;
        }
        public static List<Poetry> returnPoetry(string meterName)
        {
            List<Poetry> pt = new List<Poetry>();
            if (!string.IsNullOrEmpty(meterName))
            {
                MySqlConnection myConn;
                MySqlDataReader dataReader;
                myConn = new MySqlConnection(TaqtiController.connectionString);
                myConn.Open();
                MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
                cmd = myConn.CreateCommand();
                //cmd.CommandText = "select * from Poetry where meterID like '%" + meterName + "%' order by RAND() limit 4";
                cmd.CommandText = "SELECT  a.* FROM  poetry a INNER JOIN  (SELECT title,poet,meterID FROM poetry  where meterID like '%@meterName%'  GROUP BY poet order by rand()) b ON a.title = b.title and a.meterID = b.meterID limit 0,4";
                cmd.Parameters.AddWithValue("@meterName", meterName.Trim());
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
            }
            return pt;
        }

        public static bool find(string meterName)
        {
            List<Poetry> pt = new List<Poetry>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select * from Poetry where meterID like '%@meterName%' order by id DESC";
            cmd.Parameters.AddWithValue("@ meterName", meterName); 
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
        public static List<MetersList> findMeters()
        {
            List<string> finalList = new List<string>();
            List<string> cleanList = new List<string>();
            List<string> metersList = new List<string>();
            MySqlConnection myConn;
            MySqlDataReader dataReader;
            myConn = new MySqlConnection(TaqtiController.connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(TaqtiController.connectionString);
            cmd = myConn.CreateCommand();
            cmd.CommandText = "select meterID from Poetry;";
            dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                metersList.Add(dataReader.GetString(0));
            }
            myConn.Close();

            foreach (var meter in metersList)
            {
                char[] delimiters = new[] { ',', '،' };  // List of delimiters
                var subStrings = meter.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in subStrings)
                {
                    cleanList.Add(t);
                    if (finalList.Count == 0)
                    {
                        finalList.Add(t);
                    }
                    bool flag = true;
                    foreach (var m in finalList)
                    {
                        if (m.Equals(t.Trim()))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        finalList.Add(t.Trim());
                    }
                }
            }

            string[] meterNames = new string[finalList.Count()];
            for (int i = 0; i < finalList.Count; i++)
            {
                meterNames[i] = finalList[i];
            }

            Array.Sort(meterNames);
            finalList.Clear();
            foreach (var m in meterNames)
            {
                finalList.Add(m);
            }

            List<MetersList> list = new List<MetersList>();
            foreach(var m in finalList)
            {
                int count = 0; 
                foreach(var t in cleanList)
                {
                    if(t.Equals(m))
                    {
                        count++;
                    }
                }
                MetersList ml = new MetersList();
                ml.name = m;
                ml.count = count;
                list.Add(ml);
            }
            return list;
        }
    }
    public class MetersList
    {
        public string name { get; set; }
        public int count { get; set; }
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
        public int id { get; set; }
        public bool add { get; set; }
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
        public int maxpages { get; set; }
        public int currentPage { get; set; }

    }
    public class Input
    {
        [Required]
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
        public bool modified = false;
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
            modified = wrd.modified;
        }
        public static string wordLookup(int id)
        {
            string wrd = "";
            string connectionString = TaqtiController.connectionString;
            MySqlConnection myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            cmd = myConn.CreateCommand();

            cmd.CommandText = "select * from mastertable where id = @id;";
            cmd.Parameters.AddWithValue("@id",id);
            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows) // look in master table
            {
                while (dataReader.Read())
                {
                    wrd = dataReader.GetString(dataReader.GetOrdinal("Word"));
                }
            }
            myConn.Close();
            return wrd;
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
    public class Lines
    {
        public List<Words> wordsList;
        public string originalLine = "";
        public Lines(string line)
        {
            line = line.Replace(",", "").Replace("\"", "").Replace("*", "").Replace("'", "").Replace("-", "").Replace("۔", "").Replace("،", "").Replace("?", "").Replace("!", "").Replace("ؔ", "").Replace("؟", "").Replace("‘", "").Replace("(", "").Replace(")", "").Replace("؛", "").Replace(";", "").Replace("\u200B", "").Replace("\u200C", "").Replace("\u200D", "").Replace("\uFEFF", "").Replace(".", "").Replace("ؒ", "").Replace("؎", "").Replace("-", "").Replace("=", "").Replace("ؑ", "").Replace("ؓ", "").Replace("\uFDFD", "").Replace("\uFDFA", "").Replace(":", "").Replace("’", "");
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