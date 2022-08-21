using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Army : MonoBehaviour
{
    Player owner;
    [SerializeField] List<Minion> minions = new List<Minion>();
    [SerializeField] float sideOffset = 0.35f;
    [SerializeField] private float upwardOffset = 0.35f;
    //[SerializeField]  private int nbByLine;
    [SerializeField]  private AnimationCurve nbByLineCurve;
    [SerializeField]  private ArmyUI ui;

    private void Start()
    {
        owner = GetComponent<Player>();
        /*foreach(Minion min in minions)
        {
            min.dieEvent.AddListener(delegate { minions.Remove(min); ui.SetUI(minions); });
        }*/
        //ui.SetUI(minions);
    }

    public void SetMinionsPosition(Vector3 position, Vector3 direction)
    {
        //Debug.Log("Army, SetMinionsPositions : direction = " + direction);
        var count = minions.Count;
        var warriors = new List<Minion>();
        var tanks = new List<Minion>();
        var rangers = new List<Minion>();
        var supports = new List<Minion>();

        for (int i = 0; i < minions.Count; i++)
        {
            /*switch (minions[i].MinionType)
            {
                case Minion.Type.Warrior:
                    warriors.Add(minions[i]);
                    break;
                case Minion.Type.Tank:
                    tanks.Add(minions[i]);
                    break;
                case Minion.Type.Range:
                    rangers.Add(minions[i]);
                    break;
                case Minion.Type.Support:
                    supports.Add(minions[i]);
                    break;
                default:
                    break;
            }*/
        }
        
        var lastI = 0f;
        for (int i = 0; i < warriors.Count; i++)
        {
            if (!warriors[i].Fighting)
            {
                var nbByLine = NbMinionByLine(warriors.Count);
                lastI = upwardOffset * Mathf.Ceil(1 + (i / nbByLine));
                var lastPos = GetLinePosition(i, nbByLine) * owner.transform.right + lastI * owner.transform.forward + position;
                warriors[i].SetPosition(lastPos);
                warriors[i].SetRotation(direction);
            }
        }
        var tanksBasePos = lastI * owner.transform.forward;

        for (int i = 0; i < tanks.Count; i++)
        {
            if (!tanks[i].Fighting)
            {
                var nbByLine = NbMinionByLine(tanks.Count);
                lastI = upwardOffset * Mathf.Ceil(1 + (i / nbByLine));
                var lastPos = GetLinePosition(i, nbByLine) * owner.transform.right + lastI * owner.transform.forward + position;
                tanks[i].SetPosition(tanksBasePos + lastPos);
                tanks[i].SetRotation(direction);
            }
        }

        for (int i = 0; i < rangers.Count; i++)
        {
            if (!rangers[i].Fighting)
            {
                var nbByLine = NbMinionByLine(rangers.Count);
                lastI = upwardOffset * Mathf.Ceil(1 + (i / nbByLine));
                var lastPos = GetLinePosition(i, nbByLine) * owner.transform.right - lastI * owner.transform.forward + position;
                rangers[i].SetPosition(lastPos);
                rangers[i].SetRotation(direction);
            }
        }
        var supportsBasePos = -lastI * owner.transform.forward;

        for (int i = 0; i < supports.Count; i++)
        {
            if (!supports[i].Fighting)
            {
                var nbByLine = NbMinionByLine(supports.Count);
                lastI = upwardOffset * Mathf.Ceil(1 + (i / nbByLine));
                var lastPos = GetLinePosition(i, nbByLine) * owner.transform.right - lastI * owner.transform.forward + position;
                supports[i].SetPosition(supportsBasePos + lastPos);
                supports[i].SetRotation(direction);
            }
        }
        
        /*for (int i = 0; i < count; i++)
        {
            if (!minions[i].Fighting)
            {
                var lastI = upwardOffset * Mathf.Ceil(1 + (i / NbMinionByLine(minions.Count)));
                minions[i].SetPosition(GetLinePosition(i, NbMinionByLine(minions.Count)) * owner.transform.right + lastI * owner.transform.forward + position);
                minions[i].SetRotation(direction);
            }
        }*/

    }

    internal void ResetToOrigin()
    {
        for(int i = 0; i < minions.Count; i++)
        {
            minions[i].transform.position = Vector3.zero;
        }
        SetMinionsPosition(Vector3.zero, Vector3.forward);
    }

    float GetLinePosition(int nb, int nbByLine)
    {
        var sign = Mathf.FloorToInt((float)nb / (float)nbByLine) % 2 == 1 ? -1 : 1;
            
        var leftOrRight = nb % nbByLine == 0 && nbByLine % 2 == 1 ? 0 : (nb % 2 == 0 ? -1f : 1f) * sign;
        var posInLine = sideOffset * ((Mathf.Floor(nb / 2)) % (Mathf.Floor((float)nbByLine / 2f)) + 1f);
        posInLine = nbByLine % 2 == 0 ? posInLine - 0.5f * sideOffset : posInLine;

        return posInLine * leftOrRight;

    }

    internal void AddMinion(Minion minion)
    {
        minions.Add(minion);
        /*ui.SetUI(minions);
        minion.dieEvent.AddListener(delegate { minions.Remove(minion); ui.SetUI(minions); });*/
    }

    int NbMinionByLine(int nb)
    {
        var nbByLine = (int)Mathf.Floor(nbByLineCurve.Evaluate(nb));
        //nbByLine = Mathf.Clamp(3, (int)Mathf.Ceil(nbTotal / 4), 9);

        return nbByLine;
    }
}

