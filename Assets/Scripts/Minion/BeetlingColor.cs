using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class BeetlingColor
{
    static Dictionary<Minion.Type, Color> typeToColor = new Dictionary<Minion.Type, Color>()
    {
        [Minion.Type.Tank] = new Color(.5f, .5f, .5f),
        [Minion.Type.Warrior] = new Color(1f, 0f, .3f),
        [Minion.Type.Range] = new Color(.78f, .68f, .3f),
        [Minion.Type.Support] = new Color(.48f, 0f, .65f)
    };
    static List<Color> leafColors = new List<Color>()
    { 
        new Color(0.048f, 1f, 0f),
        new Color(1f, 0.048f, 0f)
    };

    public static Dictionary<Minion.Type, Color> TypeToColor { get => typeToColor; }
    public static List<Color> LeafColors { get => leafColors; set => leafColors = value; }
}
