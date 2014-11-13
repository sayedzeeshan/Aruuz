using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruuz.Models
{
    public class patternTree
    {
        codeLocation location;
        List<patternTree> children;
        public patternTree(codeLocation loc)
        {
            location = new codeLocation();
            location = loc;
            children = new List<patternTree>();
        }
        public void AddChild(codeLocation loc) //Recursive Implementation [be careful!]
        {

            if (this.children.Count == 0)
            {
                if (loc.code.Equals("x"))
                {
                    codeLocation locLeft = new codeLocation();
                    locLeft.code = "-";
                    locLeft.codeRef = loc.codeRef;
                    locLeft.wordRef = loc.wordRef;
                    locLeft.word = loc.word;
                    patternTree childLeft = new patternTree(locLeft);
                    children.Add(childLeft);
                    codeLocation locRight = new codeLocation();
                    locRight.code = "=";
                    locRight.codeRef = loc.codeRef;
                    locRight.wordRef = loc.wordRef;
                    locRight.word = loc.word;
                    patternTree childRight = new patternTree(locRight);
                    children.Add(childRight);
                }
                else
                {
                    patternTree child = new patternTree(loc);
                    children.Add(child);
                }
            }
            else
            {
                for (int i = 0; i < this.children.Count; i++)
                    this.children[i].AddChild(loc);
            }
        }
        public List<scanPath> isMatch()
        {
            scanPath scn = new scanPath();
            List<scanPath> b = new List<scanPath>();
            List<scanPath> a = new List<scanPath>();
            a = traverseOriginalHindi(scn, 0);
            if (a.Count > 0)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    b.Add(a[i]);
                }
            }

            scanPath scn3 = new scanPath();
            List<scanPath> a3 = new List<scanPath>();

            a3 = traverseZamzama(scn3, 0);
            if (a3.Count > 0)
            {
                for (int i = 0; i < a3.Count; i++)
                {
                    b.Add(a3[i]);
                }
            }
            
            /*scanPath scn2 = new scanPath();
            List<scanPath> a2 = new List<scanPath>();

            a2 = traverseHindi(scn2, 0);
            if ((a2.Count > 0) && (a.Count == 0) && (a3.Count == 0))
            {
                for (int i = 0; i < a2.Count; i++)
                {
                    b.Add(a2[i]);
                }
            }
            */

            return b;
        }
        private List<scanPath> traverseZamzama(scanPath scn, int state)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (this.children.Count > 0)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    int localstate = StateMachine.ZamzamaMeter(this.children[i].location.code, state);
                    if (localstate != -1)
                    {
                        scanPath scpath = new scanPath();
                        for (int j = 0; j < scn.location.Count; j++)
                            scpath.location.Add(scn.location[j]);

                        scpath.location.Add(this.children[i].location);
                        List<scanPath> temp;
                        temp = new List<scanPath>();
                        temp = children[i].traverseZamzama(scpath, localstate);
                        for (int j = 0; j < temp.Count; j++)
                            mainList.Add(temp[j]);
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = 0; i < scn.location.Count; i++)
                {
                    if (scn.location[i].code.Equals("="))
                    {
                        count += 2;
                    }
                    else if (scn.location[i].code.Equals("-"))
                    {
                        count += 1;
                    }
                }
                if (count == 32)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 7);
                        mainList.Add(scn);
                    }
                }
                else if (count == 33)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 7);
                        mainList.Add(scn);
                    }
                }
                else if (count == 24)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 8);
                        mainList.Add(scn);
                    }
                }
                else if (count == 25)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 8);
                        mainList.Add(scn);
                    }
                }
                else if (count == 16)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 9);
                        mainList.Add(scn);
                    }
                }
                else if (count == 17)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 9);
                        mainList.Add(scn);
                    }
                }
            }
            return mainList;
        }
        private List<scanPath> traverseHindi(scanPath scn, int state)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (this.children.Count > 0)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    int localstate = StateMachine.HindiMeter(this.children[i].location.code, state);
                    if (localstate != -1)
                    {
                        scanPath scpath = new scanPath();
                        for (int j = 0; j < scn.location.Count; j++)
                            scpath.location.Add(scn.location[j]);

                        scpath.location.Add(this.children[i].location);
                        List<scanPath> temp;
                        temp = new List<scanPath>();
                        temp = children[i].traverseHindi(scpath, localstate);
                        for (int j = 0; j < temp.Count; j++)
                            mainList.Add(temp[j]);
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = 0; i < scn.location.Count; i++)
                {
                    if (scn.location[i].code.Equals("="))
                    {
                        count += 2;
                    }
                    else if (scn.location[i].code.Equals("-"))
                    {
                        count += 1;
                    }
                }
                if (count == 30)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters);
                        mainList.Add(scn);
                    }
                }
                else if (count == 31)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters);
                        mainList.Add(scn);
                    }
                }
                else if (count == 22)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 1);
                        mainList.Add(scn);
                    }
                }
                else if (count == 23)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 1);
                        mainList.Add(scn);
                    }
                }
                else if (count == 32)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 2);
                        mainList.Add(scn);
                    }
                }
                else if (count == 33)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 2);
                        mainList.Add(scn);
                    }
                }
                else if (count == 14)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 3);
                        mainList.Add(scn);
                    }
                }
                else if (count == 15)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 3);
                        mainList.Add(scn);
                    }
                }

            }
            return mainList;
        }
        private List<scanPath> traverseOriginalHindi(scanPath scn, int state)
        {
            List<scanPath> mainList = new List<scanPath>();
            if (this.children.Count > 0)
            {
                for (int i = 0; i < this.children.Count; i++)
                {
                    int localstate = StateMachine.OriginalHindiMeter(this.children[i].location.code, state);
                    if (localstate != -1)
                    {
                        scanPath scpath = new scanPath();
                        for (int j = 0; j < scn.location.Count; j++)
                            scpath.location.Add(scn.location[j]);

                        scpath.location.Add(this.children[i].location);
                        List<scanPath> temp;
                        temp = new List<scanPath>();
                        temp = children[i].traverseOriginalHindi(scpath, localstate);
                        for (int j = 0; j < temp.Count; j++)
                            mainList.Add(temp[j]);
                    }
                }
            }
            else
            {
                int count = 0;
                for (int i = 0; i < scn.location.Count; i++)
                {
                    if (scn.location[i].code.Equals("="))
                    {
                        count += 2;
                    }
                    else if (scn.location[i].code.Equals("-"))
                    {
                        count += 1;
                    }
                }
                if (count == 30)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters);
                        mainList.Add(scn);
                    }
                }
                else if (count == 31)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters);
                        mainList.Add(scn);
                    }
                }
                else if (count == 22)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 1);
                        mainList.Add(scn);
                    }
                }
                else if (count == 23)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 1);
                        mainList.Add(scn);
                    }
                }
                else if (count == 32)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 2);
                        mainList.Add(scn);
                    }
                }
                else if (count == 33)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 2);
                        mainList.Add(scn);
                    }
                }
                else if (count == 14)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 3);
                        mainList.Add(scn);
                    }
                }
                else if (count == 15)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 3);
                        mainList.Add(scn);
                    }
                }
                else if (count == 16)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 4);
                        mainList.Add(scn);
                    }
                }
                else if (count == 17)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 4);
                        mainList.Add(scn);
                    }
                }
                else if (count == 10)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 5);
                        mainList.Add(scn);
                    }
                }
                else if (count == 11)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 5);
                        mainList.Add(scn);
                    }
                }
                else if (count == 24)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 6);
                        mainList.Add(scn);
                    }
                }
                else if (count == 25)
                {
                    if (scn.location[scn.location.Count - 1].code.Equals("-") && scn.location[scn.location.Count - 2].code.Equals("="))
                    {
                        scn.meters.Add(Meters.numMeters + Meters.numVariedMeters + Meters.numRubaiMeters + 6);
                        mainList.Add(scn);
                    }
                }
            }
            return mainList;
        }
    }
}