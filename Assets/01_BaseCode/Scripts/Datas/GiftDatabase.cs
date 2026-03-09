using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(menuName = "Datas/GiftDatabase", fileName = "GiftDatabase.asset")]
public class GiftDatabase : SerializedScriptableObject
{
    public Dictionary<Item, Gift> giftList;

    public bool GetGift(Item item, out Gift gift)
    {
        return giftList.TryGetValue(item, out gift);
    }

    public Sprite GetIconItem(Item item)
    {
        Gift gift = null;
        //if (IsCharacter(giftType))
        //{
        //    var Char = GameController.Instance.dataContain.dataSkins.GetSkinInfo(giftType);
        //    if (Char != null)
        //        return Char.iconSkin;
        //}
        bool isGetGift = GetGift(item, out gift);
        return isGetGift ? gift.getGiftSprite : null;
    }
    public GameObject GetAnimItem(Item item)
    {
        Gift gift = null;
        bool isGetGift = GetGift(item, out gift);
        return isGetGift ? gift.getGiftAnim : null;
    }

    // public void Claim(GiftType giftType, int amount, Reason reason = Reason.none)
    // {
    //     switch (giftType)
    //     {
    //         case GiftType.Dial:
    //             GameData.Dial += amount;
    //             break;
    //         case GiftType.Find:
    //             GameData.Find += amount;
    //             break;
    //         case GiftType.Key:
    //             GameData.Key += amount;
    //             break;
    //         case GiftType.RemoveAds:
    //             //UseProfile.IsRemoveAds = true;
    //             //GameController.Instance.admobAds.DestroyBanner();
    //             break;
    //
    //     }
    // }
   
    /*private void ClaimBranch(int id)
    {
        var oldBranchData = JsonConvert.DeserializeObject<List<int>>(GameController.Instance.useProfile.OwnedBranchSkin);
        if (oldBranchData == null)
        {
            oldBranchData = new List<int>();
        }
        if (!oldBranchData.Contains(id))
        {
            oldBranchData.Add(id);
            GameController.Instance.useProfile.CurrentBranchSkin = id;
        }
        GameController.Instance.useProfile.OwnedBranchSkin = JsonConvert.SerializeObject(oldBranchData);
    }*/
   
    public static bool IsCharacter(Item item)
    {
       
        return false;
    }
}

public class Gift
{
    [SerializeField] private Sprite giftSprite;
    [SerializeField] private GameObject giftAnim;
    public virtual Sprite getGiftSprite => giftSprite;
    public virtual GameObject getGiftAnim => giftAnim;

}

public enum Item
{
    None = 0,
    Coin = 1,
    Hint = 2,
    FreezeTime = 3,
}


public enum Reason
{
    none = 0,
    play_with_item = 1,
    watch_video_claim_item_main_home = 2,
    daily_login = 3,
    lucky_spin = 4,
    unlock_skin_in_special_gift = 5,
    reward_accumulate = 6,
}

[Serializable]
public class RewardRandom
{
    public int id;
    public Item typeItem;
    public int amount;
    public int weight;

    public RewardRandom()
    {
    }
    public RewardRandom(Item item, int amount, int weight = 0)
    {
        this.typeItem = item;
        this.amount = amount;
        this.weight = weight;
    }

    public GiftRewardShow GetReward()
    {
        GiftRewardShow rew = new GiftRewardShow();
        rew.type = typeItem;
        rew.amount = amount;

        return rew;
    }
}
