using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSettings
{
    static List<Color> colors = new List<Color>() { 
        new Color(.3f, .5f, .7f), // Start
        new Color(.7f, .3f, .3f), // End
        new Color(.4f, .7f, .3f), // Gather
        Color.white,              // Default
        new Color(1f, .85f, .2f), // Bonus
        new Color(.8f, .55f, .4f) // Shop
    };

    static public Color RoomTypeToColor(Type roomType, SpecialRoom.Type specialType = SpecialRoom.Type.Null)
    {
        switch (roomType)
        {
            case Type.Start:
                return colors[0];
            case Type.End:
                return colors[1];
            case Type.Special:
                return SpecialRoomTypeToColor(specialType);
            case Type.Default:
                return colors[3];
            default:
                return colors[3];
        }
    }

    static public Color SpecialRoomTypeToColor(SpecialRoom.Type roomType)
    {
        switch (roomType)
        {
            case SpecialRoom.Type.Troup:
                return colors[2];
            case SpecialRoom.Type.Bonus:
                return colors[4];
            case SpecialRoom.Type.Shop:
                return colors[5];
            default:
                return colors[3];
        }
    }
}
