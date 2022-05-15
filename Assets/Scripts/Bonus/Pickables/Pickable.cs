using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Pickable : ScriptableObject
{
    GameObject container = null;
    [SerializeField] Sprite sprite;
    [SerializeField] Texture texture;
    [SerializeField] float rate = .2f;

    public float Rate { get => rate; set => rate = value; }
    public GameObject Container { get => container; set => container = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public Texture Texture { get => texture; set => texture = value; }

    virtual public void AddToPlayer(Player player)
    {
        Destroy(container);
    }
}
