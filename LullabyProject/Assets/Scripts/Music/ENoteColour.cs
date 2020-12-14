
using UnityEngine;

namespace Music
{
    public enum ENoteColour
    {
        eBlue   = 0,
        eGreen  = 1,
        eYellow = 2,
        eOrange = 3,
        eRed    = 4
    }

    public static class NoteColours
    {
        public static int GetNumber()
        {
            return System.Enum.GetValues(typeof(ENoteColour)).Length; 
        }

        public static Color GetColour(ENoteColour noteColour)
        {
            Color colour = Color.clear;
            switch (noteColour)
            {
                case ENoteColour.eBlue:   colour = Color.blue;
                    break;
                case ENoteColour.eGreen:  colour = Color.green;
                    break;
                case ENoteColour.eYellow: colour = Color.yellow;
                    break;
                case ENoteColour.eOrange: colour = Color.Lerp(Color.red, Color.yellow, 0.5f);
                    break;
                case ENoteColour.eRed:    colour = Color.red;
                    break;
            }
            return colour;
        }
        
    }
}