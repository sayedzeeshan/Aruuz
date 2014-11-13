using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Aruuz.Controllers;
using System.Text.RegularExpressions;

namespace Aruuz.Models
{
    public class CodeTree
    {
        codeLocation location;
        List<CodeTree> children;
        public int errorParam = 2;
        public CodeTree(codeLocation loc)
        {
            location = new codeLocation();
            location = loc;
            children = new List<CodeTree>();
        }
        public void AddChild(codeLocation loc) //Recursive Implementation [be careful!]
        {

            if (this.children.Count > 0)
            {
                if (this.children[0].location.wordRef == loc.wordRef)
                {
                    bool flag = false;
                    for (int i = 0; i < this.children.Count; i++)
                    {
                        if (loc.codeRef == this.children[i].location.codeRef)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        CodeTree child = new CodeTree(loc);
                        child.errorParam = errorParam;
                        this.children.Add(child);
                    }
                }
                else
                {
                    for (int i = 0; i < this.children.Count; i++)
                    {
                        this.children[i].errorParam = errorParam;
                        this.children[i].AddChild(loc);
                    }
                }
            }
            else
            {
                if (this.location.wordRef == loc.wordRef - 1)
                {
                    CodeTree child = new CodeTree(loc);
                    child.errorParam = errorParam;
                    children.Add(child);
                }
            }
        }
        public List<scanPath> findMeter(List<int> meters)
        {
            bool flag = false;
            List<int> indices = new List<int>();
            if (meters.Count == 0)
            {
                for (int i = 0; i < Meters.numMeters; i++)
                {
                    if (Meters.usage[i] == 1)
                    {
                        indices.Add(i);
                    }
                }
                for (int i = 0; i < Meters.numMeters; i++)
                {
                    if (Meters.usage[i] == 0)
                    {
                        indices.Add(i);
                    }
                }
                for (int i = Meters.numMeters; i < Meters.numMeters + Meters.numRubaiMeters; i++)
                {
                    indices.Add(i);
                }
            }
            else
            {
                if (meters[0] == -2)
                {
                    for (int i = Meters.numMeters; i < Meters.numMeters + Meters.numRubaiMeters; i++)
                    {
                        indices.Add(i);
                    }
                }
                else
                {
                    for (int i = 0; i < meters.Count; i++)
                    {
                        if (meters[i] != -1)
                        {
                            indices.Add(meters[i]);
                        }
                        else
                        {
                            flag = true;
                        }

                    }
                }
            }
           
            //indices.Add(27);
            //indices.Add(18);
            List<scanPath> mainList = new List<scanPath>();
            List<scanPath> a = new List<scanPath>();
            List<scanPath> b = new List<scanPath>();
            scanPath scn = new scanPath();
            scn.meters = indices;
            scn.location.Add(new codeLocation());


            if (TaqtiController.fuzzy)
            {
                mainList = traverseFuzzy(scn);
                //mainList = traverseFreeVerse(scn);
            }
            else if(TaqtiController.freeVerse)
            {
                mainList = traverseFreeVerse(scn);
            }
            else
            {
                mainList = traverse(scn);
                //mainList = traverseFreeVerse(scn);

                if (flag || (meters.Count == 0))
                {
                    List<string> codeList = new List<string>();
                    a = getCode(scn);
                    codeLocation locs = new codeLocation();
                    locs.code = "root";
                    locs.word = "";
                    locs.wordRef = -1;
                    locs.codeRef = -1;
                    for (int i = 0; i < a.Count; i++)
                    {
                        patternTree pTree = new patternTree(locs);
                        for (int j = 0; j < a[i].location.Count; j++)
                        {
                            for (int k = 0; k < a[i].location[j].code.Length; k++)
                            {
                                codeLocation locn = new codeLocation();
                                locn.codeRef = a[i].location[j].codeRef;
                                locn.word = a[i].location[j].word;
                                locn.wordRef = a[i].location[j].wordRef;
                                locn.code = a[i].location[j].code[k].ToString();
                                if (j == a[i].location.Count - 1)
                                {
                                    if (k == a[i].location[j].code.Length - 1)
                                        if (locn.code.Equals("x"))
                                        {
                                            locn.code = "=";
                                        }
                                }
                                pTree.AddChild(locn);
                            }
                        }
                        b = pTree.isMatch();
                        if (b.Count > 0)
                        {
                            b = compressList(b);
                            for (int n = 0; n < b.Count; n++)
                                mainList.Add(b[n]);
                        }
                    }
                }
            }
            return mainList;
        }
        private List<scanPath> compressList(List<scanPath> lst)
        {
            List<scanPath> list = new List<scanPath>();
            for (int i = 0; i < lst.Count; i++)
            {
                scanPath sc = new scanPath();
                sc.meters = lst[i].meters;
                int j;
                string code = "";
                for (j = 0; j < lst[i].location.Count - 1; j++)
                {
                    if (j == 0) //first redundant element
                    {
                        codeLocation L = new codeLocation();
                        L.codeRef = -1;
                        L.word = "root";
                        L.wordRef = -1;
                        code += "";
                        L.code = code;
                        code = "";
                        sc.location.Add(L);
                    }
                    int wordRef = lst[i].location[j].wordRef;
                    if (wordRef == lst[i].location[j + 1].wordRef)
                    {
                        code += lst[i].location[j].code;
                    }
                    else
                    {
                        codeLocation cL = new codeLocation();
                        cL.codeRef = lst[i].location[j].codeRef;
                        cL.word = lst[i].location[j].word;
                        cL.wordRef = lst[i].location[j].wordRef;
                        code += lst[i].location[j].code;
                        cL.code = code;
                        code = "";
                        sc.location.Add(cL);
                    }
                }

                int wordRef2 = lst[i].location[j - 1].wordRef;
                if (wordRef2 == lst[i].location[lst[i].location.Count - 1].wordRef)
                {
                    code += lst[i].location[j].code;
                }
                else
                {
                    code = lst[i].location[j].code;
                }

                codeLocation cL2 = new codeLocation();
                cL2.codeRef = lst[i].location[j].codeRef;
                cL2.word = lst[i].location[j].word;
                cL2.wordRef = lst[i].location[j].wordRef;
                cL2.code = code;

                sc.location.Add(cL2);
                list.Add(sc);
            }
            return list;

        }
        private List<int> checkCodeLength(string code, List<int> indices)
        {
            List<int> list = new List<int>();
            string meter = "";
            for (int i = 0; i < indices.Count; i++)
                list.Add(indices[i]);

            foreach (int i in indices)
            {
                if (i < Meters.numMeters)
                {
                    meter = Meters.meters[i].Replace("/", "");
                }
                else if (i < Meters.numMeters + Meters.numVariedMeters && i >= Meters.numMeters)
                {
                    meter = Meters.metersVaried[i - Meters.numMeters].Replace("/", "");
                }
                else if (i < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && i >= Meters.numMeters + Meters.numVariedMeters)
                {
                    meter = Meters.rubaiMeters[i - Meters.numMeters - Meters.numVariedMeters].Replace("/", "");
                }

                meter = meter.Replace("/", "");
                string meter2 = meter.Replace("+", "") + "-";
                string meter3 = meter.Replace("+", "-") + "-";
                string meter4 = meter.Replace("+", "-");
                meter = meter.Replace("+", "");


                bool flag1 = false, flag2 = false, flag3 = false, flag4 = false;


                #region original meter
                if (meter.Length == code.Length)
                {
                    for (int j = 0; j < meter.Length; j++)
                    {
                        char met = meter[j];
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
                #region Variation 1
                meter = meter2;
                if (meter.Length == code.Length)
                {
                    for (int j = 0; j < meter.Length; j++)
                    {
                        char met = meter[j];
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
                #region Variation 2
                meter = meter3;
                if (meter.Length == code.Length)
                {
                    for (int j = 0; j < meter.Length; j++)
                    {
                        char met = meter[j];
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
                #region Variation 3
                meter = meter4;
                if (meter.Length == code.Length)
                {
                    for (int j = 0; j < meter.Length; j++)
                    {
                        char met = meter[j];
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



                if (flag1 && flag2 && flag3 && flag4)
                    list.Remove(i);
            }
            return list;
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
                    if(i == m)
                    {

                    }
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
        private bool isMatch(string meter, string tentativeCode, string wordCode)
        {
            int i = 0;
            bool flag1 = true, flag2 = true, flag3 = true, flag4 = true;
            meter = meter.Replace("/", "");
            string meter2 = meter.Replace("+", "") + "-";
            string meter3 = meter.Replace("+", "-") + "-";
            string meter4 = meter.Replace("+", "-");


            if (tentativeCode.Length + wordCode.Length > 0)
            {

                //////////////// Caesura Detection  /////////////////////
                if (meter.Length > tentativeCode.Length + wordCode.Length)
                {
                    if (meter[tentativeCode.Length + wordCode.Length - 1] == '+')
                    {
                        if (wordCode.Length >= 2)
                        {
                            if (wordCode[wordCode.Length - 1] == '-')
                            {

                            }
                            else // word-boundary caesura violation
                            {
                                return false;
                            }
                        }
                        else if (wordCode.Length == 1)
                        {

                        }
                    }
                }
                string code = wordCode;
                meter = meter.Replace("+", "");
                #region original meter
                if (!(meter.Length < tentativeCode.Length + wordCode.Length))
                {

                    meter = meter.Substring(tentativeCode.Length);
                    while (i < code.Length)
                    {
                        char met = meter[i];



                        char cd = code[i];

                        ////////////////////////// Pattern matching  ///////////////////////

                        if (met == '-')
                        {
                            if (cd == '-' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag1 = false;
                                break;
                            }
                        }
                        else if (met == '=')
                        {
                            if (cd == '=' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag1 = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    flag1 = false;
                }
                #endregion


                #region Variation 1
                meter = meter2;
                i = 0;

                if (!(meter.Length < tentativeCode.Length + wordCode.Length))
                {
                    meter = meter.Substring(tentativeCode.Length);
                    while (i < code.Length)
                    {
                        char met = meter[i];



                        char cd = code[i];

                        if (i == code.Length - 1)
                        {
                            if (cd != '-')
                            {
                                flag2 = false;
                                break;
                            }
                        }

                        ////////////////////////// Pattern matching  ///////////////////////

                        if (met == '-')
                        {
                            if (cd == '-' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag2 = false;
                                break;
                            }
                        }
                        else if (met == '=')
                        {
                            if (cd == '=' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag2 = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    flag2 = false;
                }
                #endregion

                #region Variation 2
                meter = meter3;
                i = 0;

                if (!(meter.Length < tentativeCode.Length + wordCode.Length))
                {
                    meter = meter.Substring(tentativeCode.Length);
                    while (i < code.Length)
                    {
                        char met = meter[i];



                        char cd = code[i];

                        if (i == code.Length - 1)
                        {
                            if (cd != '-')
                            {
                                flag3 = false;
                                break;
                            }
                        }

                        ////////////////////////// Pattern matching  ///////////////////////

                        if (met == '-')
                        {
                            if (cd == '-' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag3 = false;
                                break;
                            }
                        }
                        else if (met == '=')
                        {
                            if (cd == '=' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag3 = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    flag3 = false;
                }
                #endregion

                #region Variation 3
                meter = meter4;
                i = 0;

                if (!(meter.Length < tentativeCode.Length + wordCode.Length))
                {
                    meter = meter.Substring(tentativeCode.Length);
                    while (i < code.Length)
                    {
                        char met = meter[i];



                        char cd = code[i];



                        ////////////////////////// Pattern matching  ///////////////////////

                        if (met == '-')
                        {
                            if (cd == '-' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        else if (met == '=')
                        {
                            if (cd == '=' || cd == 'x')
                            {
                                i = i + 1;
                            }
                            else
                            {
                                flag4 = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    flag4 = false;
                }
                #endregion

                if (flag1 || flag2 || flag3 || flag4)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        private List<scanPath> getCode(scanPath scn)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (this.children.Count > 0)
            {
                codeLocation wordCode = new codeLocation();
                for (int k = 0; k < this.children.Count; k++)
                {
                    scanPath scpath = new scanPath();
                    for (int i = 0; i < scn.location.Count; i++)
                        scpath.location.Add(scn.location[i]);

                    scpath.location.Add(this.children[k].location);
                    List<scanPath> temp;
                    temp = new List<scanPath>();
                    temp = this.children[k].getCode(scpath);

                    for (int i = 0; i < temp.Count; i++)
                        mainList.Add(temp[i]);
                    // scn.location.Add(this.children[k].location);
                }
            }
            else  //tree leaf
            {
                mainList.Add(scn);
            }
            return mainList;
        }
        private List<scanPath> traverse(scanPath scn)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (scn.meters.Count == 0)
            {
                return mainList;
            }
            else
            {
                if (this.children.Count > 0)
                {
                    string code = "";
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    List<int> ind = new List<int>();
                    for (int k = 0; k < this.children.Count; k++)
                    {
                        bool flag = false;
                        string tentativeCode = code;
                        string wordCode = this.children[k].location.code;
                        List<int> indices = new List<int>();
                        for (int i = 0; i < scn.meters.Count; i++)
                            indices.Add(scn.meters[i]);
                        int numIndices = scn.meters.Count;
                        for (int i = 0; i < numIndices; i++)
                        {
                            if (scn.meters[i] < Meters.numMeters)
                            {

                                if (!isMatch(Meters.meters[scn.meters[i]], tentativeCode, wordCode))  //remove meter indices that don't match
                                {

                                    indices.Remove(scn.meters[i]);
                                }
                                else
                                {
                                    flag = true;
                                }

                            }
                            else if (scn.meters[i] < Meters.numMeters + Meters.numVariedMeters && scn.meters[i] >= Meters.numMeters)
                            {
                                if (!isMatch(Meters.metersVaried[scn.meters[i] - Meters.numMeters], tentativeCode, wordCode))  //remove meter indices that donot match
                                {
                                    indices.Remove(scn.meters[i]);
                                }
                                else
                                {
                                    flag = true;
                                }


                            }
                            else if (scn.meters[i] < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && scn.meters[i] >= Meters.numMeters + Meters.numVariedMeters)
                            {
                                if (!isMatch(Meters.rubaiMeters[scn.meters[i] - Meters.numMeters - Meters.numVariedMeters], tentativeCode, wordCode))  //remove meter indices that don't match
                                {
                                    indices.Remove(scn.meters[i]);
                                }
                                else
                                {
                                    flag = true;
                                }
                            }

                        }

                        if (flag)
                        {
                            scanPath scpath = new scanPath();
                            scpath.meters = indices;
                            for (int i = 0; i < scn.location.Count; i++)
                                scpath.location.Add(scn.location[i]);

                            scpath.location.Add(this.children[k].location);
                            List<scanPath> temp;
                            temp = new List<scanPath>();
                            temp = this.children[k].traverse(scpath);

                            for (int i = 0; i < temp.Count; i++)
                                mainList.Add(temp[i]);
                        }
                        // return ind;
                    }

                    return mainList;
                }
                else   //Tree Leaf
                {
                    string code = "";
                    List<scanPath> sp = new List<scanPath>();
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    List<int> met = checkCodeLength(code, scn.meters);
                    if (met.Count != 0)
                    {
                        scn.meters = met;
                        sp.Add(scn);
                    }
                    return sp;
                }
            }

        }
        private List<int> checkCodeLengthFuzzy(string code, List<int> indices)
        {
            List<int> list = new List<int>();
            string meter = "";
            for (int i = 0; i < indices.Count; i++)
                list.Add(indices[i]);

            foreach (int i in indices)
            {
                if (i < Meters.numMeters)
                {
                    meter = Meters.meters[i].Replace("/", "");
                }
                else if (i < Meters.numMeters + Meters.numVariedMeters && i >= Meters.numMeters)
                {
                    meter = Meters.metersVaried[i - Meters.numMeters].Replace("/", "");
                }
                else if (i < Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters && i >= Meters.numMeters + Meters.numVariedMeters)
                {
                    meter = Meters.rubaiMeters[i - Meters.numMeters - Meters.numVariedMeters].Replace("/", "");
                }

                meter = meter.Replace("/", "");
                string meter2 = meter.Replace("+", "") + "~";
                string meter3 = meter.Replace("+", "~") + "~";
                string meter4 = meter.Replace("+", "~");
                meter = meter.Replace("+", "");
                int flag1 = 20, flag2 = 20, flag3 = 20, flag4 = 20;
                if (code.Length > 0)
                {
                    meter = meter.Replace("+", "");
                    int test = LevenshteinDistance(meter, code);
                    flag1 = LevenshteinDistance(meter, code);
                    flag2 = LevenshteinDistance(meter2, code);
                    flag3 = LevenshteinDistance(meter3, code);
                    flag4 = LevenshteinDistance(meter4, code);

                    if (Math.Min(flag4, min(flag1, flag2, flag3)) > errorParam)
                    {
                        list.Remove(i);
                    }
                }
                else
                    return list;

            }
            return list;
        }
        private List<scanPath> traverseFuzzy(scanPath scn)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (scn.meters.Count == 0)
            {
                return mainList;
            }
            else
            {
                if (this.children.Count > 0)
                {
                    string code = "";
                    int fuzz = 0;
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    for (int i = 0; i < scn.location.Count; i++)
                        fuzz = fuzz + scn.location[i].fuzzy;
                    List<int> ind = new List<int>();
                    for (int k = 0; k < this.children.Count; k++)
                    {
                        bool flag = false;
                        string tentativeCode = code;
                        string wordCode = this.children[k].location.code;
                        List<int> indices = new List<int>();
                        for (int i = 0; i < scn.meters.Count; i++)
                            indices.Add(scn.meters[i]);
                        int numIndices = scn.meters.Count;

                        scanPath scpath = new scanPath();
                        scpath.meters = indices;
                        for (int i = 0; i < scn.location.Count; i++)
                            scpath.location.Add(scn.location[i]);

                        scpath.location.Add(this.children[k].location);
                        List<scanPath> temp;
                        temp = new List<scanPath>();
                        temp = this.children[k].traverseFuzzy(scpath);

                        for (int i = 0; i < temp.Count; i++)
                            mainList.Add(temp[i]);

                        // return ind;
                    }
                    return mainList;
                }
                else   //Tree Leaf
                {
                    string code = "";
                    List<scanPath> sp = new List<scanPath>();
                    List<int> metList = new List<int>();
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    List<int> met = checkCodeLengthFuzzy(code, scn.meters);
                    if (met.Count != 0)
                    {
                        scn.meters = met;
                        sp.Add(scn);
                    }
                    return sp;
                }
            }
        }
        private List<int> checkMeterFreeVerse(string code, List<int> indices)
        {
            List<int> list = new List<int>();
            string meter = "";
            for (int i = 0; i < indices.Count; i++)
                list.Add(indices[i]);

            foreach (int i in indices)
            {
                List<string> feet = new List<string>();
                bool f = true;
                if (i < Meters.numMeters)
                {
                    meter = Meters.meters[i];
                }
                else if (i < Meters.numMeters + Meters.numVariedMeters && i >= Meters.numMeters)
                {
                    meter = Meters.metersVaried[i - Meters.numMeters];
                }

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
                        }
                        else
                        {
                            f = false;
                            break;
                        }
                    }

                    if (!f)
                    {
                        list.Remove(i);
                    }
                    else
                    {
                        int hurray;
                        hurray = 555;
                    }

                }
                else
                    return list;
            }
            return list;
        }
        private List<scanPath> traverseFreeVerse(scanPath scn)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (scn.meters.Count == 0)
            {
                return mainList;
            }
            else
            {
                if (this.children.Count > 0)
                {
                    string code = "";
                    int fuzz = 0;
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    for (int i = 0; i < scn.location.Count; i++)
                        fuzz = fuzz + scn.location[i].fuzzy;
                    List<int> ind = new List<int>();
                    for (int k = 0; k < this.children.Count; k++)
                    {
                        bool flag = false;
                        string tentativeCode = code;
                        string wordCode = this.children[k].location.code;
                        List<int> indices = new List<int>();
                        for (int i = 0; i < scn.meters.Count; i++)
                            indices.Add(scn.meters[i]);
                        int numIndices = scn.meters.Count;

                        scanPath scpath = new scanPath();
                        scpath.meters = indices;
                        for (int i = 0; i < scn.location.Count; i++)
                            scpath.location.Add(scn.location[i]);

                        scpath.location.Add(this.children[k].location);
                        List<scanPath> temp;
                        temp = new List<scanPath>();
                        temp = this.children[k].traverseFreeVerse(scpath);

                        for (int i = 0; i < temp.Count; i++)
                            mainList.Add(temp[i]);
                        // return ind;
                    }
                    return mainList;
                }
                else   //Tree Leaf
                {
                    string code = "";
                    List<scanPath> sp = new List<scanPath>();
                    for (int i = 0; i < scn.location.Count; i++)
                        code = code + scn.location[i].code;
                    List<int> met = checkMeterFreeVerse(code, scn.meters);
                    if (met.Count != 0)
                    {
                        scn.meters = met;
                        sp.Add(scn);
                    }
                    return sp;
                }
            }

        }
    }
}