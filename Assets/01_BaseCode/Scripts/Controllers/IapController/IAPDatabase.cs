using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Events;
using Newtonsoft.Json;
public enum TypePackIAP
{
    NoAdsCoinPack = 0,
    NoAdsHeartPack = 1,
    NoAdsPack = 2,
    BirdSkin_1_0_Pack = 3,
    BirdSkin_2_0_Pack = 4,
    BirdSkin_3_0_Pack = 5,
    BirdSkin_4_0_Pack = 6,
    BirdSkin_5_0_Pack = 7,
    BirdSkin_6_0_Pack = 8,
    BirdSkin_7_0_Pack = 9,
    BirdSkin_8_0_Pack = 10,
    BirdSkin_9_0_Pack = 11,
    BirdSkin_10_0_Pack = 12,
    BirdSkin_1_1_Pack = 13,
    BirdSkin_2_1_Pack = 14,
    BirdSkin_3_1_Pack = 15,
    BirdSkin_4_1_Pack = 16,
    BirdSkin_5_1_Pack = 17,
    BirdSkin_6_1_Pack = 18,
    BirdSkin_7_1_Pack = 19,
    BirdSkin_8_1_Pack = 20,
    BirdSkin_9_1_Pack = 21,
    BirdSkin_10_1_Pack = 22,
    DefaultBranchPack = 23,
    ParkBranchPack = 24,
    MoonlightBranchPack = 25,
    GreeceBranchPack = 26,
    AutumnBranchPack = 27,
    ChinaBranchPack = 28,
    NederlandPackPack = 29,
    SummerBranchPack = 30,
    SpringBranchPack = 31,
    WinterBranchPack = 32,
    DesertBranchPack = 33,
    DefaultThemePack = 34,
    AutumnThemePack = 35,
    ParkThemePack = 36,
    GreeceThemePack = 37,
    NederlandThemePack = 38,
    MoonlightThemePack = 39,
    DesertThemePack = 40,
    SpringThemePack = 41,
    SummerThemePack = 42,
    ChinaThemePack = 43,
    WinterThemePack = 44,
    StarterPack = 45,
    PremiumPack = 46,
    ThreeSkinBirdPack = 47,
    TwoBranchBackgroundPack = 48,
    DoubleSaleItemPack = 49,
    BigRemoveAdsPack = 50,
    SaleWeekend_1_Pack = 51,
    SaleWeekend_2_Pack = 52,
    SaleWeekend_3_Pack = 53,

    BirdSkinIronManPack = 54,
    BirdSkinCaptainPack = 55,
    BirdSkinDrStrangePack = 56,
    BirdSkinLokiPack = 57,
    BirdSkinBatmanPack = 58,
    BirdSkinAquaPack = 59,
    BirdSkinIAP_7_1_Pack = 60,
    BirdSkinIAP_8_1_Pack = 61,
    BirdSkinIAP_9_1_Pack = 62,
    BirdSkinIAP_10_1_Pack = 63,

    CyberpunkBranchPack = 64,
    AquaBranchPack = 65,
    DrStrangeBranchPack = 66,
    AsgardBranchPack = 67,
    BatmanBranchPack = 68,
    StarkTowerBranchPack = 69,

    CyberpunkThemePack = 70,
    AquaThemePack = 71,
    DrStrangeThemePack = 72,
    AsgardThemePack = 73,
    BatmanThemePack = 74,
    StarkTowerThemePack = 75,
    
    ConnectBirdMiniGame_Pack_1 = 76,
    ConnectBirdMiniGame_Pack_2 = 77,
    EventThemePack = 78,
    BranchSkin_11_Pack = 79,
    BirdSkin_2_2_Pack = 80,

}


[CreateAssetMenu(menuName = "ScriptableObject/IAPDatabase", fileName = "IAPDatabase.asset")]
public class IAPDatabase : SerializedScriptableObject
{
    public List<IAPPack> lstPacksInapp;
    public List<IAPPack> lstPacksNotInapp;

    public IAPPack GetPack(TypePackIAP type)
    {
        for (int i = 0; i < lstPacksInapp.Count; i++)
        {
            if (lstPacksInapp[i].type != type) continue;

            return lstPacksInapp[i];
        }

        return null;
    }


    public IAPPack GetPackNotInapp(TypePackIAP type)
    {
        for (int i = 0; i < lstPacksNotInapp.Count; i++)
        {
            if (lstPacksNotInapp[i].type != type) continue;

            return lstPacksNotInapp[i];
        }

        return null;
    }

    public IAPPack GetPackAll(TypePackIAP type)
    {
        var pack = GetPack(type);
        if (pack == null)
            pack = GetPackNotInapp(type);

        return pack;
    }
}

public class IAPPack
{
    private const string SALE = "sale";

