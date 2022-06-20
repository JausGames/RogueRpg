using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpUi : MonoBehaviour
{
    [SerializeField] Transform container;

    public void ShowBonus(string bonusName, float bonusValue)
    {

        var GO = Instantiate(Resources.Load<GameObject>("Prefabs/Ui/PowerUp_Element"), Vector3.zero, Quaternion.identity, container);
        var element = GO.GetComponent<PowerUpElement>();
        var signStr = bonusValue > 0 ? "+ " : "- ";
        var color = bonusValue > 0 ? Color.green : Color.red;
        element.BonusName.color = color;
        element.BonusValue.color = color;
        element.BonusName.text = bonusName;
        element.BonusValue.text = signStr + Mathf.Abs(bonusValue).ToString();

        Destroy(GO, 2f);
    }
}
