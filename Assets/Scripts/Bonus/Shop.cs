using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Interactable
{
    [SerializeField] List<Factory> factories;
    [SerializeField] List<int> prices;

    private void Start()
    {
        SetUpShop();
    }
    virtual public void SetUpShop()
    {
        foreach (Factory factory in factories)
        {
            factory.OnInteract(null);
        }
    }

    internal int GetPrice(Factory factory)
    {
        int price = -0;
        for(int i = 0; i < factories.Count; i++)
        {
            if (factories[i] == factory) return price = prices[i];
        }
        return price;
    }

    public override void OnInteract(Hitable player)
    {
    }
}
