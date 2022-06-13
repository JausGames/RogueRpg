using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerSettings : Settings
{
    static private LayerMask allLayer = Physics.AllLayers;
    static private LayerMask defaultLayer = 0;
    static private LayerMask hidable = 3;
    static private LayerMask door = 6;
    static private LayerMask ennemy = 7;
    static private LayerMask friend = 8;
    static private LayerMask player = 9;
    static private LayerMask projectile = 10;
    static private LayerMask bonus = 11;
    static private LayerMask floor = 12;
    static private LayerMask pickable = 13;
    static private LayerMask vfx = 14;
    static private LayerMask camera = 15;
    static private LayerMask hitbox = 16;

    public static LayerMask DefaultLayer { get => defaultLayer; }
    public static LayerMask Hidable { get => hidable; }
    public static LayerMask Door { get => door; }
    public static LayerMask Ennemy { get => ennemy; }
    public static LayerMask Friend { get => friend; }
    public static LayerMask Player { get => player; }
    public static LayerMask Projectile { get => projectile; }
    public static LayerMask Bonus { get => bonus; }
    public static LayerMask Floor { get => floor; }
    public static LayerMask Pickable { get => pickable; }
    public static LayerMask Camera { get => camera;  }
    public static LayerMask VFX { get => vfx;  }
    public static LayerMask Hitbox { get => hitbox;  }
    public static LayerMask AllLayer { get => allLayer;}

    public static LayerMask AddLayers(List<LayerMask> masks)
    {
        var result = new LayerMask();
        foreach(LayerMask mask in masks)
        {
            result |= (1 << mask);
        }
        return result;
    }
    public static LayerMask AddLayers(LayerMask mask1, LayerMask mask2)
    {
        var result = new LayerMask();
            result |= (1 << mask1);
            result |= (1 << mask2);
        return result;
    }
    public static LayerMask SubstractLayers(LayerMask baseMask, List<LayerMask> substracted)
    {
        var result = baseMask;
        foreach (LayerMask mask in substracted)
        {
            result &= ~(1 << mask);
        }
        return result;
    }
    public static LayerMask SubstractLayers(LayerMask baseMask, LayerMask substracted)
    {
        var result = baseMask;
        return result &= ~(1 << substracted);
    }
}
