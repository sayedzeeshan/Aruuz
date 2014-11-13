using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
}