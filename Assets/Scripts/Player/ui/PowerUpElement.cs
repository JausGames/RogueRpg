using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpElement : MonoBehaviour
{
    [SerializeField] Text bonusName;
    [SerializeField] Text bonusValue;

    public Text BonusName { get => bonusName; set => bonusName = value; }
    public Text BonusValue { get => bonusValue; set => bonusValue = value; }
}
