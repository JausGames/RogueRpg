using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusFactory : Factory
{
    [SerializeField] List<Bonus> bonusList = new List<Bonus>();
    [SerializeField] Bonus bonus;
    [SerializeField] ParticleSystem prtc;
    [SerializeField] Text bonusName;
    [SerializeField] Text costTxt;
    [SerializeField] Image coinImage;
    [SerializeField] bool open = false;

    public bool Open { get => open; set => open = value; }

    override public void OnInteract(Hitable player)
    {
        if (!open) OpenChest();
        else
        {
            var buyable = true;
            if(shop)
            {
                Player pl = (Player)player;
                buyable = pl.Wallet.RemoveMoney(bonus.Price);
                //Destroy(shop.gameObject, 0.1f);
            }
            if (!buyable) return;
            player.AddBonus(bonus);
            
            Destroy(this.gameObject);
        }
    }
    private void OpenChest()
    {
        var rnd = Random.Range(0, bonusList.Count);
        bonusName.gameObject.SetActive(true);
        bonusName.text = bonusList[rnd].name;

        bonus = Instantiate(bonusList[rnd], transform);
        bonus.name = bonusName.text;

        var prtcRenderer = prtc.GetComponent<ParticleSystemRenderer>();
        var mat = new Material(prtcRenderer.material);
        mat.mainTexture = bonus.Texture;
        prtcRenderer.material = mat;
        open = true;
        if(shop)
        {
            coinImage.gameObject.SetActive(true);
            costTxt.gameObject.SetActive(true);
            costTxt.text = bonus.Price.ToString();
        }
    }

}
