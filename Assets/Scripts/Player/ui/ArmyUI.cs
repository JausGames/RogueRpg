using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmyUI : MonoBehaviour
{
    [SerializeField] public Text tanksCount;
    [SerializeField] public Text warriorCount;
    [SerializeField] public Text rangeCount;
    [SerializeField] public Text mageCount;

    public void SetUI(List<Minion> army)
    {
        var warriors = new List<Minion>();
        var tanks = new List<Minion>();
        var rangers = new List<Minion>();
        var supports = new List<Minion>();

        for (int i = 0; i < army.Count; i++)
        {
            switch (army[i].MinionType)
            {
                case Minion.Type.Warrior:
                    warriors.Add(army[i]);
                    break;
                case Minion.Type.Tank:
                    tanks.Add(army[i]);
                    break;
                case Minion.Type.Range:
                    rangers.Add(army[i]);
                    break;
                case Minion.Type.Support:
                    supports.Add(army[i]);
                    break;
                default:
                    break;
            }
        }
        tanksCount.text = tanks.Count < 10 ? "0" + tanks.Count.ToString() : tanks.Count.ToString();
        warriorCount.text = warriors.Count < 10 ? "0" + warriors.Count.ToString() : warriors.Count.ToString();
        rangeCount.text = rangers.Count < 10 ? "0" + rangers.Count.ToString() : rangers.Count.ToString();
        mageCount.text = supports.Count < 10 ? "0" + supports.Count.ToString() : supports.Count.ToString();
    }
}
