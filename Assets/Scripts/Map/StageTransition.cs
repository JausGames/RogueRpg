using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageTransition : Interactable
{
    UnityEvent onTransitionStart = new UnityEvent();

    private void Awake()
    {
        var generator = FindObjectOfType<DungeonGenerator>();
        onTransitionStart.AddListener(generator.GenerateNextStage);
    }
    public override void OnInteract(Hitable player)
    {
        onTransitionStart.Invoke();
        
        var rplayer = (Player)player;
        rplayer.ResetForNewStage();
    }
}
