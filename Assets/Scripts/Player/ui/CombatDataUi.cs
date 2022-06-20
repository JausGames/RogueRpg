using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatDataUi : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected Text strength;
    [SerializeField] protected Text agility; 
    [SerializeField] protected Text speed; 
    [SerializeField] protected Text physicArmor;
    [SerializeField] protected Text maxHealth;


    [Header("Stats")]
    [SerializeField] GameObject bonusSpritePrefab;
    [SerializeField] List<GameObject> bonusList;
    [SerializeField] RectTransform bonusRect;

    public void UpdateStatUi(CombatData data, Bonus newBonus)
    {
        strength.text = (Mathf.Ceil(data.Strength * 10f) * 0.1f) .ToString();
        speed.text = (Mathf.Ceil(data.Speed * 10f) * 0.1f).ToString();
        agility.text = (Mathf.Ceil(data.Agility * 10f) * 0.1f).ToString();
        physicArmor.text = (Mathf.Ceil(data.PhysicArmor * 10f) * 0.1f).ToString();
        maxHealth.text = (Mathf.Ceil(data.MAX_HEALTH * 10f) * 0.1f).ToString();

        var icon = Instantiate(bonusSpritePrefab, bonusRect);
        var rect = icon.GetComponent<RectTransform>();
        var x = bonusList.Count % 2 == 0 ? 0f : 50f;
        var y = Mathf.Floor(bonusList.Count / 2f) * -50f;
        rect.localPosition = x * Vector3.right + y * Vector3.up;
        var img = icon.GetComponent<Image>();
        img.sprite = newBonus.Sprite;
        bonusList.Add(icon);

    }
    internal void SetUpStat(CombatData data)
    {
        strength.text = (Mathf.Ceil(data.Strength * 10f) * 0.1f).ToString();
        speed.text = (Mathf.Ceil(data.Speed * 10f) * 0.1f).ToString();
        agility.text = (Mathf.Ceil(data.Agility * 10f) * 0.1f).ToString();
        physicArmor.text = (Mathf.Ceil(data.PhysicArmor * 10f) * 0.1f).ToString();
        maxHealth.text = (Mathf.Ceil(data.MAX_HEALTH * 10f) * 0.1f).ToString();


        foreach(GameObject go in bonusList)
        {
            Destroy(go);
        }
        bonusList.Clear();

        for(int i = 0; i < data.BonusList.Count; i++)
        {
            var icon = Instantiate(bonusSpritePrefab, bonusRect);
            var rect = icon.GetComponent<RectTransform>();
            var x = i % 2 == 0 ? 0f : 50f;
            var y = Mathf.Floor(i / 2f) * -50f;
            rect.localPosition = x * Vector3.right + y * Vector3.up;
            var img = icon.GetComponent<Image>();
            img.sprite = data.BonusList[i].Sprite;
            bonusList.Add(icon);
        }
    }
}