    public string namePack;
    public TypePackIAP type;
    public ProductType productType;
    public TypeBuy typeBuy;
    [HideInInspector] public bool isNotInappPack { get { return typeBuy != TypeBuy.Inapp ? true : false; } }
    public string shortID;
    private UnityAction actClaimDone;
    public string ProductID
    {
        get
        {
            return string.Format("{0}.{1}", Config.package_name, shortID);
        }
    }
    public string ProductID_Origin
    {
        get
        {
            return string.Format("{0}.{1}.{2}", Config.package_name, shortID, SALE);
        }
    }

    //Đã mua hay chưa
    public bool IsBought
    {
        get
        {
            return PlayerPrefs.GetInt("Is_Buy_" + ProductID, 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("Is_Buy_" + ProductID, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public Dictionary<Item, int> itemsResult;//Các Item nhận được sau khi mua Pack
    //                Kiểu     Số lượng

    [HideIf("typeBuy", TypeBuy.Free)] public string defaultPrice;
    [ShowIf("isNotInappPack")][HideIf("typeBuy", TypeBuy.Free)] public int price;
    public string tittle;
    public Sprite icon;

    public bool isSale;
    [ShowIf("isSale", true)] public string idSale;
    [ShowIf("isSale", true)] public float percentSale;
    public void Claim(bool isIapInited = true)
    {
        //int value = 0;
        //Item typeItem = Item.Coin;

        //foreach (var item in itemsResult)
        //{
        //    switch (type)
        //    {
        //        case TypePackIAP.ConnectBirdMiniGame_Pack_1:
        //            //GamePlayController.Instance.miniGameController.connectBirdMG.ConnectBirdTurnBuy += item.Value;
        //            break;
        //        case TypePackIAP.ConnectBirdMiniGame_Pack_2:
        //            //GamePlayController.Instance.miniGameController.connectBirdMG.ConnectBirdTurnBuy += item.Value;
        //            break;
        //        case TypePackIAP.NoAdsCoinPack:
        //            break;
        //        case TypePackIAP.NoAdsHeartPack:

        //            break;
        //        case TypePackIAP.NoAdsPack:
        //            GameController.Instance.useProfile.IsRemoveAds = true;
        //            GameController.Instance.admobAds.DestroyBanner();
        //            EventDispatcher.EventDispatcher.Instance.PostEvent(EventID.REMOVE_ADS);
        //           // return;
        //        //case TypePackIAP.StarterPack:
        //        //    GameController.Instance.useProfile.IsRemoveAds = true;
        //            break;

        //        //case TypePackIAP.ThreeSkinBirdPack:
        //        //    GameController.Instance.useProfile.IsRemoveAds = true;
        //        //    break;

        //        //case TypePackIAP.BigRemoveAdsPack:
        //        //    GameController.Instance.useProfile.IsRemoveAds = true;
        //        //    break;

        //        //case TypePackIAP.SaleWeekend_1_Pack:
        //        //    GameController.Instance.useProfile.IsRemoveAds = true;
        //        //    break;
        //    }
        //}

        //if (typeBuy == TypeBuy.Video)
        //{
        //    List<GiftRewardShow> lstReward = new List<GiftRewardShow>();
        //    foreach (var item in itemsResult)
        //    {
        //        GameController.Instance.dataContain.giftDatabase.Claim(item.Key, item.Value);

        //        GiftRewardShow rw = new GiftRewardShow();
        //        rw.type = item.Key;
        //        rw.amount = item.Value;

        //        lstReward.Add(rw);
        //    }
        //    if (lstReward.Count <= 1)
        //    {
        //        RewardIAPBox.Setup2().Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //    }
        //    else
        //    {
        //        RewardIAPBox.Setup2(true).Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //    }

        //}
        //else
        //{
        //    List<GiftRewardShow> lstReward = new List<GiftRewardShow>();
        //    foreach (var item in itemsResult)
        //    {
        //        GameController.Instance.dataContain.giftDatabase.Claim(item.Key, item.Value);
        //    }
                
        //    if (typeBuy == TypeBuy.Inapp && productType == ProductType.NonConsumable)
        //    {
        //        if (isIapInited)
        //        {
        //            if (!IsBought)
        //            {
        //                if (lstReward.Count <= 1)
        //                {
        //                    RewardIAPBox.Setup2().Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //                }
        //                else
        //                {
        //                    RewardIAPBox.Setup2(true).Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (lstReward.Count <= 1)
        //        {
        //            RewardIAPBox.Setup2().Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //        }
        //        else
        //        {
        //            RewardIAPBox.Setup2(true).Show(lstReward, actionClaim: () => { actClaimDone?.Invoke(); });
        //        }
        //    }    
        
        //    IsBought = true;
        //}
    }

    public UnityAction ActClaimDone
    {
        set
        {
            actClaimDone = value;
        }
    }

    public int GetAmount(Item itmName)
    {
        int amount = 0;
        if (itemsResult.TryGetValue(itmName, out amount))
        {
            return amount;
        }

        return amount;
    }
}