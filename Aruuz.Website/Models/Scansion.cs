using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using Aruuz.Controllers;
using System.Text.RegularExpressions;

namespace Aruuz.Models
{
    public class Scansion
    {
        public List<Lines> lstLines;
        public int numLines = 0;
        public bool isChecked = false;
        public bool freeVerse = false;
        public bool fuzzy = false;
        public int errorParam = 8;
        public List<int> meter;
        public Scansion()
        {
            lstLines = new List<Lines>();
        }
        private int LevenshteinDistance(string pattern, string code)
        {
            int m = pattern.Length;
            int n = code.Length;
            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (((pattern[i - 1].Equals(code[j - 1])) || (code[j - 1].Equals('x'))) && !pattern[i - 1].Equals('~'))
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        if (pattern[i - 1].Equals('~'))
                        {
                            if (code[j - 1].Equals('-'))
                            {
                                d[i, j] = d[i - 1, j - 1];
                            }
                            else
                            {
                                d[i, j] = min(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + 1); //deletion, insertion, substitution

                            }
                        }
                        else
                        {
                            d[i, j] = min(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + 1); //deletion, insertion, substitution

                        }
                    }
                }
            }
            return d[m, n];
        }
        private string[,] matchFuzzy(string meter, string code)
        {
            int m = meter.Length;
            int n = code.Length;

            int[,] d = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
            {
                d[i, 0] = i;
            }
            for (int j = 0; j <= n; j++)
            {
                d[0, j] = j;
            }
            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (((meter[i - 1].Equals(code[j - 1])) || (code[j - 1].Equals('x'))) && !meter[i - 1].Equals('~'))
                    {
                        d[i, j] = d[i - 1, j - 1];
                    }
                    else
                    {
                        if (meter[i - 1].Equals('~'))
                        {
                            if (code[j - 1].Equals('-'))
                            {
                                d[i, j] = d[i - 1, j - 1];
                            }
                            else
                            {
                                d[i, j] = min(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + 1); //deletion, insertion, substitution

                            }
                        }
                        else
                        {
                            d[i, j] = min(d[i - 1, j] + 1, d[i, j - 1] + 1, d[i - 1, j - 1] + 1); //deletion, insertion, substitution

                        }

                    }
                }
            }

            int length = Math.Max(m, n);
            string[,] table = new string[length + d[m, n] + 1, 3];
            table[0, 2] = d[m, n].ToString();

            for (int i = 0; i < length + d[m, n] + 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    table[i, j] = "";
                }
            }

            int k = m;
            int l = n;
            int s = 0;
            while (k >= 0 && l >= 0)
            {
                if (k == 0 || l == 0)   //corner case
                {
                    if (k == 0 && l > 0)
                    {
                        table[s, 0] = " ";
                        table[s, 1] = code[l - 1].ToString();
                        s++;
                        l = l - 1;
                    }
                    else if (l == 0 && k > 0)
                    {
                        table[s, 0] = meter[k - 1].ToString();
                        table[s, 1] = " ";
                        s++;
                        k = k - 1;
                    }
                    else // reached the top
                    {
                        break;
                    }
                }
                else
                {
                    if (d[k - 1, l - 1] <= d[k - 1, l] && d[k - 1, l - 1] <= d[k, l - 1])
                    {
                        if (d[k - 1, l - 1] == d[k, l] || d[k - 1, l - 1] == d[k, l] - 1)
                        {

                            if (d[k - 1, l - 1] == d[k, l])  //no operation
                            {
                                table[s, 0] = meter[k - 1].ToString();
                                table[s, 1] = meter[k - 1].ToString(); // code[l-1].ToString()
                                s++;
                            }
                            else  //substitution
                            {
                                table[s, 0] = meter[k - 1].ToString();
                                table[s, 1] = "[" + code[l - 1].ToString() + "]";
                                s++;
                            }
                            k = k - 1;
                            l = l - 1;
                        }
                    }
                    else if (d[k - 1, l] <= d[k, l - 1]) // insertion
                    {

                        table[s, 0] = meter[k - 1].ToString();
                        table[s, 1] = " ";
                        s++;
                        k = k - 1;
                        // l remains the same

                    }
                    else //deletion
                    {
                        table[s, 0] = " ";
                        table[s, 1] = code[l - 1].ToString();

                        s++;
                        l = l - 1;
                        // k remains the same
                    }
                }
            }

            return table;

        }
        private int min(int x, int y, int z)
        {
            int a = x;
            if (x > y)
            {
                if (y > z)
                {
                    a = z;
                }
                else
                {
                    a = y;
                }

            }
            else if (x > z)
            {
                if (y > z)
                {
                    a = z;
                }
                else
                {
                    a = y;
                }
            }
            else
            {
                a = x;
            }

            return a;
        }
        public void addLine(Lines line)
        {
            if (line.wordsList.Count > 0)
            {
                lstLines.Add(line);
                numLines += 1;
            }
        }
        public bool isVowelPlusH(char letter)
        {
            if (letter == 'ا' || letter == 'ی' || letter == 'ے' || letter == 'و' || letter == 'ہ' || letter == 'ؤ')
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool isIzafat(string word)
        {
            /*  ۂ= \u06C2
             *  
             * 
             * 
             * */

            if (word[word.Length - 1].ToString().Equals(Araab.characters[1]) || word[word.Length - 1].ToString().Equals(Araab.characters[10]) || (word[word.Length - 1] == '\u06C2'))
                return true;
            else
                return false;
        }
        public bool isConsonantPlusConsonant(string word)
        {
            if (!(word[1] == 'ا' || word[1] == 'ی' || word[1] == 'ے' || word[1] == 'ہ'))
            {
                if (!(word[0] == 'ا' || word[0] == 'ی' || word[0] == 'ے' || word[0] == 'ہ'))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        static public bool isMuarrab(string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                for (int j = 0; j < Araab.length; j++)
                    if (word[i].ToString().Equals(Araab.characters[j]))
                        return true;
            }
            return false;
        }
        static public bool isMuarrab(char letter)
        {
            for (int j = 0; j < Araab.length; j++)
                if (letter.ToString().Equals(Araab.characters[j]))
                    return true;
            return false;
        }
        public string locateAraab(string word)
        {
            string loc = "";
            for (int i = 0; i < word.Length; i++)
            {
                if (i < word.Length - 1)
                {
                    if (isMuarrab(word[i + 1]))
                    {
                        loc = loc + word[i + 1];
                        i = i + 1;
                    }
                    else
                    {
                        loc = loc + " ";
                    }
                }
                else
                {
                    loc = loc + " ";
                }
            }
            return loc;
        }
        public List<scanPath> findMeter(Lines line)
        {
            codeLocation loc = new codeLocation();
            loc.code = "root";
            CodeTree tree = new CodeTree(loc);
            tree.errorParam = errorParam;
			tree.fuzzy = fuzzy;
			tree.freeVerse = freeVerse;
            for (int w = 0; w < line.wordsList.Count; w++)
            {
                Words wrd = line.wordsList[w];
                List<codeLocation> codeList = new List<codeLocation>();
                for (int i = 0; i < wrd.code.Count; i++)
                {
                    if (codeList.Count > 0)
                    {
                        for (int j = 0; j < codeList.Count; j++)
                        {
                            if (codeList[j].code.Equals(wrd.code[i]))
                            {
                                break;
                            }
                            else
                            {
                                codeLocation cd = new codeLocation();
                                cd.code = wrd.code[i];
                                cd.codeRef = i;
                                cd.wordRef = w;
                                cd.word = wrd.word;
                                codeList.Add(cd);
                                tree.AddChild(cd);
                            }
                        }
                    }
                    else
                    {
                        codeLocation cd = new codeLocation();
                        cd.code = wrd.code[i];
                        cd.codeRef = i;
                        cd.wordRef = w;
                        cd.word = wrd.word;
                        codeList.Add(cd);
                        tree.AddChild(cd);
                    }

                }
                for (int k = 0; k < wrd.taqtiWordGraft.Count; k++)
                {
                    if (codeList.Count > 0)
                    {
                        for (int j = 0; j < codeList.Count; j++)
                        {
                            if (codeList[j].code.Equals(wrd.taqtiWordGraft[k]))
                            {
                                break;
                            }
                            else
                            {
                                codeLocation cd = new codeLocation();
                                cd.code = wrd.taqtiWordGraft[k];
                                cd.codeRef = wrd.code.Count + k;
                                cd.wordRef = w;
                                cd.word = wrd.word;
                                codeList.Add(cd);
                                tree.AddChild(cd);
                            }
                        }
                    }
                    else
                    {
                        codeLocation cd = new codeLocation();
                        cd.code = wrd.taqtiWordGraft[k];
                        cd.codeRef = wrd.code.Count + k;
                        cd.wordRef = w;
                        cd.word = wrd.word;
                        codeList.Add(cd);
                        tree.AddChild(cd);
                    }
                }

            }
            return tree.findMeter(meter);
        }
        public string assignCode(Words word)
        {

            /////////////////Rest of the cases///////////////////////////
            Words wrd = new Words();
            wrd = word;
            string word1 = Araab.removeAraab(word.word);
            word1 = word1.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            wrd.taqti[word.taqti.Count - 1] = word.taqti[word.taqti.Count - 1].Replace("\u06BE", "").Replace("\u06BA", "");
            string code = "";
            if (word1.Length == 2)
            {

                return lengthTwoScan(word1);
            }
            else if (word1.Length == 1)
            {
                return lengthOneScan(word1);
            }
            else
            {

                string residue = wrd.taqti[word.taqti.Count - 1].Trim();
               
                char[] delimiters = new [] { '+', ' ' };  // List of delimiters
                var subStrings = residue.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);


                for (int i = 0; i < subStrings.Length; i++)
                {
                    string subString = subStrings[i];
                    if (Araab.removeAraab(subString.Replace("\u06BE", "").Replace("\u06BA", "")).Length == 1)
                    {
                        code = code + lengthOneScan(subString);
                    }
                    else if (Araab.removeAraab(subString.Replace("\u06BE", "").Replace("\u06BA", "")).Length == 2)
                    {
                        string stripped = Araab.removeAraab(subString);
                        if (stripped[0] == '\u0622') // آ
                            code = code + "=-";
                        else
                            code = code + "=";

                    }
                    else if (Araab.removeAraab(subString.Replace("\u06BE", "").Replace("\u06BA", "")).Length == 3)
                    {
                        code = code + lengthThreeScan(subString);

                    }
                    else if (Araab.removeAraab(subString.Replace("\u06BE", "").Replace("\u06BA", "")).Length == 4)
                    {
                        code = code + lengthFourScan(subString);

                    }
                    else if (Araab.removeAraab(subString.Replace("\u06BE", "").Replace("\u06BA", "")).Length == 5)
                    {
                        code = code + lengthFiveScan(subString);
                    }
                }

                if (!string.IsNullOrWhiteSpace(code))
                {
                    if (code[code.Length - 1].ToString().Equals("=") || code[code.Length - 1].ToString().Equals("x"))   // Word-end flexible syllable
                    {
                        if (isVowelPlusH(Araab.removeAraab(word1)[word1.Length - 1]))
                        {
                            if (Araab.removeAraab(word1)[word1.Length - 1] == 'ا' || Araab.removeAraab(word1)[word1.Length - 1] == 'ی')
                            {
                                if (word.language.Count > 0)
                                {
                                    bool fl = false;
                                    for(int x = 0; x< word.language.Count; x++)
                                    {
                                        if(word.language[x].Equals("عربی")  && !word.modified)
                                        {
                                            fl = true;
                                        }
                                        else if (word.language[x].Equals("فارسی") && Araab.removeAraab(word1)[word1.Length - 1] == 'ا' && !word.modified)
                                        {
                                            fl = true;
                                        }
                                    }

                                    if(fl)
                                    {
                                        code = code.Remove(code.Length - 1, 1);
                                        code += "=";
                                    }
                                    else
                                    {
                                        code = code.Remove(code.Length - 1, 1);
                                        code += "x";
                                    }
                                }
                                else
                                {
                                    code = code.Remove(code.Length - 1, 1);
                                    code += "x";
                                }
                            }
                            else
                            {
                                code = code.Remove(code.Length - 1, 1);
                                code += "x";
                            }
                        }
                    }
                }
                
                return code;
            }
        }
        public string NoonGhunna(string word, string code)
        {
            string subString = word.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            string stripped = Araab.removeAraab(subString);
            string loc = locateAraab(subString);
            if(stripped.Length == 3)
            {
                   if (stripped[0] == 'آ')
                    {
                        if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2])) //jazr
                        {
                            if (code.Equals("=--"))  //آنت
                            {
                                code = "=-";
                            }
                        }
                    }
                   else if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2]))
                    {
                        if (code.Equals("=-"))
                        {
                            if (stripped[0] == 'ا') //انگ
                            {
                                code = "=-";
                            }
                            else if (isVowelPlusH(stripped[0]))  //ہنس
                            {
                                code = "=";
                            }
                        }
                    }
            }
            else if(stripped.Length == 4)
            {
                    if (stripped[0] == 'آ')
                    {
                        if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2]))
                        {
                            if (code.Equals("=-="))
                            {
                                code = "==";
                            }
                        }
                    }
                    else if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2]))
                    {
                        if (code.Equals("=="))
                        {
                            if (stripped[0] == 'ا')   //اندر
                            {
                                code = "==";
                            }
                            else if (isVowelPlusH(stripped[0])) //ہنسا
                            {
                                code = "-=";
                            }
                        }
                        else if (code.Equals("=--"))
                        {
                            //find an example
                        }
                    }
                    else if (stripped[2] == 'ن' && loc[2].ToString().Equals(Araab.characters[2]))
                    {
                        if (code.Equals("=--"))
                        {
                            if (isVowelPlusH(stripped[1])) //باندھ،اونٹ، ہونٹ
                            {
                                code = "=-";
                            }
                        }
                        else if(code.Equals("=="))
                        {
                            if (isVowelPlusH(stripped[1])) //بانگ
                            {
                                if(!isVowelPlusH(stripped[3]))
                                {
                                    code = "=-";
                                }
                                
                            }
                        }
                    }
            }
            else if(stripped.Length == 5)
            {
                
                    if (stripped[0] == 'آ')
                    {
                        if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2]))
                        {
                            if (code[1].Equals('-'))
                            {
                                code = code.Remove(1, 1);
                            }
                        }
                    }
                    else if (stripped[1] == 'ن' && loc[1].ToString().Equals(Araab.characters[2]))
                    {
                        if (code[0].Equals('='))
                        {
                            if (stripped[0] == 'ا')   //انگیزی
                            {

                            }
                            else if (isVowelPlusH(stripped[0]))
                            {

                            }
                        }
                        else if (code.Equals("=--"))
                        {
                            //find an example
                        }
                    }
                    else if (stripped[2] == 'ن' && loc[2].ToString().Equals(Araab.characters[2]))
                    {
                        if (code[0].Equals('=') && code[1].Equals('-'))
                        {
                            if (isVowelPlusH(stripped[1]))
                            {
                                code = code.Remove(1, 1);
                            }
                        }
                    }
                    else if (stripped[3] == 'ن' && loc[3].ToString().Equals(Araab.characters[2]))
                    {

                        if (code[code.Length - 1].Equals('-') && code[code.Length - 2].Equals('-'))
                        {
                            if (isVowelPlusH(stripped[2]))
                            {
                                if (code.Length > 2)
                                {
                                    if (code[code.Length - 3].Equals('='))
                                    {
                                        code = code.Remove(code.Length - 1, 1);
                                    }
                                }
                            }
                        }
                    }
            }

            return code;

        }
        public string lengthOneScan(string substr)
        {
            string code = "";
            if (Araab.removeAraab(substr).Equals("\u0622")) /// \u0622 آ long syllable
            {
                code = "=";
            }
            else
            {
                code = "-";
            }
            return code;

        }
        public string lengthTwoScan(string substr)
        {
            ////////////////////////////// Two-lettered words /////////////////////////////////////////////
            //// Pritchett says: "...for by treating ALL one-syllable words of the forms given above
            //// as potentially flexible one never makes errors in ascertaining the meter of a poem." $2.1
            //// We are going to treat the two-lettered words ending in ا،ی،ے،و،ہ as flexible 
            string subString = substr.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            string stripped = Araab.removeAraab(subString);
            string code = "=";
            if (substr[0] == '\u0622') // آ
                code = "=-";
            else if (isVowelPlusH(stripped[stripped.Length - 1]))
                code = "x";

            return code;
        }
        public string lengthThreeScan(string substr)
        {
            string code = "";
            string subString = substr.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            string stripped = Araab.removeAraab(subString);

            if (stripped.Length == 1)
            {
                if (stripped[0] == 'آ')
                {
                    return "-";
                }
                else
                {
                    return "=";
                }
            }
            else if (stripped.Length == 2)
            {
                return lengthTwoScan(substr);
            }
            if (isMuarrab(subString))
            {
                string loc = locateAraab(subString);

                if (loc[1].ToString().Equals(Araab.characters[2])) //jazm  
                {
                    if (stripped[0] == 'آ')
                    {
                        code = "=--";
                    }
                    else
                    {
                        code = "=-";
                    }
                }
                else if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                {
                    code = "-=";
                }
                else if (loc[1].ToString().Equals(Araab.characters[0])) //shadd
                {
                    code = "==";
                }
                else if (stripped[2].ToString().Equals("ا"))
                {
                    code = "-=";
                }
                else if (stripped[2].ToString().Equals("ا") || stripped[2].ToString().Equals("ی") || stripped[2].ToString().Equals("ے") || stripped[2].ToString().Equals("و") || stripped[2].ToString().Equals("ہ")) //vowels at end
                {
                    if (stripped[1].ToString().Equals("ا"))
                        code = "=-";
                    else
                        code = "-=";
                }
                else if (stripped[1].ToString().Equals("ا") || stripped[1].ToString().Equals("ی") || stripped[1].ToString().Equals("ے") || stripped[1].ToString().Equals("و") || stripped[2].ToString().Equals("ہ")) //vowels at center
                {
                    code = "=-";
                }
                else //default case
                {
                    code = "=-";
                }
            }
            else
            {
                if (stripped[0] == 'آ')
                {
                    code = "==";
                }
                else if (stripped[1].ToString().Equals("ا")) //Alif at centre
                {
                    code = "=-";
                }
                else if ((stripped[2].ToString().Equals("ا")))
                {
                    code = "-=";
                }
                else if (stripped[1].ToString().Equals("ی") || stripped[1].ToString().Equals("ے") || stripped[1].ToString().Equals("و") || stripped[1].ToString().Equals("ہ")) //vowels + h at centre
                {
                    if (stripped[2].ToString().Equals("ہ"))
                    {
                        code = "=-";
                    }
                    else if (stripped[2].ToString().Equals("ی") || stripped[2].ToString().Equals("ے") || stripped[2].ToString().Equals("و")) //vowels + h at end
                    {
                        code = "-=";
                    }
                    else
                    {
                        code = "=-";
                    }
                }
                else if (stripped[2].ToString().Equals("ی") || stripped[2].ToString().Equals("ے") || stripped[2].ToString().Equals("و") || stripped[2].ToString().Equals("ہ")) //vowels + h at end
                {
                    code = "-=";
                }
                else if (isVowelPlusH(stripped[0]))
                {
                    code = "-=";
                }
                else
                {
                    code = "-=";
                }
            }

            if (containsNoon(stripped))
            {
                code = NoonGhunna(substr, code);
            }
            return code;
        }
        public string lengthFourScan(string substr)
        {
            string code = "";
            string subString = substr.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            string stripped = Araab.removeAraab(subString);

            if (stripped.Length == 1)
            {
                code = lengthOneScan(subString);
            }
            else if (stripped.Length == 2)
            {
                code = lengthTwoScan(subString);
            }
            else if (stripped.Length == 3)
            {
                code = lengthThreeScan(subString);
            }
            else
            {
                if (stripped[0] == 'آ')
                {
                    code = "=" + lengthThreeScan(subString.Remove(0, 1));
                }
                else if (isMuarrab(subString))
                {
                    string loc = locateAraab(subString);
                    if (stripped[1] == 'ا')
                    {
                        if (loc[2].ToString().Equals(Araab.characters[2])) // jazr
                        {
                            code = "=--";
                        }
                        else
                        {
                            code = "==";
                        }
                    }
                    else if (stripped[2] == 'ا')
                    {
                        code = "-=-";
                    }
                    else
                    {
                        if (stripped[1] == 'و')
                        {
                            if (stripped[3] == 'ت' && loc[3].ToString().Equals(Araab.characters[2])) //jazm
                            {
                                code = "=-";
                            }
                            else
                            {
                                if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh
                                {
                                    code = "-=-";
                                }
                                else
                                {
                                    if (loc[2].ToString().Equals(Araab.characters[2])) // jazr
                                    {
                                        code = "=--";
                                    }
                                    else
                                    {
                                        code = "==";
                                    }
                                }
                            }
                        }
                        else if (stripped[1] == 'ی')
                        {
                            if (stripped[3] == 'ت' && loc[3].ToString().Equals(Araab.characters[2])) //jazm
                            {
                                code = "=-";
                            }
                            else if (loc[0].ToString().Equals(Araab.characters[1]) || loc[0].ToString().Equals(Araab.characters[8]) || loc[0].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    code = "-=-";
                                }
                                else
                                {
                                    if (loc[2].ToString().Equals(Araab.characters[2])) //jazr
                                    {
                                        code = "=--";
                                    }
                                    else
                                    {
                                        code = "==";
                                    }
                                }
                            }
                            else
                            {
                                code = "==";
                            }
                        }
                        else
                        {
                            if (loc[0].ToString().Equals(Araab.characters[1]) || loc[0].ToString().Equals(Araab.characters[8]) || loc[0].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if (isVowelPlusH(stripped[2]))
                                    {
                                        code = "-=-";
                                    }
                                    else if (loc[2].ToString().Equals(Araab.characters[2]) ) // jazr 
                                    {
                                        code = "-=-";
                                    }
                                    else
                                    {
                                        code = "--=";
                                    }

                                }
                                else if (loc[1].ToString().Equals(Araab.characters[2]))
                                {
                                    code = "==";

                                }
                                else if (loc[2].ToString().Equals(Araab.characters[2]))
                                {
                                    code = "-=-";
                                }
                                else
                                {
                                    if (stripped[3] == 'ا' || stripped[3] == 'ی')
                                    {
                                        code = "--=";
                                    }
                                    else
                                    {
                                        code = "-=-";
                                    }
                                }
                            }
                            else if (loc[1].ToString().Equals(Araab.characters[2]))
                            {
                                if (loc[2].ToString().Equals(Araab.characters[2]))
                                {
                                    code = "==";
                                }
                                else
                                {
                                    code = "=--";
                                }
                            }
                            else if (loc[2].ToString().Equals(Araab.characters[2]))
                            {
                                code = "-=-";
                            }
                            else if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                code = "==";
                            }
                            else if (isVowelPlusH(stripped[2]))
                            {
                                code = "-=-";
                            }
                            else
                            {
                                code = "==";
                            }
                        }
                    }
                }
                else if (isVowelPlusH(Araab.removeAraab(subString)[2]))
                {
                    if (Araab.removeAraab(subString)[3] == 'ا')
                    {
                        code = "==";
                    }
                    else if (isVowelPlusH(Araab.removeAraab(subString)[1]))
                        code = "==";
                    else
                        code = "-=-";
                }
                else //default
                {
                    code = "==";
                }
            }

            if (containsNoon(stripped))
            {
                code = NoonGhunna(substr, code);
            }
            return code;
        }
        public string lengthFiveScan(string substr)
        {
            string code = "";
            string subString = substr.Replace("\u06BE", "").Replace("\u06BA", ""); /// remove ھ \u06BE and ں \u06BA for scansion purposes
            string stripped = Araab.removeAraab(subString);
            if (stripped.Length == 3)
            {
                code = lengthThreeScan(substr);
            }
            else if (stripped.Length == 4)
            {
                code = lengthFourScan(substr);
            }
            else
            {
                if (stripped[0] == 'آ')
                {
                    code = "=" + lengthFourScan(substr.Remove(0, 2));
                }
                else if (isMuarrab(substr))
                {
                    string loc = locateAraab(substr);
                    if (stripped[1] == 'ا' || stripped[2] == 'ا' || stripped[3] == 'ا') // check alif at position 2,3,4
                    {
                        #region Position 3 Alif
                        if (stripped[2] == 'ا')
                        {
                            code = "-==";
                        }
                        #endregion
                        #region Position 2 Alif
                        else if (stripped[1] == 'ا')
                        {
                            if (isMuarrab(loc[0]))
                            {
                                if (isMuarrab(loc[1]))
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 3));
                                }
                                else
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 4));
                                }
                            }
                            else
                            {
                                if (isMuarrab(loc[1]))
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 2));
                                }
                                else
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 3));
                                }
                            }
                        }
                        #endregion
                        #region  Position 4 Alif
                        else
                        {
                            code = "==-";
                            if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                code = "--=-";
                            }
                            else if (loc[1].ToString().Equals(Araab.characters[2]))
                            {
                                code = "--=-";
                            }
                            else if (stripped[0] == 'ب')
                            {
                                if (isVowelPlusH(stripped[1]))
                                {
                                    code = "==-";
                                }
                                else if (stripped[1] == 'ر')
                                {
                                    code = "==-";
                                }
                                else if (stripped[1] == 'ن')
                                {
                                    code = "==-";
                                }
                                else if (stripped[1] == 'غ')
                                {
                                    code = "==-";
                                }
                                else
                                {
                                    code = "--=-";
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (stripped[1] == 'و' || stripped[2] == 'و' || stripped[3] == 'و' || stripped[1] == 'ی' || stripped[2] == 'ی' || stripped[3] == 'ی')
                        {
                            if (stripped[1] == 'و' || stripped[1] == 'ی')
                            {
                                if ((loc[1].ToString().Equals(Araab.characters[2])))
                                {
                                    if (isMuarrab(loc[0]))
                                    {
                                        if (isMuarrab(loc[1]))
                                        {
                                            code = "=" + lengthThreeScan(substr.Remove(0, 5));
                                        }
                                        else
                                        {
                                            code = "=" + lengthThreeScan(substr.Remove(0, 4));
                                        }

                                    }
                                    else
                                    {
                                        if (isMuarrab(loc[1]))
                                        {
                                            code = "=" + lengthThreeScan(substr.Remove(0, 3));
                                        }
                                        else
                                        {
                                            code = "=" + lengthThreeScan(substr.Remove(0, 4));
                                        }
                                    }
                                }
                                else if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                    {
                                        code = "--=-";
                                    }
                                    else
                                    {
                                        code = "-==";
                                    }
                                }
                                else
                                {
                                    if ((loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9]))) // zer, zabr, pesh )
                                    {
                                        if (loc[3].ToString().Equals(Araab.characters[1]) || loc[3].ToString().Equals(Araab.characters[8]) || loc[3].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                        {
                                            code = "=-=";
                                        }
                                        else if (loc[3].ToString().Equals(Araab.characters[2]))
                                        {
                                            code = "==-";
                                        }
                                        else
                                        {
                                            code = "==-";
                                        }
                                    }
                                    else if ((loc[2].ToString().Equals(Araab.characters[2])))
                                    {
                                        if (loc[3].ToString().Equals(Araab.characters[1]) || loc[3].ToString().Equals(Araab.characters[8]) || loc[3].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                        {
                                            code = "=-=";
                                        }
                                        else if (loc[3].ToString().Equals(Araab.characters[2]))
                                        {
                                            code = "=---";
                                        }
                                        else
                                        {
                                            if (isMuarrab(loc[2]))
                                            {
                                                code = "=" + lengthThreeScan(substr.Remove(0, 4));
                                            }
                                            else
                                            {
                                                code = "=" + lengthThreeScan(substr.Remove(0, 3));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        code = "=" + lengthThreeScan(substr.Remove(0, 2));
                                    }

                                }
                            }
                            else if ((stripped[2] == 'و' || stripped[2] == 'ی'))
                            {
                                if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if ((loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9]))) // zer, zabr, pesh 
                                    {
                                        if (loc[3].ToString().Equals(Araab.characters[1]) || loc[3].ToString().Equals(Araab.characters[8]) || loc[3].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                        {
                                            code = "-----";   //highly unlikely
                                        }
                                        else
                                        {
                                            code = "--=-";
                                        }
                                    }
                                }
                                else if ((loc[2].ToString().Equals(Araab.characters[2])))
                                {

                                    code = "-==";

                                }
                                else
                                {
                                    code = "-==";
                                }
                            }
                            else if ((stripped[3] == 'و' || stripped[3] == 'ی'))
                            {
                                if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if ((loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9]))) // zer, zabr, pesh 
                                    {
                                        if (loc[3].ToString().Equals(Araab.characters[1]) || loc[3].ToString().Equals(Araab.characters[8]) || loc[3].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                        {
                                            code = "---=";   //highly unlikely
                                        }
                                        else
                                        {
                                            code = "--=-";
                                        }
                                    }
                                }
                                else if ((loc[2].ToString().Equals(Araab.characters[2])))
                                {

                                    code = "-==";

                                }
                                else
                                {
                                    code = "==-";
                                }
                            }
                            else
                            {
                                if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if ((loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9]))) // zer, zabr, pesh 
                                    {
                                        if (loc[3].ToString().Equals(Araab.characters[1]) || loc[3].ToString().Equals(Araab.characters[8]) || loc[3].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                        {
                                            code = "-----";   //highly unlikely
                                        }
                                        else
                                        {
                                            code = "--=-";
                                        }
                                    }
                                }
                                else if ((loc[2].ToString().Equals(Araab.characters[2])))
                                {

                                    code = "-==";

                                }
                                else
                                {
                                    code = "==-";
                                }
                            }
                        }
                        else
                        {
                            if (loc[1].ToString().Equals(Araab.characters[1]) || loc[1].ToString().Equals(Araab.characters[8]) || loc[1].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                                {
                                    if (stripped[4] == 'ا')
                                    {
                                        code = "---=";
                                    }
                                    else
                                    {
                                        code = "--=-";
                                    }
                                }
                                else if (loc[2].ToString().Equals(Araab.characters[2]))
                                {
                                    code = "-==";
                                }
                                else
                                {
                                    code = "-==";
                                }
                            }
                            else if (loc[1].ToString().Equals(Araab.characters[2]))
                            {
                                if (isMuarrab(loc[0]))
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 4));
                                }
                                else
                                {
                                    code = "=" + lengthThreeScan(substr.Remove(0, 3));
                                }
                            }
                            else if (loc[2].ToString().Equals(Araab.characters[1]) || loc[2].ToString().Equals(Araab.characters[8]) || loc[2].ToString().Equals(Araab.characters[9])) // zer, zabr, pesh 
                            {
                                code = "=-=";
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else if (stripped[1] == 'ا' || stripped[2] == 'ا' || stripped[3] == 'ا') // check alif at position 2,3,4
                {
                    #region Position 3 Alif
                    if (stripped[2] == 'ا')
                    {
                        code = "-==";
                    }
                    #endregion
                    #region Position 2 Alif
                    else if (stripped[1] == 'ا')
                    {
                        if (stripped[3] == 'ا')
                        {
                            code = "==-";
                        }
                        else
                        {
                            if (isVowelPlusH(stripped[3]))
                            {
                                if (isVowelPlusH(stripped[4]))
                                {
                                    code = "=-=";
                                }
                                else
                                {
                                    code = "==-";
                                }
                            }
                            else if (isVowelPlusH(stripped[4]))
                            {
                                code = "=-=";
                            }
                            else
                            {
                                code = "==-";
                            }
                        }
                    }
                    #endregion
                    #region  Position 4 Alif
                    else
                    {
                        code = "==-";
                        if (stripped[0] == 'ب')
                        {
                            if (isVowelPlusH(stripped[1]))
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'ر')
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'ن')
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'غ')
                            {
                                code = "==-";
                            }
                            else
                            {
                                code = "--=-";
                            }
                        }
                    }
                    #endregion
                }
                else if (isVowelPlusH(stripped[1]) || isVowelPlusH(stripped[2]) || isVowelPlusH(stripped[3])) // check vowels at position 2,3,4
                {
                    #region Position 3 Vowel
                    if (isVowelPlusH(stripped[2]))
                    {
                        code = "-==";
                        if (isVowelPlusH(stripped[3]))
                        {
                            code = "-==";
                        }
                    }
                    #endregion
                    #region Position 2 Vowel
                    else if (isVowelPlusH(stripped[1]))
                    {
                        if (isVowelPlusH(stripped[3]))
                        {
                            code = "==-";
                        }
                        else
                        {
                            if (isVowelPlusH(stripped[3]))
                            {
                                if (isVowelPlusH(stripped[4]))
                                {
                                    code = "=-=";
                                }
                                else
                                {
                                    code = "==-";
                                }
                            }
                            else if (isVowelPlusH(stripped[4]))
                            {
                                code = "=-=";
                            }
                            else
                            {
                                code = "==-";
                            }
                        }
                    }
                    #endregion
                    #region  Position 4 Vowel
                    else
                    {
                        code = "==-";

                        if (stripped[0] == 'ب')
                        {
                            if (isVowelPlusH(stripped[1]))
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'ر')
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'ن')
                            {
                                code = "==-";
                            }
                            else if (stripped[1] == 'غ')
                            {
                                code = "==-";
                            }
                            else
                            {
                                code = "--=-";
                            }
                        }
                        if (stripped[4] == 'ت' && stripped[3] == 'ی')
                        {
                            code = code.Remove(code.Length - 1, 1);
                            code += "=";
                        }
                    }
                    #endregion
                }
                else //consonants
                {
                    #region Consonants
                    code = "==-";
                    if (stripped[0] == 'ب')
                    {
                        if (isVowelPlusH(stripped[1]))
                        {
                            code = "==-";
                        }
                        else if (stripped[1] == 'ر')
                        {
                            code = "==-";
                        }
                        else if (stripped[1] == 'ن')
                        {
                            code = "==-";
                        }
                        else if (stripped[1] == 'غ')
                        {
                            code = "==-";
                        }
                        else
                        {
                            code = "--=-";
                        }
                    }
                    if (stripped[0] == 'ت' || stripped[0] == 'ش')
                    {
                        code = "-==";
                    }
                    if (stripped[4] == 'ت' && stripped[3] == 'ی')
                    {
                        code = code.Remove(code.Length - 1, 1);
                        code += "=";
                    }
                    if (stripped[4] == 'ا') //???
                    {
                        code = "-==";
                    }
                    else if (isVowelPlusH(stripped[4]))
                    {
                        code = "=-=";
                    }
                    #endregion
                }

            }
            if (containsNoon(stripped))
            {
                code = NoonGhunna(substr, code);
            }
            return code;
        }
        public Words pluralForm(string substr, int len)
        {
            Words wrd = new Words();
            wrd.word = substr;
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }
            string form1 = "", form2 = "";
            form1 = substr.Remove(substr.Length - len, len);
            form2 = form1 + "نا";


            wrd.word = form1;
            wrd = findWord(wrd);
            if (wrd.id.Count == 0)
            {
                wrd.word = form2;
                wrd = findWord(wrd);
            }

            return wrd;
        }
        public Words pluralFormNoonGhunna(string substr)
        {
            Words wrd = new Words();
            wrd.word = substr;
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }

            wrd.word = substr;
            wrd = findWord(wrd);
            return wrd;
        }
        public Words pluralFormAat(string substr)
        {
            Words wrd = new Words();
            wrd.word = substr;
            string form1 = "", form2 = "", form3 = "", form4 = "", form5 = "";
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }
            int length = substr.Length;
            form1 = substr.Remove(substr.Length - 2, 2);   // تصورات
            form2 = form1 + "ہ"; //نظریہ،کلیہ
            form3 = substr.Remove(substr.Length - 2, 1); // آیت،صفت
            form4 = substr.Remove(substr.Length - 3, 1); 
            wrd.word = form1;
            wrd = findWord(wrd);
            if (wrd.id.Count == 0)
            {
                wrd.word = form2;
                wrd = findWord(wrd);
                if (wrd.id.Count == 0)
                {
                    wrd.word = form3;
                    wrd = findWord(wrd);
                    if (wrd.id.Count == 0)
                    {
                        if (substr[length - 1] == 'ت' && substr[length - 2] == 'ا' && substr[length - 3] == 'ی')
                        {
                            wrd.word = form4;
                            wrd = findWord(wrd);
                            if (wrd.id.Count == 0)
                            {
                                wrd.word = form5;
                                wrd = findWord(wrd);
                            }
                        }
                    }
                }

            }

            return wrd;
        }
        public Words pluralFormAan(string substr)
        {
           
            Words wrd = new Words();
            wrd.word = substr;
            string form1 = "", form2 = "", form3 = "",form4 = "";
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }
            int length = substr.Length;
            form1 = substr.Remove(substr.Length - 2, 2);   // لڑکیاں
            form2 = form1 + "ہ"; //رستوں
            form3 = form1 + "ا"; // سودوں
            form4 = form1 + "نا"; // دکھاوں

            wrd.word = form1;
            wrd = findWord(wrd);
            if (wrd.id.Count == 0)
            {
                wrd.word = form2;
                wrd = findWord(wrd);
                if (wrd.id.Count == 0)
                {
                    wrd.word = form3;
                    wrd = findWord(wrd);
                    if (wrd.id.Count == 0)
                    {
                        wrd.word = form4;
                        wrd = findWord(wrd);
                    }


                }

            }
            return wrd;
        }
        public Words pluralFormYe(string substr)
        {
           
            Words wrd = new Words();
            wrd.word = substr;
            string form1 = "", form2 = "";
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }
            int length = substr.Length;
            form1 = substr.Remove(substr.Length - 2, 2) + "نا";   // ستائے
            form2 = substr.Remove(substr.Length - 2, 2);          // استغنائے

            wrd.word = form1;
            wrd = findWord(wrd);
            if (wrd.id.Count == 0)
            {
                wrd.word = form2;
                wrd = findWord(wrd);
            }

            return wrd;
        }
        public Words findWord(Words wrd)
        {
            string connectionString = TaqtiController.connectionString;
            MySqlConnection myConn = new MySqlConnection(connectionString);
            myConn.Open();
            MySqlCommand cmd = new MySqlCommand(connectionString);
            cmd = myConn.CreateCommand();

            string searchWord = Araab.removeAraab(wrd.word);  //Remove Araab if present

            cmd.CommandText = "select * from exceptions where word like @search;";
            cmd.Parameters.AddWithValue("@search", searchWord);

            MySqlDataReader dataReader = cmd.ExecuteReader();

            if (dataReader.HasRows) // look in exceptions table
            {
                while (dataReader.Read())
                {
                    Words wd = new Words();
                    string taqti2 = "", taqti3 = "";
                    wrd.id.Add(dataReader.GetInt32(0)*-1);
                    wrd.code.Add(dataReader.GetString(2).Replace(" ", ""));
                    try
                    {
                        taqti2 = dataReader.GetString(3).Replace(" ", "");
                    }
                    catch
                    {

                    }
                    try
                    {
                        taqti3 = dataReader.GetString(4).Replace(" ", "");
                    }
                    catch
                    {

                    }
                    if (!String.IsNullOrEmpty(taqti2)) wrd.code.Add(taqti2);
                    if (!String.IsNullOrEmpty(taqti3)) wrd.code.Add(taqti3);

                }
                myConn.Close();
            }
            else
            {
                myConn.Close();
                MySqlConnection myConn2 = new MySqlConnection(connectionString);
                myConn2.Open();
                MySqlCommand cmd2 = new MySqlCommand(connectionString);
                cmd2 = myConn2.CreateCommand();
                cmd2.CommandText = "select * from mastertable where word like @s or word like @s1 or word like @s2 or word like @s3  or word like @s4 or word like @s5 or word like @s6 or word like @s7 or word like @s8 or word like @s9 or word like @s10 or word like @s11 or word like @s12;";
                cmd2.Parameters.AddWithValue("@s", searchWord);
                cmd2.Parameters.AddWithValue("@s1", searchWord + " 1");
                cmd2.Parameters.AddWithValue("@s2", searchWord + " 2");
                cmd2.Parameters.AddWithValue("@s3", searchWord + " 3");
                cmd2.Parameters.AddWithValue("@s4", searchWord + " 4");
                cmd2.Parameters.AddWithValue("@s5", searchWord + " 5");
                cmd2.Parameters.AddWithValue("@s6", searchWord + " 6");
                cmd2.Parameters.AddWithValue("@s7", searchWord + " 7");
                cmd2.Parameters.AddWithValue("@s8", searchWord + " 8");
                cmd2.Parameters.AddWithValue("@s9", searchWord + " 9");
                cmd2.Parameters.AddWithValue("@s10", searchWord + " 10");
                cmd2.Parameters.AddWithValue("@s11", searchWord + " 11");
                cmd2.Parameters.AddWithValue("@s12", searchWord + " 12");
                
                MySqlDataReader dataReader2 = cmd2.ExecuteReader();
                if (dataReader2.HasRows) // found word in mastertable?
                {

                    while (dataReader2.Read())
                    {
                        Words wd = new Words();
                        wrd.id.Add(dataReader2.GetInt32(0));
                        wrd.muarrab.Add(dataReader2.GetString(2).Trim());
                        wrd.taqti.Add(dataReader2.GetString(3).Trim());
                        try
                        {
                            wrd.language.Add(dataReader2.GetString(4));
                        }
                        catch
                        {

                        }
                        wrd.isVaried.Add(dataReader2.GetBoolean(5));
                        wrd.code.Add(assignCode(wrd));
                    }
                    myConn2.Close();
                    if (wrd.isVaried.Count > 0)
                    {
                        if (wrd.isVaried[0])
                        {
                            MySqlConnection con = new MySqlConnection(connectionString);
                            con.Open();
                            MySqlCommand cmdtemp = new MySqlCommand(connectionString);
                            cmdtemp = con.CreateCommand();
                            cmdtemp.CommandText = "select * from variations where id = @id;";
                            cmdtemp.Parameters.AddWithValue("@id",wrd.id[0]);
                            MySqlDataReader dR2 = cmdtemp.ExecuteReader();
                            if (dR2.HasRows) // found word in mastertable?
                            {

                                while (dR2.Read())
                                {
                                    Words wd = new Words();
                                    wrd.id.Add(dR2.GetInt32(0));
                                    wrd.muarrab.Add(dR2.GetString(2).Trim());
                                    wrd.taqti.Add(dR2.GetString(3).Trim());
                                    wrd.code.Add(assignCode(wrd));
                                }
                                con.Close();

                            }
                            con.Close();

                        }
                    }
                }
                else //else search in plurals table
                {
                    myConn2.Close();

                    MySqlConnection myConn3 = new MySqlConnection(connectionString);
                    myConn3.Open();
                    MySqlCommand cmd3 = new MySqlCommand(connectionString);
                    cmd3 = myConn3.CreateCommand();
                    cmd3.CommandText = "select * from Plurals where word like @s;";
                    cmd3.Parameters.AddWithValue("@s",searchWord);
                    MySqlDataReader dataReader3 = cmd3.ExecuteReader();


                    if (dataReader3.HasRows)
                    {
                        while (dataReader3.Read())
                        {
                            Words wd = new Words();
                            wrd.id.Add(dataReader3.GetInt32(0));
                            wrd.muarrab.Add(dataReader3.GetString(2).Trim());
                            wrd.taqti.Add(dataReader3.GetString(3).Trim());
                            wrd.code.Add(assignCode(wrd));
                        }
                        myConn3.Close();
                    }
                    else // not found in plurals either? find in variations table 
                    {
                        myConn3.Close();


                        MySqlConnection myConn4 = new MySqlConnection(connectionString);
                        myConn4.Open();
                        MySqlCommand cmd4 = new MySqlCommand(connectionString);
                        cmd4 = myConn4.CreateCommand();
                        cmd4.CommandText = "select * from Variations where word like @s;";
                        cmd4.Parameters.AddWithValue("@s",searchWord);
                        MySqlDataReader dataReader4 = cmd4.ExecuteReader();

                        if (dataReader4.HasRows)
                        {
                            while (dataReader4.Read())
                            {
                                Words wd = new Words();
                                wrd.id.Add(dataReader4.GetInt32(0));
                                wrd.muarrab.Add(dataReader4.GetString(2).Trim());
                                wrd.taqti.Add(dataReader4.GetString(3).Trim());
                                wrd.code.Add(assignCode(wrd));
                            }
                        }
                        myConn4.Close();

                    }
                }
            }
            myConn.Close();
            return wrd;
        }
        public Words wordCode(Words wrd)
        {
            wrd = findWord(wrd);

            if (wrd.id.Count > 0)
            {
                string subString =   Araab.removeAraab(wrd.word.Replace("\u06BE", "").Replace("\u06BA", "")); /// remove ھ \u06BE and ں \u06BA for scansion purposes
                if (subString.Length == 3)
                {
                    if(subString[2] == 'ا')
                    {
                        if (subString[0] == 'آ')
                        {
                            if (!wrd.code[0].Equals("==") && !wrd.code[0].Equals("=x"))
                            {
                                wrd.id.Add(-1);
                                wrd.code.Add("==");
                            }
                            
                        }
                        else
                        {
                            if (!wrd.code[0].Equals("-=") && !wrd.code[0].Equals("-x"))
                            {
                                wrd.id.Add(-1);
                                wrd.code.Add("-=");
                            }
                            
                        }
                    }
                }
            }
            if (wrd.id.Count == 0) //word not found in dictionary
            {
                string code = "";
                int id = -1;
                bool flag = true;
                string stripped = Araab.removeAraab(wrd.word);
                int length = stripped.Length;
                wrd.modified = true;
                Words wd;


                ///////////////Noon Ghunna at the end (ں)  /////////////

                #region ں
                if (stripped.Length > 3)
                {
                    if (stripped[length - 1] == 'ں')
                    {
                        string originalWord = wrd.word;
                        wd = pluralForm(wrd.word.Remove(wrd.word.Length - 1, 1) + "ن", 0);
                        if (wd.id.Count > 0)
                        {
                            for (int k = 0; k < wd.code.Count; k++)
                            {
                                if (wd.code[k][wd.code[k].Length - 1] == '-')
                                {
                                    wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                }
                            }
                            flag = false;
                        }
                        if (!flag)
                        {
                            wrd.id = wd.id;
                            wrd.code = wd.code;
                            wrd.isVaried = wd.isVaried;
                            wrd.language = wd.language;
                            wrd.length = wd.length;
                            for (int k = 0; k < wd.muarrab.Count; k++)
                            {
                                wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length-1) + "ں");
                            }
                            wrd.taqti = wd.taqti;
                            wrd.taqtiWordGraft = wd.taqtiWordGraft;
                        }
                    }
                }
                #endregion


                #region ئے
                if (stripped.Length > 4)
                {
                    if (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ئ')
                    {
                        string originalWord = wrd.word;
                        wd = pluralForm(wrd.word, 2);
                        if (wd.id.Count > 0)
                        {
                            string form1 = "", form2 = "";
                            string substr = Araab.removeAraab(wd.word);
                            int len = substr.Length;

                            form1 = stripped.Remove(stripped.Length - 2, 2);
                            form2 = form1 + "نا";
                            if (substr.Equals(form1))
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {
                                    if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                    {
                                        wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                        wd.code[k] += "=";
                                    }
                                    wd.code[k] += "x";
                                }
                                for (int k = 0; k < wd.muarrab.Count; k++)
                                {
                                    wrd.muarrab.Add(wd.muarrab[k] + "ئے");
                                }
                                flag = false;
                            }
                            else //no change required
                            {
                                for (int k = 0; k < wd.muarrab.Count; k++)
                                {
                                    wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length-2) + "ئے");
                                }
                                flag = false;
                            }

                        }
                        if (!flag)
                        {
                            wrd.id = wd.id;
                            wrd.code = wd.code;
                            wrd.isVaried = wd.isVaried;
                            wrd.language = wd.language;
                            wrd.length = wd.length;
                            wrd.taqti = wd.taqti;
                            wrd.taqtiWordGraft = wd.taqtiWordGraft;
                        }
                    }
                }
                #endregion

                //////////////// Plural Case ///////////////////////////
                #region تا،تے،تی،تیں
                if (flag)
                {
                    if (stripped.Length >= 4)
                    {
                        if (stripped[length - 1] == 'ا' && stripped[length - 2] == 'ت')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";

                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "تا";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "تا");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "تا");
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                        else if (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ت')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";

                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "تے";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "تے");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "تے");
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                        else if (stripped[length - 1] == 'ی' && stripped[length - 2] == 'ت')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "تی";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "تی");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "تی");
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                        else if (stripped[length - 1] == 'ں' && stripped[length - 2] == 'ی' && stripped[length - 3] == 'ت')
                        {
                            wd = pluralForm(wrd.word, 3);
                            if (wd.id.Count > 0)
                            {

                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 3, 3);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "تیں";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "تیں");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "تیں");
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.muarrab = wd.muarrab;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }

                #endregion

                #region نا،نے،نی

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if (stripped[length - 1] == 'ا' && stripped[length - 2] == 'ن')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "نا";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "نا");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "نا");
                                    }
                                    flag = false;
                                }


                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                        else if (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ن')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "نے";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "نے");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "نے");
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                        else if (stripped[length - 1] == 'ی' && stripped[length - 2] == 'ن')
                        {
                            wd = pluralForm(wrd.word, 2);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 2, 2);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "نی";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "نی");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "نی");
                                    }
                                    flag = false;
                                }

                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }


                #endregion

                        #region ئیں

                        else if (stripped[length - 1] == 'ں' && stripped[length - 2] == 'ی' && stripped[length - 3] == 'ئ')
                        {
                            wd = pluralForm(wrd.word, 3);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 3, 3);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "ئیں";
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + "ئیں");
                                    }
                                    flag = false;
                                }
                                else //no change required
                                {
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + "ئیں");
                                    }
                                    flag = false;
                                }

                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }
                        #endregion

                #region ا،ے،ی،و،ہ،ؤ

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if (stripped[length - 1] == 'ا' || stripped[length - 1] == 'ے' || stripped[length - 1] == 'ی' || stripped[length - 1] == 'و' || stripped[length - 1] == 'ہ' || stripped[length - 1] == 'ؤ')
                        {
                            wd = pluralForm(wrd.word, 1);
                            if (wd.id.Count > 0)
                            {

                                string form1 = "", form2 = "";
                                string substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = stripped.Remove(stripped.Length - 1, 1);
                                form2 = form1 + "نا";
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k].Length >= 2)
                                        {
                                            if (wd.code[k][wd.code[k].Length - 1] == '-')
                                            {
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                                wd.code[k] += "x";
                                            }
                                            else if (wd.code[k][wd.code[k].Length - 1] == '=')
                                            {
                                                if (wd.code[k][wd.code[k].Length - 2] == '-')
                                                {
                                                    wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                                    wd.code[k] += "=x";
                                                }
                                                else
                                                {
                                                    wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                                    wd.code[k] += "-x";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            wd.code[k] = wd.code[k].Remove(0, 1);
                                            wd.code[k] += "-x";
                                        }
                                    }
                                    wd.word += stripped[length - 1];
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k] + stripped[length - 1]);
                                    }
                                    flag = false;
                                }
                                else
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k].Length > 2)
                                        {
                                            if (wd.code[k][wd.code[k].Length - 2] == '-')
                                            {
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                                wd.code[k] += "x";
                                            }
                                            else if (wd.code[k][wd.code[k].Length - 2] == '=')
                                            {
                                                if (wd.code[k][wd.code[k].Length - 3] == '-')
                                                {
                                                    wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 3, 3);
                                                    wd.code[k] += "=x";

                                                }
                                                else
                                                {
                                                    wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                                    wd.code[k] += "-x";

                                                }
                                            }
                                        }
                                        else
                                        {
                                            wd.code[k] = wd.code[k].Remove(0, 2);
                                            wd.code[k] += "-x";
                                        }
                                    }
                                    wd.word += wd.word.Remove(wd.word.Length-2) +  stripped[length - 1];
                                    for (int k = 0; k < wd.muarrab.Count; k++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[k].Remove(wd.muarrab[k].Length - 2) + stripped[length - 1]);
                                    }
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }
                #endregion

                #region ات، یات،ئیات

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if ((stripped[length - 1] == 'ت' && stripped[length - 2] == 'ا') || (stripped[length - 1] == 'ت' && stripped[length - 2] == 'ا' && stripped[length - 3] == 'ی'))
                        {
                            string originalWord = wrd.word;
                            wd = pluralFormAat(wrd.word);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {

                                    string form1 = "", form2 = "", form3 = "", form4 = "", form5 = "";
                                    string substr;
                                    string strpd = Araab.removeAraab(originalWord);
                                    substr = Araab.removeAraab(wd.word);
                                    int len = substr.Length;
                                    form1 = strpd.Remove(strpd.Length - 2, 2);   // تصورات
                                    form2 = form1 + "ہ"; //نظریہ،کلیہ
                                    form3 = strpd.Remove(strpd.Length - 2, 1); // آیت،صفت
                                    if (Araab.removeAraab(originalWord)[len - 1] == 'ت' && Araab.removeAraab(originalWord)[len - 2] == 'ا' && Araab.removeAraab(originalWord)[len - 3] == 'ی')
                                    {
                                        form4 = strpd.Remove(strpd.Length - 3, 3);   // اخلاقیات
                                        form5 = form4 + "ہ";
                                        //مادیات
                                        if (substr.Equals(form4))
                                        {
                                            wd.code[k] += "=-";
                                            for (int l = 0; l < wd.muarrab.Count; l++)
                                            {
                                                wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length-3));
                                            }
                                        }
                                        else if (substr.Equals(form5))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "-=-";
                                            for (int l = 0; l < wd.muarrab.Count; l++)
                                            {
                                                wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1));
                                            }
                                        }
                                    }
                                    if (substr.Equals(form1))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "-=-";
                                           
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "=-";
                                           
                                        }
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 2));
                                        }
                                    }
                                    else if (substr.Equals(form2))
                                    {
                                        wd.code[k] = wd.code[k].Replace("x", "=") + "-";
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1));
                                        }
                                    }
                                    else if (substr.Equals(form3))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            wd.code[k] = wd.code[k].Replace("x", "=") + "-";
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                            wd.code[k] += "-=-";
                                        }
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1));
                                        }
                                    }
                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                if ((stripped[length - 1] == 'ت' && stripped[length - 2] == 'ا' && stripped[length - 3] == 'ی'))
                                {
                                    for (int l = 0; l < wrd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab[l] = wrd.muarrab[l] + "یات";
                                    }
                                }
                                else if ((stripped[length - 1] == 'ت' && stripped[length - 2] == 'ا'))
                                {
                                    for (int l = 0; l < wrd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab[l] = wrd.muarrab[l] + "ات";
                                    }
                                }

                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }

                #endregion

                #region وں،اں،یں

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if ((stripped[length - 1] == 'ں' && stripped[length - 2] == 'ا') || (stripped[length - 1] == 'ں' && stripped[length - 2] == 'و') || (stripped[length - 1] == 'ں' && stripped[length - 2] == 'ی'))
                        {
                            string originalWord = wrd.word;
                            wd = pluralFormAan(wrd.word);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {

                                    string form1 = "", form2 = "", form3 = "", form4 = "";
                                    string substr;
                                    string strpd = Araab.removeAraab(originalWord);
                                    substr = Araab.removeAraab(wd.word);
                                    int len = substr.Length;

                                    form1 = strpd.Remove(strpd.Length - 2, 2);   // لڑکیاں
                                    form2 = form1 + "ہ"; //رستوں
                                    form3 = form1 + "ا"; // سودوں
                                    form4 = form1 + "نا"; // دکھاوں

                                    if (substr.Equals(form1))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            if (wd.code[k].Length > 1)
                                            {
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                                wd.code[k] += "-x";   // زادیوں
                                            }
                                            else
                                            {
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                                wd.code[k] += "-x";
                                            }
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "x";
                                        }
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l] + stripped[length - 2] + stripped[length - 1]);
                                        }
                                    }
                                    else if (substr.Equals(form2))
                                    {
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1) + stripped[length - 2] + stripped[length - 1]);
                                        }
                                        // no addition required  رتبوں، رستوں
                                    }
                                    else if (substr.Equals(form3))
                                    {
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1) + stripped[length - 2] + stripped[length - 1]);
                                        }
                                        // no addition required
                                    }
                                    else if (substr.Equals(form4))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "-";
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "-";
                                        }
                                    }
                                    for (int l = 0; l < wd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 2) + stripped[length - 2] + stripped[length - 1]);
                                    }
                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }
                #endregion

                #region ؤں

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if ((stripped[length - 1] == 'ں' && stripped[length - 2] == 'ؤ') )
                        {
                            string originalWord = wrd.word;
                            wd = pluralFormAan(wrd.word);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {

                                    string form1 = "", form2 = "", form3 = "", form4 = "";
                                    string substr;
                                    string strpd = Araab.removeAraab(originalWord);
                                    substr = Araab.removeAraab(wd.word);
                                    int len = substr.Length;

                                    form1 = strpd.Remove(strpd.Length - 2, 2);   // لڑکیاں
                                    form2 = form1 + "ہ"; //رستوں
                                    form3 = form1 + "ا"; // سودوں
                                    form4 = form1 + "نا"; // دکھاوں

                                    if (substr.Equals(form1))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            if (wd.code[k].Length > 1)
                                            {
                                                
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                                wd.code[k] += "=x";
                                            }
                                            else
                                            {
                                                wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                                wd.code[k] += "-x";
                                            }
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "x";
                                        }
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l] + stripped[length - 2] + stripped[length - 1]);
                                        }
                                    }
                                    else if (substr.Equals(form2))
                                    {
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1) + stripped[length - 2] + stripped[length - 1]);
                                        }
                                        // no addition required  رتبوں، رستوں
                                    }
                                    else if (substr.Equals(form3))
                                    {
                                        for (int l = 0; l < wd.muarrab.Count; l++)
                                        {
                                            wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 1) + stripped[length - 2] + stripped[length - 1]);
                                        }
                                        // no addition required
                                    }
                                    else if (substr.Equals(form4))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("=") || wd.code[k][wd.code[k].Length - 1].ToString().Equals("x"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "x";
                                        }
                                        else if (wd.code[k][wd.code[k].Length - 1].ToString().Equals("-"))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1);
                                            wd.code[k] += "x";
                                        }
                                    }
                                    for (int l = 0; l < wd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 2) + stripped[length - 2] + stripped[length - 1]);
                                    }
                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }
                #endregion

                #region نا postfix
                if (flag)
                {
                    if (stripped.Length > 2)
                    {
                        if (stripped[length - 1] == 'ا')
                        {
                            string originalWord = wrd.word;
                            wd = pluralForm(wrd.word, 0);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {
                                    string form1 = "", form2 = "";
                                    string substr;
                                    string strpd = Araab.removeAraab(originalWord);
                                    substr = Araab.removeAraab(wd.word);
                                    int len = substr.Length;

                                    form1 = strpd;
                                    form2 = form1 + "نا";

                                    if (substr.Equals(form1))
                                    {

                                    }
                                    else if (substr.Equals(form2))
                                    {
                                        if (wd.code[k][wd.code[k].Length - 2].Equals('='))
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                            wd.code[k] += "x";
                                        }
                                        else
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 2, 2);
                                            wd.code[k] += "-";
                                        }
                                    }

                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.muarrab = wd.muarrab;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }

                #endregion

                #region ان postfix
                if (flag)
                {
                    if ((stripped[length - 1] == 'ن' && stripped[length - 2] == 'ا'))
                        if (stripped.Length > 2)
                        {
                            string originalWord = wrd.word;
                            wd = pluralFormPostfixAan(wrd.word);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {
                                    if (wd.code[k][wd.code[k].Length - 1].Equals('-'))
                                    {
                                        wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                        wd.code[k] += "=-";
                                    }
                                    else
                                    {
                                        wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                        wd.code[k] += "-=-";
                                    }

                                }
                                for (int l = 0; l < wd.muarrab.Count; l++)
                                {
                                    wrd.muarrab.Add(wd.muarrab[l] + "ان");
                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }

                }

                #endregion



                ///////////////////Special cases/////////////////////////

                #region ئے,تے،تی،تا،نے،ئی

                if (flag)
                {
                    if (stripped.Length > 4)
                    {
                        if ((stripped[length - 1] == 'ی' && stripped[length - 2] == 'ئ') | (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ئ') || (stripped[length - 1] == 'ی' && stripped[length - 2] == 'ت') || (stripped[length - 1] == 'ا' && stripped[length - 2] == 'ت') || (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ت') || (stripped[length - 1] == 'ے' && stripped[length - 2] == 'ن'))
                        {
                            string originalWord = wrd.word;
                            wd = pluralFormYe(wrd.word);
                            if (wd.id.Count > 0)
                            {
                                string form1 = "", form2 = "";
                                string substr;
                                string strpd = Araab.removeAraab(originalWord);
                                substr = Araab.removeAraab(wd.word);
                                int len = substr.Length;

                                form1 = strpd.Remove(strpd.Length - 2, 2);
                                form2 = form1 + "نا"; //رستوں
                                if (substr.Equals(form1))
                                {
                                    for (int k = 0; k < wd.code.Count; k++)
                                    {
                                        if (wd.code[k][wd.code[k].Length - 1] == 'x')
                                        {
                                            wd.code[k] = wd.code[k].Remove(wd.code[k].Length - 1, 1);
                                            wd.code[k] += "=";
                                        }
                                        wd.code[k] += "x";
                                    }
                                    wd.word += "تا";
                                    for (int l = 0; l < wd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[l] + stripped[length - 2].ToString() + stripped[length - 1].ToString());
                                    }
                                    flag = false;

                                }
                                else
                                {
                                    for (int l = 0; l < wd.muarrab.Count; l++)
                                    {
                                        wrd.muarrab.Add(wd.muarrab[l].Remove(wd.muarrab[l].Length - 2) + stripped[length - 2] + stripped[length - 1]);
                                    }
                                    flag = false;

                                }

                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                            if (stripped.Length == 4)
                            {
                                if (isVowelPlusH(stripped[1]))
                                {
                                    code = "=x";
                                }
                                else
                                {
                                    code = "=x";
                                }
                                flag = false;
                            }
                           /* else
                            {
                                if (stripped[0] == 'آ')
                                {
                                    code = "=x";
                                }
                                else
                                {
                                    code = "=x";
                                }
                                flag = false;

                            }*/
                            /*if (!flag)
                            {
                                wrd.id.Add(-1);
                                wrd.code.Add(code);
                                wrd.length = Araab.removeAraab(wrd.word).Length;
                            }*/

                        }
                    }

                }
                #endregion

                #region ال
                if (flag)
                {
                    if (stripped.Length > 1)
                    {
                        if (stripped[0] == 'ا' && stripped[1] == 'ل')
                        {
                            wd = pluralForm(wrd.word, 0);
                            if (wd.id.Count > 0)
                            {
                                for (int k = 0; k < wd.code.Count; k++)
                                {
                                    wd.code[k] = "=" + wd.code[k];
                                }
                                flag = false;
                            }
                            if (!flag)
                            {
                                wrd.id = wd.id;
                                wrd.code = wd.code;
                                wrd.isVaried = wd.isVaried;
                                wrd.language = wd.language;
                                wrd.length = wd.length;
                                wrd.muarrab = wd.muarrab;
                                wrd.taqti = wd.taqti;
                                wrd.taqtiWordGraft = wd.taqtiWordGraft;
                            }
                        }
                    }
                }
                #endregion


             

                if (wrd.id.Count > 0)
                {
                    string word = Araab.removeAraab(wrd.word.Replace("ں", ""));
                    if (isVowelPlusH(word[word.Length-1]))
                    {
                        for (int k = 0; k < wrd.code.Count; k++)
                        {
                            if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("="))   // Word-end flexible syllable
                            {
                                if (Araab.removeAraab(word)[word.Length - 1] == 'ا' || Araab.removeAraab(word)[word.Length - 1] == 'ی' )
                                {
                                    if (wrd.language.Count > 0)
                                    {
                                        bool fl = false;
                                        for (int x = 0; x < wrd.language.Count; x++)
                                        {
                                            if (wrd.language[x].Equals("عربی") && !wrd.modified)
                                            {
                                                fl = true;
                                            }
                                            else if (wrd.language[x].Equals("فارسی") && Araab.removeAraab(word)[word.Length - 1] == 'ا' && !wrd.modified)
                                            {
                                                fl = true;
                                            }
                                        }

                                        if (fl)
                                        {
                                            wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1, 1);
                                            wrd.code[k] += "=";
                                            
                                        }
                                        else
                                        {
                                            wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1, 1);
                                            wrd.code[k] += "x";
                                        }
                                    }
                                    else
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1, 1);
                                        wrd.code[k] += "x";
                                        
                                    }
                                }
                                else
                                {
                                    wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1, 1);
                                    wrd.code[k] += "x";
                                }

                               
                            }
                        }
                    }
                }
            }
            
            return wrd;
        }
        public bool containsNoon(string word)
        {
            if(word.Length > 1)
            {
                for(int i = 0; i < word.Length-1; i++)
                {
                    if(word[i].Equals('ن'))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
            return false;
        }
        public Words pluralFormPostfixAan(string substr)
        {
           
            Words wrd = new Words();
            wrd.word = substr;
            string form1 = "";
            substr = Araab.removeAraab(substr);
            if (substr[0] == 'ا' && substr[1] == 'ل')
            {
                substr = substr.Remove(0, 2);
            }
            int length = substr.Length;
            form1 = substr.Remove(substr.Length - 2, 2);   
            wrd.word = form1;
            wrd = findWord(wrd);
            
            return wrd;
        }
        public Words compoundWord(Words wrd)
        {
            Words wd = new Words();
            wd.word = wrd.word;
            string stripped = Araab.removeAraab(wrd.word);


            for (int i = 1; i < stripped.Length - 1; i++)
            {
                bool flag = false;
                Words first = new Words();
                first.word = stripped.Substring(0, i);
                first = findWord(first);
                Words second = new Words();
                second.word = stripped.Substring(i, stripped.Length - i);
                second = wordCode(second);


                if (first.id.Count > 0)
                {
                    if (second.id.Count == 0)
                    {
                        if (second.word.Length <= 2)
                        {
                            second.code.Add(lengthTwoScan(second.word));
                            second.id.Add(-1);
                            flag = true;
                        }
                    }
                    else  //perfecto!!!
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (second.id.Count > 0)
                    {
                        if (first.word.Length <= 2)
                        {
                            first.code.Add(lengthTwoScan(first.word));
                            first.id.Add(-1);
                            flag = true;
                        }

                    }
                }

                if (flag)
                {
                    first.word += "" + second.word;
                    List<string> codes = new List<string>();

                    for (int k = 0; k < first.code.Count; k++)
                    {
                        for (int j = 0; j < second.code.Count; j++)
                        {
                            codes.Add(first.code[k] + second.code[j]);
                        }
                    }
                    first.code = codes;

                    List<string> muarrab = new List<string>();
                    for (int k = 0; k < first.muarrab.Count; k++)
                    {
                        for (int j = 0; j < second.muarrab.Count; j++)
                        {
                            muarrab.Add(first.muarrab[k] + second.muarrab[j]);
                        }
                    }
                    first.muarrab = muarrab;
                    wd = first;
                    break;
                }
            }
            wd.modified = true;
            return wd;
        }
        static public string removeTashdid(string word)
        {
            string wrd = "";
            if (isMuarrab(word))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    if (word[i].ToString().Equals(Araab.characters[0])) //shadd
                    {
                        if (i - 2 > -1)
                        {
                            if (!isMuarrab(word[i - 2]))
                            {
                                if (!isMuarrab(word[i - 1]))
                                {
                                    wrd = wrd.Remove(wrd.Length - 1);
                                    wrd += word[i - 1] + Araab.characters[2] + word[i - 1] + Araab.characters[8];
                                }
                                else
                                {
                                    wrd = wrd.Remove(wrd.Length - 2);
                                    wrd += word[i - 2] + Araab.characters[2] + word[i - 2] + Araab.characters[8];
                                }
                            }
                            else
                            {
                                wrd += Araab.characters[2] + word[i - 1] + Araab.characters[8];
                            }
                        }
                        else
                        {
                            wrd += Araab.characters[2] + word[i - 1] + Araab.characters[8];
                        }
                    }
                    else
                    {
                        wrd += word[i];
                    }
                }
                return wrd;
            }
            else
                return word;
           
        }
        public string zamzamaFeet(int index,string code)
        {
            string feet = "";
            int len = 0;
            if (code[code.Length - 1].Equals('-'))
            {
                code = code.Remove(code.Length - 1);
                len = code.Length;
            }
            else
            {
                len = code.Length;
            }
            for (int i = 0; i < len; i++)
            {
                if(code[i].Equals('-'))
                {
                    if(i < code.Length)
                    {
                        if(code[++i].Equals('-') )
                        {
                            if (i < code.Length)
                            {
                                if (code[++i].Equals('='))
                                {
                                    feet += " فَعِلن";
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    if (i < code.Length)
                    {
                        if (code[++i].Equals('='))
                        {
                            feet += " فعْلن";
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
                return feet;
        }
        public string hindiFeet(int ind, string code)
        {
            string feet = "";
            int len = 0;
            string[] afail = { "==","=-","-==","-=-","-=","=","==-","-==-"};
            string[] names = { "فعلن", "فعْل", "فعولن", "فعول", "فَعَل", "فع", "فعْلان", "فعولان"};
            int numFeet = 0;
            if(ind == 0 || ind == 1 || ind == 3 || ind == 2 || ind == 4 || ind == 5 || ind == 6 || ind == 7)
            {
                if(code[code.Length-1].Equals('-'))
                {
                    code = code.Remove(code.Length - 1);
                    len = code.Length;
                }
                else
                {
                    len = code.Length;
                }
                if (len > 0)
                {
                    for (int j = 0; j < len; j++)
                    {

                        int index = -1;
                        for (int k = 0; k < afail.Count(); k++)
                        {
                            bool flag = true;
                            index = k;
                            if (j + afail[k].Length > code.Length)
                            {
                                index = -1;
                                flag = false;
                            }
                            else
                            {
                                string slice = code.Substring(j, afail[k].Length);
                                for (int z = 0; z < afail[k].Length; z++)
                                {
                                    if (!((slice[z] == afail[k][z]) ))
                                    {
                                        flag = false;
                                        index = -1;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                                break;
                        }
                        if (index >= 0)
                        {
                            j = j + afail[index].Length - 1;
                            feet += names[index] + " ";
                            numFeet++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                    return feet;
            }
            if (ind == 0 && numFeet == 8)
            {
                return feet;
            }
            else if (ind == 1 && numFeet == 6)
            {
                return feet;
            }
            else if (ind == 2 && numFeet == 8)
            {
                return feet;
            }
            else if (ind == 3 && numFeet == 4)
            {
                return feet;
            }
            else if (ind == 4 && numFeet == 4)
            {
                return feet;
            }
            else if (ind == 5 && numFeet == 3)
            {
                return feet;
            }
            else if (ind == 6 && numFeet == 6)
            {
                return feet;
            }
            else if (ind == 7 && numFeet == 2)
            {
                return feet;
            }
            else
            {
                return " ";
            }
        }
        private string assignAfailFreeVerse(string code, string meter)
        {

        
                List<string> feet = new List<string>();
                bool f = true;
                string afail = "";
                string residue = meter;
                residue = residue.Replace(" ", "");
                residue = residue.Replace("+", " ");
                residue = residue.Replace("/", " ");
                foreach (string s in residue.Split(' '))
                {
                    bool flag = true;
                    for (int j = 0; j < feet.Count; j++)
                    {
                        if (s.Equals(feet[j]))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                        feet.Add(s);
                }

                if (code.Length > 0)
                {
                    for (int j = 0; j < code.Length; j++)
                    {

                        int index = -1;
                        for (int k = 0; k < feet.Count; k++)
                        {
                            bool flag = true;
                            index = k;
                            if (j + feet[k].Length > code.Length)
                            {
                                index = -1;
                                flag = false;
                            }
                            else
                            {
                                string slice = code.Substring(j, feet[k].Length);
                                for (int z = 0; z < feet[k].Length; z++)
                                {
                                    if (!((slice[z] == feet[k][z]) || (slice[z] == 'x')))
                                    {
                                        flag = false;
                                        index = -1;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                                break;
                        }
                        if (index >= 0)
                        {
                            j = j + feet[index].Length - 1;
                            afail += Meters.Rukn(feet[index]) + " ";
                        }
                        else
                        {
                            f = false;
                            break;
                        }
                    }
                    if (f)
                    {

                    }
                }
                else
                    return afail;
          
            return afail;
        }
        private string assignMeterFreeVerse(string code, string meter)
        {


            List<string> feet = new List<string>();
            bool f = true;
            string afail = "";
            string residue = meter;
            residue = residue.Replace(" ", "");
            residue = residue.Replace("+", " ");
            residue = residue.Replace("/", " ");
            foreach (string s in residue.Split(' '))
            {
                bool flag = true;
                for (int j = 0; j < feet.Count; j++)
                {
                    if (s.Equals(feet[j]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    feet.Add(s);
            }

            if (code.Length > 0)
            {
                for (int j = 0; j < code.Length; j++)
                {

                    int index = -1;
                    for (int k = 0; k < feet.Count; k++)
                    {
                        bool flag = true;
                        index = k;
                        if (j + feet[k].Length > code.Length)
                        {
                            index = -1;
                            flag = false;
                        }
                        else
                        {
                            string slice = code.Substring(j, feet[k].Length);
                            for (int z = 0; z < feet[k].Length; z++)
                            {
                                if (!((slice[z] == feet[k][z]) || (slice[z] == 'x')))
                                {
                                    flag = false;
                                    index = -1;
                                    break;
                                }
                            }
                        }
                        if (flag)
                            break;
                    }
                    if (index >= 0)
                    {
                        j = j + feet[index].Length - 1;
                        afail += feet[index];
                    }
                    else
                    {
                        f = false;
                        break;
                    }
                }
                if (f)
                {

                }
            }
            else
                return afail;

            return afail;
        }
        public List<scanPath> Scan(int lineNum)
        {
            if (lineNum <= numLines)
            {
                ////// Deal with Noon ghunna case separately later 
                Lines line = lstLines[lineNum];
                string connectionString = TaqtiController.connectionString;

                List<Words> list = new List<Words>();
                #region word code assignment
                foreach (Words wrd in line.wordsList)
                {
                    string code = "";
                    int id = -1;
                    string stripped = Araab.removeAraab(wrd.word);
                    int length = stripped.Length;

                    if (wrd.code.Count == 0)
                    {
                        Words wd = wordCode(wrd);
                        if (wd.id.Count == 0)
                        {
                            //////////////// Guesswork /////////////////////////////
                            #region Guesswork
                            if (wd.id.Count == 0)
                            {
                                string originalWord = wrd.word;
                                //wrd.word = Araab.removeAraab(originalWord);
                                string newWord = removeTashdid(wrd.word.Replace("\u06BE", "").Replace("\u06BA", "")); /// remove ھ \u06BE and ں \u06BA for scansion purposes
                                wrd.length = Araab.removeAraab(wrd.word).Length;
                                if (wrd.length == 1)
                                {
                                    if (newWord.Equals("آ"))
                                    {
                                        code = "=";
                                    }
                                    else
                                    {
                                        code = "-";
                                    }
                                }
                                else if (wrd.length == 2)
                                {
                                    code = lengthTwoScan(newWord);
                                }
                                else if (wrd.length == 3)
                                {
                                    code = lengthThreeScan(newWord);
                                }
                                else if (wrd.length == 4)
                                {
                                    code = lengthFourScan(newWord);
                                }
                                else if (wrd.length == 5)
                                {
                                    code = lengthFiveScan(newWord);
                                }

                                if (!string.IsNullOrEmpty(code))
                                {
                                    if (code[code.Length - 1].ToString().Equals("="))   // Word-end flexible syllable
                                    {
                                        if (isVowelPlusH(Araab.removeAraab(wrd.word.Replace("ں", ""))[Araab.removeAraab(wrd.word.Replace("ں", "")).Length - 1]))
                                        {
                                            code = code.Remove(code.Length - 1, 1);
                                            code += "x";
                                        }
                                    }
                                }

                                //GuessWork
                                if (stripped.Length > 4 && code.Equals(""))
                                {
                                    wd = compoundWord(wd);
                                    if (wd.id.Count == 0)
                                    {
                                        code = "---";
                                        MySqlConnection myConn = new MySqlConnection(connectionString);
                                        myConn.Open();
                                        MySqlCommand cmd = new MySqlCommand(connectionString);
                                        cmd = myConn.CreateCommand();
                                        cmd.CommandText = "select * from unassigned where word like @word";
                                        cmd.Parameters.AddWithValue("@word", Araab.removeAraab(wd.word));
                                        MySqlDataReader dataReader = cmd.ExecuteReader();

                                        if (!dataReader.HasRows) // look for existing entry in the unassigned table
                                        {
                                            myConn.Close();
                                            MySqlConnection myConn2 = new MySqlConnection(connectionString);
                                            myConn2.Open();
                                            MySqlCommand cmd2 = new MySqlCommand(connectionString);
                                            cmd2 = myConn2.CreateCommand();
                                            cmd2.CommandText = "INSERT into unassigned(word,assigned) VALUES (@word,@assigned);";
                                            cmd2.Parameters.AddWithValue("@word", Araab.removeAraab(wd.word));
                                            cmd2.Parameters.AddWithValue("@assigned", false);

                                            cmd2.ExecuteNonQuery();
                                            myConn2.Close();

                                        }
                                        myConn.Close();
                                    }
                                    else if (code.Equals(""))
                                    {
                                        code = "---";
                                    }

                                }

                                if (wd.id.Count == 0)
                                {
                                    wd.word = originalWord;
                                    wd.code.Add(code);
                                    wd.id.Add(id);
                                }
                            }
                            #endregion
                        }
                        list.Add(wd);
                    }
                    else
                    {
                        list.Add(wrd);
                    }
                }
                line.wordsList = list;
                #endregion

                //////////////////////////////  Al (ال) //////////////////////////////////////////////////

                #region ال
                for (int i = 0; i < line.wordsList.Count - 1; i++)
                {
                    Words wrd = line.wordsList[i];
                    Words nwrd = line.wordsList[i + 1];
                    if (nwrd.word.Length > 1)
                    {
                        if (nwrd.word[0] == 'ا' && nwrd.word[1] == 'ل' && (wrd.word[wrd.word.Length - 1].ToString().Equals(Araab.characters[9]) || wrd.word[wrd.word.Length - 1].ToString().Equals(Araab.characters[8])))
                        {
                            string stripped = Araab.removeAraab(wrd.word);
                            int length = stripped.Length;
                            for (int k = 0; k < wrd.code.Count; k++)
                            {
                                if (isVowelPlusH(stripped[length - 1]))
                                {
                                    if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("=") || wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "=";
                                    }
                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "=";
                                    }
                                }
                                else //consonant
                                {
                                    if (Araab.removeAraab(wrd.word).Length == 2)
                                    {
                                        if (isConsonantPlusConsonant(wrd.word))
                                        {
                                            wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                            wrd.code[k] += "==";
                                        }
                                        else
                                        {
                                            wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                            wrd.code[k] += "=";
                                        }
                                    }
                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("=") || wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "-=";
                                    }
                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "=";
                                    }
                                }
                            }
                            for (int k = 0; k < nwrd.code.Count; k++)
                            {
                                nwrd.code[k] = nwrd.code[k].Remove(0,1);
                            }
                            for (int l = 0; l < wrd.muarrab.Count; l++)
                            {
                                wrd.muarrab[l] = wrd.muarrab[l] + "ل";
                            }
                            for (int l = 0; l < nwrd.muarrab.Count; l++)
                            {
                                nwrd.muarrab[l] = nwrd.muarrab[l].Remove(0,2);
                            }
                        }
                    }
                }
                #endregion


                ///////////////////////////////  Izafat (اضافت) /////////////////////////////////////////

                #region اضافت
                foreach (Words wrd in line.wordsList)
                {
                        if (isIzafat(wrd.word))
                        {
                            if (wrd.id.Count > 0)
                            {
                                int count = wrd.code.Count;
                                for (int k = 0; k < count; k++)
                                {
                                    string tWord = Araab.removeAraab(wrd.word);
                                    ///////////////////// Arabic Monosyllabic Words $3.2 ///////////////////////////////////////
                                    if (wrd.length == 2)
                                    {
                                        wrd.code[k] = "xx";
                                    }
                                    ///////////////////// Consonants and other vowels (ی،ے)////////////////////////////////////

                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("=") || wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        if (tWord[tWord.Length - 1].Equals('ا') || tWord[tWord.Length - 1].Equals('و'))
                                        {
                                            wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                            wrd.code[k] += "=x";
                                        }
                                        else
                                        {
                                            if (tWord[tWord.Length - 1].Equals('ی'))
                                            {
                                                wrd.code.Add(wrd.code[k] + "x");
                                                wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                                wrd.code[k] += "-x";
                                            }
                                            else
                                            {
                                                wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                                wrd.code[k] += "-x";
                                            }
                                        }

                                    }
                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "x";
                                    }
                                }
                            }
                            else
                            {
                                for (int k = 0; k < wrd.code.Count; k++)
                                {
                                    if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("=") || wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "-x";
                                    }
                                    else if (wrd.code[k][wrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        wrd.code[k] = wrd.code[k].Remove(wrd.code[k].Length - 1);
                                        wrd.code[k] += "x";
                                    }

                                }
                            }
                        }
                    
                }
                #endregion

                ////////////////////////////// Ataf (عطف) ////////////////////////////////////////////

                #region عطف
                for (int i = 1; i < line.wordsList.Count; i++)
                {
                    Words wrd = line.wordsList[i];
                    Words pwrd = line.wordsList[i - 1];
                    if (wrd.word.Equals("و"))
                    {
                        string stripped = Araab.removeAraab(pwrd.word);
                        int length = stripped.Length;
                        for (int k = 0; k < pwrd.code.Count; k++)
                        {
                            if (isVowelPlusH(stripped[length - 1]))
                            {
                                if (stripped[length - 1] == 'ا' || stripped[length - 1] == 'ی')
                                {
                                    // do nothing as it already in correct form
                                }
                                else if (stripped[length - 1] == 'ے' || stripped[length - 1] == 'و')
                                {
                                    if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("=") || pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                        pwrd.code[k] += "-x";
                                        for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                    }
                                    else if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                        pwrd.code[k] += "x";
                                        for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                    }
                                }
                                else
                                {
                                    if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("=") || pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("x"))
                                    {
                                        pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                        pwrd.code[k] += "-x";
                                        for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                    }
                                    else if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("-"))
                                    {
                                        pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                        pwrd.code[k] += "x";
                                        for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                    }
                                }

                            }
                            else //consonant
                            {
                                if (length == 2 && isConsonantPlusConsonant(Araab.removeAraab(pwrd.word)))
                                {
                                    pwrd.code[k] = "xx";
                                    for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                }
                                else if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("=") || pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("x"))
                                {
                                    pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                    pwrd.code[k] += "-x";
                                    for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                }
                                else if (pwrd.code[k][pwrd.code[k].Length - 1].ToString().Equals("-"))
                                {
                                    pwrd.code[k] = pwrd.code[k].Remove(pwrd.code[k].Length - 1);
                                    pwrd.code[k] += "x";
                                    for (int j = 0; j < wrd.code.Count; j++) wrd.code[j] = "";
                                }
                            }
                        }

                    }
                }
                #endregion

                //////////////////////////// Word Grafting (وصالِ الف) ////////////////////////////////

                #region وصال الف

                for (int i = 1; i < line.wordsList.Count; i++)
                {
                    Words wrd = line.wordsList[i];
                    Words prevWord = line.wordsList[i - 1];
                    if (wrd.word[0] == 'ا' || wrd.word[0] == 'آ')
                    {
                        if (!isVowelPlusH(Araab.removeAraab(prevWord.word)[Araab.removeAraab(prevWord.word).Length - 1]))
                        {
                            for (int k = 0; k < prevWord.code.Count; k++)
                            {
                                if (prevWord.code[k][prevWord.code[k].Length - 1] == '=')
                                {
                                    string code = line.wordsList[i - 1].code[k];
                                    code = code.Remove(code.Length - 1);
                                    code = code + "-";
                                    line.wordsList[i - 1].taqtiWordGraft.Add(code);
                                    
                                }
                                else if (prevWord.code[k][prevWord.code[k].Length - 1] == '-')
                                {
                                    string code = line.wordsList[i - 1].code[k];
                                    code = code.Remove(code.Length - 1);
                                    line.wordsList[i - 1].taqtiWordGraft.Add(code);
                                }
                            }
                        }
                    }
                }

                #endregion

                return findMeter(line);
            }
            return findMeter(new Lines(""));
        }
        public int calculateScore(string meter, string lineFeet)
        {
            List<string> feet = new List<string>();
            string residue = "";
            foreach(var m in Meters.meterIndex(meter))
            {
                residue +=  Meters.Afail(Meters.meters[m]) + " ";
            }
            foreach (string s in residue.Split(' '))
            {
                bool flag = true;
                for (int j = 0; j < feet.Count; j++)
                {
                    if (s.Trim().Equals(feet[j].Trim()))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && !String.IsNullOrWhiteSpace(s))
                    feet.Add(s.Trim());
            }
            List<string> lineArkan = new List<string>();
            residue = lineFeet;
            foreach (string s in residue.Split(' '))
            {
                bool flag = true;
                for (int j = 0; j < lineArkan.Count; j++)
                {
                    if (s.Trim().Equals(lineArkan[j].Trim()))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag && !String.IsNullOrWhiteSpace(s))
                    lineArkan.Add(s.Trim());
            }
            if(lineArkan.Count() == feet.Count())
            {
                if (isOrdered(lineArkan, feet))
                    return 1;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
            //return 1.0/Convert.ToDouble(lineArkan.Count());
        }
        public bool isOrdered(List<string> lineArkaan, List<string> feet)
        {
            bool flag = true;
            for (int i = 0; i < lineArkaan.Count(); i++)
            {
                if(!lineArkaan[i].Equals(feet[i]))
                {
                    flag = false;
                    break;
                }
            }
            return flag;

        }
        public List<scanOutput> crunch(List<scanOutput> list)
        {
           List<scanOutput> lst = new List<scanOutput>();
           List<string> meterNames = new List<string>();
           bool flag = true;
           for (int i = 0; i < list.Count(); i++)
           {
               flag = true;
               if (meterNames.Count > 0)
               {
                   for (int n = 0; n < meterNames.Count; n++)
                   {
                       if (meterNames[n].Equals(list[i].meterName))
                       {
                           flag = false;
                           break;
                       }
                   }
                   if (flag)
                       meterNames.Add(list[i].meterName);
               }
               else
                   meterNames.Add(list[i].meterName);
               
           }
           double[] scores = new double[meterNames.Count];
           for (int i = 0; i < meterNames.Count; i++)
           {
               for(int j = 0; j<list.Count; j++)
               {
                   if(list[j].meterName.Equals(meterNames[i]))
                   {
                       scores[i] += calculateScore(meterNames[i],list[j].feet);
                   }
               }
           }

           string[] metes = new string[meterNames.Count()];
           for (int i = 0; i < meterNames.Count; i++)
           {
               metes[i] = meterNames[i];
           }
           Array.Sort(scores, metes);
           string finalMeter = metes.Last();
           foreach (var m in list)
           {
               if (m.meterName.Equals(finalMeter))
               {
                   lst.Add(m);
               }
               //for (int i = 0; i < metes.Count(); i++)
               //{
               //    if (m.meterName.Equals(metes[i]))
               //    {
               //        if (scores[i] > 0.0)
               //        {
               //            lst.Add(m);
               //        }
               //    }
               //}
           }
           return lst;
        }
        public List<scanOutputFuzzy> crunchFuzzy(List<scanOutputFuzzy> list)
        {
            if (list.Count > 0)
            {
                List<scanOutputFuzzy> lst = new List<scanOutputFuzzy>();
                List<string> meterNames = new List<string>();
                bool flag = true;
                for (int i = 0; i < list.Count(); i++)
                {
                    flag = true;
                    if (meterNames.Count > 0)
                    {
                        for (int n = 0; n < meterNames.Count; n++)
                        {
                            if (meterNames[n].Equals(list[i].meterName))
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                            meterNames.Add(list[i].meterName);
                    }
                    else
                        meterNames.Add(list[i].meterName);

                }
                double[] scores = new double[meterNames.Count];
                double score = 0.0d;
                double count = 0.0d;
                double subtract = 0.0d;

                for (int i = 0; i < meterNames.Count; i++)
                {
                    score = 0.0d;
                    subtract = 0.0d;
                    count = 0.0d;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].meterName.Equals(meterNames[i]))
                        {

                            if (list[j].score == 0)
                            {
                                score += Math.Log(list[j].score + 1);
                                count += 1.0d;
                                subtract += 1.0d;
                            }
                            else
                            {
                                score += Math.Log(list[j].score);
                                count += 1.0d;
                            }
                        }
                    }
                    scores[i] = Math.Exp(score / count) - subtract;
                }

                string[] metes = new string[meterNames.Count()];
                for (int i = 0; i < meterNames.Count; i++)
                {
                    metes[i] = meterNames[i];
                }
                Array.Sort(scores, metes);
                string finalMeter = metes.First();
                foreach (var m in list)
                {
                    //if (m.meterName.Equals(finalMeter))
                    //{
                    //    lst.Add(m);
                    //}
                    //for (int i = 0; i < metes.Count(); i++)
                    //{
                    if (m.id == -2)
                    {
                        if (m.meterName.Equals(finalMeter))
                        {
                            lst.Add(m);
                        }
                    }
                    else if (Meters.meterIndex(finalMeter).Count > 0)
                    {
                        if (m.id.Equals(Meters.id[Meters.meterIndex(finalMeter).First()]))
                        {
                            lst.Add(m);
                        }
                    }
                   
                }
                return lst;
            }
            else
            {
                return list;
            }
        }
        public List<scanOutput> scanLines()
        {
            List<scanPath> sp = new List<scanPath>();
            scanOutput so;
            List<scanOutput> list = new List<scanOutput>();

            if (!fuzzy && !freeVerse)
            {
                #region Regular Poetry (fixed meter)
                for (int k = 0; k < numLines; k++)
                {
                    sp = Scan(k);
                    if (sp.Count > 0)
                    {
                        int curIndex = list.Count;
                        for (int l = 0; l < sp.Count; l++)
                        {
                            if (sp[l].meters.Count > 0)
                            {
                                for (int m = 0; m < sp[l].meters.Count; m++)
                                {
                                    so = new scanOutput();
                                    so.originalLine = lstLines[k].originalLine;

                                   
                                    for (int i = 1; i < sp[l].location.Count; i++)
                                    {
                                        so.words.Add(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                        so.wordTaqti.Add(sp[l].location[i].code);
                                        if (lstLines[k].wordsList[sp[l].location[i].wordRef].muarrab.Count > sp[l].location[i].codeRef)
                                        {
                                            so.wordMuarrab.Add(lstLines[k].wordsList[sp[l].location[i].wordRef].muarrab[sp[l].location[i].codeRef]);
                                        }
                                        else
                                        {
                                            if(lstLines[k].wordsList[sp[l].location[i].wordRef].taqtiWordGraft.Count > 0)
                                            {
                                                so.wordMuarrab.Add(lstLines[k].wordsList[sp[l].location[i].wordRef].taqtiWordGraft[0]);
                                            }
                                            else
                                            {
                                                so.wordMuarrab.Add(lstLines[k].wordsList[sp[l].location[i].wordRef].word);
                                            }
                                        }
                                    }

                                    string code = "";
                                    for (int x = 0; x < so.wordTaqti.Count; x++)
                                    {
                                        code += so.wordTaqti[x];
                                    }

                                    int index = sp[l].meters[m];
                                    string meter = "";
                                    if (index < Meters.numMeters)
                                    {
                                        so.meterName = Meters.meterNames[index];
                                        
                                        if(Meters.usage[index] == 0)
                                        {
                                            List<Feet> feet = Meters.Afail2(Meters.meters[index], code);
                                            foreach (var v in feet)
                                            {
                                                so.feet += v.foot + " ";
                                            }
                                           so.feet += " (غیر مستعمل وزن)";
                                        }
                                        else
                                        {
                                            List<Feet> feet = Meters.Afail2(Meters.meters[index], code);
                                            foreach (var v in feet)
                                            {
                                                so.feet += v.foot + " ";
                                            }
                                        }
                                        meter = Meters.meters[index];
                                        so.id = Meters.id[index];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters && index >= Meters.numMeters)
                                    {
                                        so.meterName = Meters.metersVeriedNames[index - Meters.numMeters];
                                        so.feet = Meters.Afail(Meters.metersVaried[index - Meters.numMeters]);
                                        meter = Meters.metersVaried[index - Meters.numMeters];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && index >= Meters.numMeters + Meters.numVariedMeters)
                                    {
                                        so.meterName = Meters.rubaiMeterNames[index - Meters.numMeters - Meters.numVariedMeters] + "(رباعی)";
                                        so.feet = Meters.Afail(Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters]);
                                        meter = Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters];
                                        so.id = -2;
                                    }
                                    else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                      int ind = index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters;
                                      if (ind > 7)
                                          so.feet = zamzamaFeet(ind, code);
                                      else
                                        so.feet = hindiFeet(ind, code);
                                        meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);

                                    }
                                    bool flag = false;
                                    if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {

                                        string mete = "", met1 = "", met2 = "", met3 = "", met4 = "";
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met1;
                                        }
                                        #endregion
                                        #region Variation1
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met2;
                                        }
                                        #endregion
                                        #region Variation2
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met3;
                                        }
                                        #endregion
                                        #region Variation3
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met4;
                                        }
                                        #endregion




                                        int z = 0;
                                        for (int x = 0; x < so.wordTaqti.Count; x++)
                                        {
                                            string temp = "";
                                            for (int y = 0; y < so.wordTaqti[x].Length; y++)
                                            {
                                                temp += mete[z];
                                                z = z + 1;
                                            }
                                            so.wordTaqti[x] = temp;
                                        }
                                        /*
                                       so.feetList = Meters.Afail2(meter, mete);

                                        #region syllable breakup
                                        for (int x = 0; x < so.wordMuarrab.Count; x++)
                                        {
                                            string wrd = Scansion.removeTashdid(so.wordMuarrab[x].Replace("\u06BE", "").Replace("\u06BA", ""));
                                            string cd = so.wordTaqti[x];
                                            so.words[x].breakup.Clear();

                                            z = 0;
                                            for (int y = 0; y < cd.Length; y++)
                                            {
                                                if (cd[y] == '-')
                                                {
                                                    string syl = "";
                                                    if (wrd.Length >= z + 1)
                                                    {
                                                        if (y == cd.Length - 1)  //word-end case
                                                        {
                                                            if (!isMuarrab(wrd[z]))
                                                            {
                                                                syl = wrd[z].ToString();
                                                                so.words[x].breakup.Add(syl);
                                                                z = z + 1;
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                z = z + 1;
                                                                y = y - 1;
                                                            }
                                                        }
                                                        else if (!isMuarrab(wrd[z]))
                                                        {
                                                            if (wrd.Length > z + 1)
                                                            {
                                                                if (isMuarrab(wrd[z + 1]))
                                                                {
                                                                    syl = wrd[z].ToString() + wrd[z + 1].ToString();
                                                                    z = z + 2;
                                                                }
                                                                else
                                                                {
                                                                    syl = wrd[z].ToString();
                                                                    z = z + 1;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                syl = wrd[z].ToString();
                                                                z = z + 1;
                                                            }
                                                            so.words[x].breakup.Add(syl);
                                                        }
                                                        else
                                                        {
                                                            z = z + 1;
                                                            y = y - 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        syl = wrd[z - 1].ToString();
                                                        so.words[x].breakup.Add(syl);
                                                        z = z + 1;
                                                    }
                                                }
                                                else if (cd[y] == '=')
                                                {
                                                    string syl = "";
                                                    if (wrd.Length >= z + 2)
                                                    {
                                                        if (!isMuarrab(wrd[z]))
                                                        {
                                                            if (wrd[z] == 'آ' || wrd[z] == 'ؤ')
                                                            {
                                                                syl = wrd[z].ToString();
                                                                z = z + 1;
                                                            }
                                                            else if (isMuarrab(wrd[z + 1]))
                                                            {
                                                                if (wrd.Length > z + 2)
                                                                {
                                                                    if (!isMuarrab(wrd[z + 2]))
                                                                    {
                                                                        syl = wrd[z].ToString() + wrd[z + 1].ToString() + wrd[z + 2].ToString();
                                                                        z = z + 3;
                                                                    }
                                                                    else
                                                                    {
                                                                        z = z + 1;
                                                                        y = y - 1;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    z = z + 1;
                                                                    y = y - 1;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (wrd.Length > z + 2)
                                                                {
                                                                    if (!isMuarrab(wrd[z + 2]))
                                                                    {
                                                                        syl = wrd[z].ToString() + wrd[z + 1].ToString();
                                                                        z = z + 2;
                                                                    }
                                                                    else
                                                                    {
                                                                        syl = wrd[z].ToString() + wrd[z + 1].ToString() + wrd[z + 2].ToString();
                                                                        z = z + 3;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    syl = wrd[z].ToString() + wrd[z + 1].ToString();
                                                                    z = z + 2;
                                                                }
                                                            }
                                                            so.words[x].breakup.Add(syl);
                                                        }
                                                        else
                                                        {
                                                            z = z + 1;
                                                            y = y - 1;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        syl = wrd[z - 1].ToString() + "ے";
                                                        so.words[x].breakup.Add(syl);
                                                        z = z + 2;
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }

                                            }

                                        }
                                        #endregion
                                        #region syllable populating
                                        List<string> sylList = new List<string>();
                                        for (int x = 0; x < so.words.Count; x++)
                                        {
                                            for (int y = 0; y < so.words[x].breakup.Count; y++)
                                            {
                                                sylList.Add(so.words[x].breakup[y]);
                                            }
                                        }
                                        z = 0;
                                        for (int x = 0; x < so.feetList.Count; x++)
                                        {
                                            string wrds = "";
                                            for (int y = 0; y < so.feetList[x].code.Length; y++)
                                            {
                                                wrds += sylList[z] + "  ";
                                                z = z + 1;
                                            }
                                            so.feetList[x].words = wrds;
                                        }
                                        #endregion

                                        */
                                    }

                                        flag = true;
                                        if (list.Count > 0)
                                        {
                                            for (int n = curIndex; n < list.Count; n++)
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        if (flag)
                                            list.Add(so);
                                    
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else if (freeVerse)
            {
                #region Free Verse
                for (int k = 0; k < numLines; k++)
                {
                    sp = Scan(k);
                    if (sp.Count > 0)
                    {
                        int curIndex = list.Count;
                        for (int l = 0; l < sp.Count; l++)
                        {
                            if (sp[l].meters.Count > 0)
                            {
                                for (int m = 0; m < sp[l].meters.Count; m++)
                                {
                                    so = new scanOutput();
                                    so.originalLine = lstLines[k].originalLine;


                                    for (int i = 1; i < sp[l].location.Count; i++)
                                    {
                                        so.words.Add(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                        so.wordTaqti.Add(sp[l].location[i].code);
                                    }

                                    string code = "";
                                    for (int x = 0; x < so.wordTaqti.Count; x++)
                                    {
                                        code += so.wordTaqti[x];
                                    }

                                    int index = sp[l].meters[m];
                                    string meter = "";
                                    if (index < Meters.numMeters)
                                    {
                                        so.meterName = Meters.meterNames[index];
                                        so.feet = assignAfailFreeVerse(code, Meters.meters[index]);
                                        meter = Meters.meters[index];
                                        so.id = Meters.id[index];
                                    }
                                    else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        int ind = index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters;
                                        if (ind > 7)
                                            so.feet = zamzamaFeet(ind, code);
                                        else
                                            so.feet = hindiFeet(ind, code);
                                        meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);

                                    }
                                    bool flag = false;
                                    if (index < Meters.numMeters + Meters.numVariedMeters)
                                    {
                                        string mete = "";
                                        mete = assignMeterFreeVerse(code, Meters.meters[index]);
                                        if (mete.Length > 0)
                                        {
                                            int z = 0;
                                            for (int x = 0; x < so.wordTaqti.Count; x++)
                                            {
                                                string temp = "";
                                                if (so.wordTaqti[x].Length + z <= mete.Length)
                                                {
                                                    for (int y = 0; y < so.wordTaqti[x].Length; y++)
                                                    {
                                                        temp += mete[z];
                                                        z = z + 1;
                                                    }
                                                    so.wordTaqti[x] = temp;
                                                }
                                                else
                                                {
                                                    int t;
                                                    t = 20;
                                                }
                                            }
                                        }
                                    }


                                    flag = true;
                                    if (list.Count > 0)
                                    {
                                        for (int n = curIndex; n < list.Count; n++)
                                        {
                                            if (!string.IsNullOrEmpty(list[n].meterName))
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                            else
                                            {
                                                flag = false;
                                            }
                                        }
                                    }
                                    if (flag)
                                    {
                                        if (!string.IsNullOrEmpty(so.meterName))
                                        list.Add(so);
                                    }
                                }
                            }
                        }
                    }
                }
                list = crunch(list);

                #endregion
            }

            return list;
        }
        public List<scanOutputFuzzy> scanLinesFuzzy()
        {
            List<scanPath> sp = new List<scanPath>();
            scanOutputFuzzy so;
            List<scanOutputFuzzy> list = new List<scanOutputFuzzy>();

            #region Create Poetry/ Corrections
            for (int k = 0; k < numLines; k++)
            {
                sp = Scan(k);
                if (sp.Count > 0)
                {
                    int curIndex = list.Count;
                    for (int l = 0; l < sp.Count; l++)
                    {
                        if (sp[l].meters.Count > 0)
                        {
                            for (int m = 0; m < sp[l].meters.Count; m++)
                            {
                                so = new scanOutputFuzzy();
                                so.originalLine = lstLines[k].originalLine;

                                int index = sp[l].meters[m];
                                string meter = "";
                                if (index < Meters.numMeters)
                                {
                                    so.meterName = Meters.meterNames[index];
                                    meter = Meters.meters[index];
                                    so.id = Meters.id[index];
                                }
                                else if (index < Meters.numMeters + Meters.numVariedMeters && index >= Meters.numMeters)
                                {
                                    so.meterName = Meters.metersVeriedNames[index - Meters.numMeters];
                                    meter = Meters.metersVaried[index - Meters.numMeters];
                                }
                                else if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && index >= Meters.numMeters + Meters.numVariedMeters)
                                {
                                    so.meterName = Meters.rubaiMeterNames[index - Meters.numMeters - Meters.numVariedMeters] + "(رباعی)";
                                    meter = Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters];
                                    so.id = -2;
                                }
                                else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                {
                                    so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                    meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                    so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);
                                }
                                bool flag = false;
                                if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                {
                                    string code = "";
                                    for (int x = 0; x < sp[l].location.Count; x++)
                                    {
                                        code += sp[l].location[x].code;
                                    }
                                    string originalMeter = meter;
                                    meter = meter.Replace("/", "");
                                    string meter2 = meter.Replace("+", "") + "~";
                                    string meter3 = meter.Replace("+", "~") + "~";
                                    string meter4 = meter.Replace("+", "~");
                                    meter = meter.Replace("+", "");

                                    string[] meters = { meter, meter2, meter3, meter4 };



                                    int[] scores = new int[4];
                                    int[] indices = { 0, 1, 2, 3 };
                                    scores[0] = LevenshteinDistance(meter, code);
                                    scores[1] = LevenshteinDistance(meter2, code);
                                    scores[2] = LevenshteinDistance(meter3, code);
                                    scores[3] = LevenshteinDistance(meter4, code);

                                    if (scores[1] == 0 || scores[2] == 0 || scores[3] == 0)
                                    {
                                        Array.Sort(scores, indices);
                                        meter = meters[indices[0]];
                                    }
                                    else
                                    {
                                        meter = meters[0];
                                    }
                                    string[,] table = matchFuzzy(meter, code);
                                    so.score = Convert.ToInt32(table[0, 2]);
                                }
                                flag = true;
                                if (list.Count > 0)
                                {
                                    for (int n = curIndex; n < list.Count; n++)
                                    {
                                        if (list[n].id == so.id)
                                        {
                                            if (list[n].score < so.score)
                                            {
                                                flag = false;
                                            }
                                            else
                                            {
                                                list[n] = so;
                                                flag = false;
                                            }
                                        }
                                    }
                                }
                                if (flag)
                                    list.Add(so);
                            }
                        }
                    }
                }
            }
            #endregion
            List<scanOutputFuzzy> listtemp = crunchFuzzy(list);
            if(listtemp.Count > 0)
            {
                List<int> idList = new List<int>();
                if (listtemp.First().id > 0)
                {
                    for (int i = 0; i < Meters.numMeters; i++)
                    {
                        if (listtemp.First().id == Meters.id[i])
                        {
                            idList.Add(i);
                        }
                    }
                }
                else
                {
                    idList.Add(listtemp.First().id);
                }
                meter = idList;
                list.Clear();
                errorParam = 7;
                #region Create Poetry/ Corrections
                for (int k = 0; k < numLines; k++)
                {
                    sp.Clear();
                   for(int ind=0; ind< lstLines[k].wordsList.Count; ind++)
                   {
                       lstLines[k].wordsList[ind].breakup.Clear();
                       lstLines[k].wordsList[ind].code.Clear();
                       lstLines[k].wordsList[ind].id.Clear();
                       lstLines[k].wordsList[ind].language.Clear();
                       lstLines[k].wordsList[ind].taqti.Clear();
                       lstLines[k].wordsList[ind].taqtiWordGraft.Clear();
                   }
                    sp = Scan(k);
                    if (sp.Count > 0)
                    {
                        int curIndex = list.Count;
                        for (int l = 0; l < sp.Count; l++)
                        {
                            if (sp[l].meters.Count > 0)
                            {
                                for (int m = 0; m < sp[l].meters.Count; m++)
                                {
                                    so = new scanOutputFuzzy();
                                    so.originalLine = lstLines[k].originalLine;

                                    int index = sp[l].meters[m];
                                    string meterstring = "";
                                    if (index < Meters.numMeters)
                                    {
                                        so.meterName = Meters.meterNames[index];
                                        if (Meters.usage[index] == 0)
                                        {
                                            so.feet = Meters.Afail(Meters.meters[index]) + " (غیر مستعمل وزن)";
                                        }
                                        else
                                        {
                                            so.feet = Meters.Afail(Meters.meters[index]);
                                        }
                                        meterstring = Meters.meters[index];
                                        so.id = Meters.id[index];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters && index >= Meters.numMeters)
                                    {
                                        so.meterName = Meters.metersVeriedNames[index - Meters.numMeters];
                                        so.feet = Meters.Afail(Meters.metersVaried[index - Meters.numMeters]);
                                        meterstring = Meters.metersVaried[index - Meters.numMeters];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && index >= Meters.numMeters + Meters.numVariedMeters)
                                    {
                                        so.meterName = Meters.rubaiMeterNames[index - Meters.numMeters - Meters.numVariedMeters] + "(رباعی)";
                                        so.feet = Meters.Afail(Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters]);
                                        meterstring = Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters];
                                        so.id = -2;
                                    }
                                    else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.feet = "";
                                        meterstring = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);
                                    }
                                    bool flag = false;
                                    if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        string code = "";
                                        for (int x = 0; x < sp[l].location.Count; x++)
                                        {
                                            code += sp[l].location[x].code;
                                        }
                                        string originalMeter = meterstring;
                                        meterstring = meterstring.Replace("/", "");
                                        string meter2 = meterstring.Replace("+", "") + "~";
                                        string meter3 = meterstring.Replace("+", "~") + "~";
                                        string meter4 = meterstring.Replace("+", "~");
                                        meterstring = meterstring.Replace("+", "");

                                        string[] meters = { meterstring, meter2, meter3, meter4 };



                                        int[] scores = new int[4];
                                        int[] indices = { 0, 1, 2, 3 };
                                        scores[0] = LevenshteinDistance(meterstring, code);
                                        scores[1] = LevenshteinDistance(meter2, code);
                                        scores[2] = LevenshteinDistance(meter3, code);
                                        scores[3] = LevenshteinDistance(meter4, code);

                                        Array.Sort(scores, indices);
                                        meterstring = meters[indices[0]];

                                        string[,] table = matchFuzzy(meterstring, code);
                                        so.score = Convert.ToInt32(table[0, 2]);
                                        for (int u = table.Length / 3 - 1; u >= 0; u--)
                                        {
                                            if (!String.IsNullOrEmpty(table[u, 0]))
                                            {
                                                so.meterSyllables.Add(table[u, 0]);
                                                so.codeSyllables.Add(table[u, 1]);
                                            }
                                        }
                                        int j = 0;
                                        for (j = 0; j < table.Length / 3; j++)
                                        {
                                            if (String.IsNullOrEmpty(table[j, 1]))
                                            {
                                                j--;
                                                break;
                                            }
                                        }



                                        for (int i = 1; i < sp[l].location.Count; i++)
                                        {
                                            Words wrd = new Words(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                            string cd = "";
                                            for (int u = 0; u < sp[l].location[i].code.Length; u++)
                                            {
                                                if (j >= 0)
                                                {
                                                    if (!String.IsNullOrEmpty(table[j, 1]))
                                                    {
                                                        if (table[j, 1].Equals(" "))
                                                        {
                                                            cd += "?";
                                                            u--;
                                                            wrd.error = true;
                                                        }
                                                        else if (table[j, 0].Equals(" "))
                                                        {
                                                            if (table[j, 1].Equals("x"))
                                                            {
                                                                cd += "(=)";
                                                            }
                                                            else
                                                            {
                                                                cd += "(" + table[j, 1].ToString() + ")";
                                                            }

                                                            wrd.error = true;
                                                        }
                                                        else
                                                        {
                                                            if (table[j, 1].Contains("]"))
                                                            {
                                                                wrd.error = true;
                                                            }
                                                            if (table[j, 1].Equals("x"))
                                                            {
                                                                cd += "=";
                                                            }
                                                            else
                                                            {
                                                                cd += table[j, 1].ToString();
                                                            }
                                                        }
                                                    }
                                                    j--;
                                                }

                                            }

                                            so.words.Add(wrd);
                                            so.wordTaqti.Add(cd);
                                            so.orignalTaqti.Add(cd.Replace("[", "").Replace("]", "").Replace("?", "").Replace("(", "").Replace(")", ""));
                                        }
                                        if (j >= 0)
                                        {
                                            for (int v = 0; v <= j; v++)
                                            {
                                                so.wordTaqti[so.wordTaqti.Count - 1] += "?";
                                                so.words[so.words.Count - 1].error = true;
                                            }
                                        }
                                    }
                                    flag = true;
                                    if (list.Count > 0)
                                    {
                                        for (int n = curIndex; n < list.Count; n++)
                                        {
                                            if (list[n].id == so.id)
                                            {
                                                if (list[n].score < so.score)
                                                {
                                                    flag = false;
                                                }
                                                else
                                                {
                                                    list[n] = so;
                                                    flag = false;
                                                }
                                            }
                                        }
                                    }
                                    if (flag)
                                        list.Add(so);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                list.Clear();
            }
            
            return list;

        }
        public List<scanOutputFuzzy> scanLineFuzzy(int id)
        {
            List<scanPath> sp = new List<scanPath>();
            scanOutputFuzzy so;
            List<scanOutputFuzzy> list = new List<scanOutputFuzzy>();

            #region Create Poetry/ Corrections
            for (int k = 0; k < numLines; k++)
            {
                sp = Scan(k);
                if (sp.Count > 0)
                {
                    int curIndex = list.Count;
                    for (int l = 0; l < sp.Count; l++)
                    {
                        if (sp[l].meters.Count > 0)
                        {
                            for (int m = 0; m < sp[l].meters.Count; m++)
                            {
                                so = new scanOutputFuzzy();
                                so.id = id;
                                so.originalLine = lstLines[k].originalLine;

                                int index = sp[l].meters[m];
                                string meter = "";
                                if (index < Meters.numMeters)
                                {
                                    so.meterName = Meters.meterNames[index];
                                    so.feet = Meters.Afail(Meters.meters[index]);
                                    meter = Meters.meters[index];
                                   // so.id = Meters.id[index];
                                }
                                else if (index < Meters.numMeters + Meters.numVariedMeters && index >= Meters.numMeters)
                                {
                                    so.meterName = Meters.metersVeriedNames[index - Meters.numMeters];
                                    so.feet = Meters.Afail(Meters.metersVaried[index - Meters.numMeters]);
                                    meter = Meters.metersVaried[index - Meters.numMeters];
                                }
                                else if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && index >= Meters.numMeters + Meters.numVariedMeters)
                                {
                                    so.meterName = Meters.rubaiMeterNames[index - Meters.numMeters - Meters.numVariedMeters] + "(رباعی)";
                                    so.feet = Meters.Afail(Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters]);
                                    meter = Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters];
                                }
                                else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                {
                                    so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                    so.feet = "";
                                    meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                }
                                bool flag = false;
                                if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                {
                                    string code = "";
                                    for (int x = 0; x < sp[l].location.Count; x++)
                                    {
                                        code += sp[l].location[x].code;
                                    }
                                    string originalMeter = meter;
                                    meter = meter.Replace("/", "");
                                    string meter2 = meter.Replace("+", "") + "~";
                                    string meter3 = meter.Replace("+", "~") + "~";
                                    string meter4 = meter.Replace("+", "~");
                                    meter = meter.Replace("+", "");

                                    string[] meters = { meter, meter2, meter3, meter4 };



                                    int[] scores = new int[4];
                                    int[] indices = { 0, 1, 2, 3 };
                                    scores[0] = LevenshteinDistance(meter, code);
                                    scores[1] = LevenshteinDistance(meter2, code);
                                    scores[2] = LevenshteinDistance(meter3, code);
                                    scores[3] = LevenshteinDistance(meter4, code);

                                    Array.Sort(scores, indices);
                                    meter = meters[indices[0]];

                                    string[,] table = matchFuzzy(meter, code);
                                    so.score = Convert.ToInt32(table[0, 2]);
                                    for (int u = table.Length / 3 - 1; u >= 0; u--)
                                    {
                                        if (!String.IsNullOrEmpty(table[u, 0]))
                                        {
                                            so.meterSyllables.Add(table[u, 0]);
                                            so.codeSyllables.Add(table[u, 1]);
                                        }
                                    }
                                    int j = 0;
                                    for (j = 0; j < table.Length / 3; j++)
                                    {
                                        if (String.IsNullOrEmpty(table[j, 1]))
                                        {
                                            j--;
                                            break;
                                        }
                                    }



                                    for (int i = 1; i < sp[l].location.Count; i++)
                                    {
                                        Words wrd = new Words(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                        string cd = "";
                                        for (int u = 0; u < sp[l].location[i].code.Length; u++)
                                        {
                                            if (j >= 0)
                                            {
                                                if (!String.IsNullOrEmpty(table[j, 1]))
                                                {
                                                    if (table[j, 1].Equals(" "))
                                                    {
                                                        cd += "?";
                                                        u--;
                                                        wrd.error = true;
                                                    }
                                                    else if (table[j, 0].Equals(" "))
                                                    {
                                                        if (table[j, 1].Equals("x"))
                                                        {
                                                            cd += "(=)";
                                                        }
                                                        else
                                                        {
                                                            cd += "(" + table[j, 1].ToString() + ")";
                                                        }

                                                        wrd.error = true;
                                                    }
                                                    else
                                                    {
                                                        if (table[j, 1].Contains("]"))
                                                        {
                                                            wrd.error = true;
                                                        }
                                                        if (table[j, 1].Equals("x"))
                                                        {
                                                            cd += "=";
                                                        }
                                                        else
                                                        {
                                                            cd += table[j, 1].ToString();
                                                        }
                                                    }
                                                }
                                                j--;
                                            }

                                        }

                                        so.words.Add(wrd);
                                        so.wordTaqti.Add(cd);
                                        so.orignalTaqti.Add(cd.Replace("[", "").Replace("]", "").Replace("?", "").Replace("(", "").Replace(")", ""));
                                    }
                                    if (j > 0)
                                    {
                                        for (int v = 0; v <= j; v++)
                                        {
                                            so.wordTaqti[so.wordTaqti.Count - 1] += "?";
                                            so.words[so.words.Count - 1].error = true;
                                        }
                                    }

                                }
                                flag = true;
                                if (list.Count > 0)
                                {
                                    for (int n = curIndex; n < list.Count; n++)
                                    {
                                        if (list[n].id == so.id)
                                        {
                                            if (list[n].score < so.score)
                                            {
                                                flag = false;
                                            }
                                            else
                                            {
                                                list[n] = so;
                                                flag = false;
                                            }
                                        }
                                    }
                                }
                                if (flag)
                                    list.Add(so);

                            }
                        }
                    }
                }
            }
            #endregion

            return list;
        }
        public List<scanOutput> scanOneLine(int id)
        {
            List<scanPath> sp = new List<scanPath>();
            scanOutput so;
            List<scanOutput> list = new List<scanOutput>();
            if (!fuzzy && !freeVerse)
            {
                #region Regular Poetry (fixed meter)
                for (int k = 0; k < numLines; k++)
                {
                    sp = Scan(k);
                    if (sp.Count > 0)
                    {
                        int curIndex = list.Count;
                        for (int l = 0; l < sp.Count; l++)
                        {
                            if (sp[l].meters.Count > 0)
                            {
                                for (int m = 0; m < sp[l].meters.Count; m++)
                                {
                                    so = new scanOutput();
                             
                                    so.originalLine = lstLines[k].originalLine;


                                    for (int i = 1; i < sp[l].location.Count; i++)
                                    {
                                        so.words.Add(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                        so.wordTaqti.Add(sp[l].location[i].code);
                                    }

                                    string code = "";
                                    for (int x = 0; x < so.wordTaqti.Count; x++)
                                    {
                                        code += so.wordTaqti[x];
                                    }

                                    int index = sp[l].meters[m];
                                    string meter = "";
                                    if (index < Meters.numMeters)
                                    {
                                        so.meterName = Meters.meterNames[index];
                                        if (Meters.usage[index] == 0)
                                        {
                                            so.feet = Meters.Afail(Meters.meters[index]) + " (غیر مستعمل وزن)";
                                        }
                                        else
                                        {
                                            so.feet = Meters.Afail(Meters.meters[index]);
                                        }
                                        meter = Meters.meters[index];
                                        so.id = Meters.id[index];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters && index >= Meters.numMeters)
                                    {
                                        so.meterName = Meters.metersVeriedNames[index - Meters.numMeters];
                                        so.feet = Meters.Afail(Meters.metersVaried[index - Meters.numMeters]);
                                        meter = Meters.metersVaried[index - Meters.numMeters];
                                    }
                                    else if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && index >= Meters.numMeters + Meters.numVariedMeters)
                                    {
                                        so.meterName = Meters.rubaiMeterNames[index - Meters.numMeters - Meters.numVariedMeters] + "(رباعی)";
                                        so.feet = Meters.Afail(Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters]);
                                        meter = Meters.rubaiMeters[index - Meters.numMeters - Meters.numVariedMeters];
                                        so.id = -2;
                                    }
                                    else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        int ind = index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters;
                                        if (ind > 7)
                                            so.feet = zamzamaFeet(ind, code);
                                        else
                                            so.feet = hindiFeet(ind, code);
                                        meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);

                                    }
                                    bool flag = false;
                                    if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {




                                        string mete = "", met1 = "", met2 = "", met3 = "", met4 = "";
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met1;
                                        }
                                        #endregion
                                        #region Variation1
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met2;
                                        }
                                        #endregion
                                        #region Variation2
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met3;
                                        }
                                        #endregion
                                        #region Variation3
                                        flag = false;
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
                                                        flag = true;
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
                                                        flag = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            flag = true;
                                        }
                                        if (!flag)
                                        {
                                            mete = met4;
                                        }
                                        #endregion


                                        int z = 0;
                                        for (int x = 0; x < so.wordTaqti.Count; x++)
                                        {
                                            string temp = "";
                                            for (int y = 0; y < so.wordTaqti[x].Length; y++)
                                            {
                                                temp += mete[z];
                                                z = z + 1;
                                            }
                                            so.wordTaqti[x] = temp;
                                        }
                                    }

                                    so.id = id;

                                    if (numLines > 1)
                                    {
                                        flag = true;
                                        if (list.Count > 0)
                                        {
                                            for (int n = 0; n < list.Count; n++)
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if(so.feet.Count() > 0)
                                            list.Add(so);
                                        }
                                        if (!flag)
                                        {
                                            if (so.feet.Count() > 0)
                                            list.Add(so);
                                        }
                                    }
                                    else
                                    {
                                        flag = true;
                                        if (list.Count > 0)
                                        {
                                            for (int n = curIndex; n < list.Count; n++)
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        if (flag)
                                        {
                                            if (so.feet.Count() > 0)
                                            list.Add(so);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }

            if (freeVerse)
            {
                #region Free Verse
                for (int k = 0; k < numLines; k++)
                {
                    sp = Scan(k);
                    if (sp.Count > 0)
                    {
                        int curIndex = list.Count;
                        for (int l = 0; l < sp.Count; l++)
                        {
                            if (sp[l].meters.Count > 0)
                            {
                                for (int m = 0; m < sp[l].meters.Count; m++)
                                {
                                    so = new scanOutput();
                                    so.originalLine = lstLines[k].originalLine;


                                    for (int i = 1; i < sp[l].location.Count; i++)
                                    {
                                        so.words.Add(lstLines[k].wordsList[sp[l].location[i].wordRef]);
                                        so.wordTaqti.Add(sp[l].location[i].code);
                                    }

                                    string code = "";
                                    for (int x = 0; x < so.wordTaqti.Count; x++)
                                    {
                                        code += so.wordTaqti[x];
                                    }

                                    int index = sp[l].meters[m];
                                    string meter = "";
                                    if (index < Meters.numMeters)
                                    {
                                        so.meterName = Meters.meterNames[index];
                                        so.feet = assignAfailFreeVerse(code,Meters.meters[index]);
                                        meter = Meters.meters[index];
                                        so.id = Meters.id[index];
                                    }
                                    else if (index >= Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        so.meterName = Meters.specialMeterNames[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        int ind = index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters;
                                        if (ind > 7)
                                            so.feet = zamzamaFeet(ind, code);
                                        else
                                            so.feet = hindiFeet(ind, code);
                                        meter = Meters.specialMeters[index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters];
                                        so.id = -2 - (index - Meters.numMeters - Meters.numVariedMeters - Meters.numRubaiMeters);

                                    }
                                    bool flag = false;
                                    if (index < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters)
                                    {
                                        string mete = "";
                                        mete= assignMeterFreeVerse(code, Meters.meters[index]);

                                        int z = 0;
                                        for (int x = 0; x < so.wordTaqti.Count; x++)
                                        {
                                            string temp = "";
                                            for (int y = 0; y < so.wordTaqti[x].Length; y++)
                                            {
                                                temp += mete[z];
                                                z = z + 1;
                                            }
                                            so.wordTaqti[x] = temp;
                                        }
                                    }

                                    if (numLines < 1)
                                    {
                                        flag = true;
                                        if (list.Count > 0)
                                        {
                                            for (int n = 0; n < list.Count; n++)
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        else
                                            list.Add(so);
                                        if (!flag)
                                            list.Add(so);
                                    }
                                    else
                                    {
                                        flag = true;
                                        if (list.Count > 0)
                                        {
                                            for (int n = curIndex; n < list.Count; n++)
                                            {
                                                if (list[n].meterName.Equals(so.meterName))
                                                {
                                                    flag = false;
                                                }
                                            }
                                        }
                                        if (flag)
                                            list.Add(so);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            return list;
        }
    }
}