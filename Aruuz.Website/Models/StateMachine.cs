using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aruuz.Models
{
    public class StateMachine
    {
        static public int HindiMeter(string input, int state)
        {
            int nextState = -1;
            if (input.Equals("-"))
            {
                if (state == 0)
                {
                    state = 2;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = 4;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = 0;
                    nextState = state;
                }
                else if (state == 4)
                {
                    state = 5;
                    nextState = state;
                }
                else if (state == 5)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 6)
                {
                    state = 7;
                    nextState = state;
                }
                else if (state == 7)
                {
                    state = -1;
                    nextState = state;
                }
            }
            else if (input.Equals("="))
            {
                if (state == 0)
                {
                    state = 1;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = 0;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = 3;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 4)
                {
                    state = 6;
                    nextState = state;
                }
                else if (state == 5)
                {
                    state = 1;
                    nextState = state;
                }
                else if (state == 6)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 7)
                {
                    state = 0;
                    nextState = state;
                }
            }
            return nextState;
        }

        static public int ZamzamaMeter(string input, int state)
        {
            int nextState = -1;
            if (input.Equals("-"))
            {
                if (state == 0)
                {
                    state = 1;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = 2;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = -1;
                    nextState = state;
                }
            }
            else if (input.Equals("="))
            {
                if (state == 0)
                {
                    state = 3;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = 0;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = 0;
                    nextState = state;
                }
            }
            return nextState;
        }

        static public int OriginalHindiMeter(string input, int state)
        {
            int nextState = -1;
            if (input.Equals("-"))
            {
                if (state == 0)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = 2;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = 3;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = -1;
                    nextState = state;
                }
            }
            else if (input.Equals("="))
            {
                if (state == 0)
                {
                    state = 1;
                    nextState = state;
                }
                else if (state == 1)
                {
                    state = 0;
                    nextState = state;
                }
                else if (state == 2)
                {
                    state = -1;
                    nextState = state;
                }
                else if (state == 3)
                {
                    state = 1;
                    nextState = state;
                }
            }
            return nextState;

        }
    }
}