using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruuz.Models
{
    public class StateMachine
    {
        static private readonly Dictionary<string, int[]> hindiMeterTransition = new Dictionary<string, int[]>
        {
            {"-", new int[] {2, 4, -1, 0, 5, -1, 7, -1}},
            {"=", new int[] {1, 0, 3, -1, 6, 1, -1, 0}}
        };

        static private readonly Dictionary<string, int[]> zamzamaMeterTransition = new Dictionary<string, int[]>
        {
            {"-", new int[] {1, 2, -1, -1}},
            {"=", new int[] {3, -1, 0, 0}}
        };

        static private readonly Dictionary<string, int[]> originalHindiMeterTransition = new Dictionary<string, int[]>
        {
            {"-", new int[] {-1, 2, 3, -1}},
            {"=", new int[] {1, 0, -1, 1}}
        };

        static private int NextState(Dictionary<string, int[]> transition, string input, int state)
        {
            try
            {
               return transition[input][state];
            }
            catch
            {
                return -1;
            }
        }

        static public int HindiMeter(string input, int state)
        {
            return NextState(hindiMeterTransition, input, state);
        }

        static public int ZamzamaMeter(string input, int state)
        {
            return NextState(zamzamaMeterTransition, input, state);
        }

        static public int OriginalHindiMeter(string input, int state)
        {
            return NextState(originalHindiMeterTransition, input, state);
        }
    }
}
