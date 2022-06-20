using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapSettings
{
    static Dictionary<SpecialRoom.Type, Color> colorsDict = new Dictionary<SpecialRoom.Type, Color>(){
        {SpecialRoom.Type.PowerUp, new Color(0.78f, 0.94f, 1.00f)},
        {SpecialRoom.Type.Shop, new Color(.8f, .55f, .4f) },
        {SpecialRoom.Type.Heal, new Color(1.00f, 0.62f, 0.93f)},
        {SpecialRoom.Type.Smith, new Color(0.67f, 0.67f, 0.79f)},
        {SpecialRoom.Type.LevelUp, new Color(1f, .85f, .2f)},
        {SpecialRoom.Type.Null, Color.white},
        };

    static Dictionary<Type, Color> roomColorDict = new Dictionary<Type, Color>(){
        {Type.Start, new Color(.3f, .5f, .7f)},
        {Type.End, new Color(.7f, .3f, .3f) },
        {Type.Default, Color.white}
        };


    static public Color RoomTypeToColor(Type roomType, SpecialRoom.Type specialType = SpecialRoom.Type.Null)
    {
        if (specialType != SpecialRoom.Type.Null) return colorsDict[specialType];
        else return roomColorDict[roomType];
    }
}
