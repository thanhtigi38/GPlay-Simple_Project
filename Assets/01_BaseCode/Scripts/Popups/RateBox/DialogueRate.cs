using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class DialogueRate : BaseBox
{

    private static DialogueRate instance;
    public static DialogueRate Setup()
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<DialogueRate>(PathPrefabs.RATE_GAME_BOX));
            instance.Init();
        }
        //ChickenDataManager.CountTillShowRate = 0;
        instance.gameObject.SetActive(true);
        return instance;
    }
    private const int MIN_API_LEVEL_REVIEW = 21;


    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnConfirm;
    [SerializeField] private List<Sprite> lstSprStar;
    [SerializeField] private List<Button> lstBtnStar;
    [SerializeField] private List<Image> lstImgStar;
    private int countStar;
    public override void Show()
    {
        base.Show();
        GameController.Instance.admobAds.DestroyBannerCollapse();
    }
    public void Init()
    {
        btnConfirm.onClick.AddListener(RateAction);
        btnClose.onClick.AddListener(CloseAction);
    }
    public void InitState()
    {
        for (int i = 0; i < lstBtnStar.Count; i++)
        {
            int index = i + 1;
            // lstBtnStar[i].onClick.AddListener(() => { ClickStar(index); });
            lstImgStar[i].sprite = lstSprStar[0];
        }
        countStar = 0;
    }
    public void ClickStar(int index)
    {
        countStar = index;
        for (int i = 0; i < lstImgStar.Count; i++)
        {
            lstImgStar[i].sprite = lstSprStar[0];
        }
        for (int i = 0; i < index; i++)
        {
            lstImgStar[i].sprite = lstSprStar[1];
        }
        //GameController.Instance.musicManager.Pla();
    }
    public void RateAction()
    {
        GameController.Instance.musicManager.PlayClickSound();
        if (countStar <= 0)
        {
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(btnConfirm.transform.position, Localization.Get("s_please_choose_the_star"), Color.red);
            return;
        }
        ShowTextThankRate();
        if (countStar == 5)
        {
            UseProfile.CanShowRate = false;

            try
            {
                Application.OpenURL(Config.OPEN_LINK_RATE);

            }
            catch
            {

            }
            CloseAction();
        }
        else
        {
            CloseAction();
        }
    }
    public void CloseAction()
    {
        UseProfile.CanShowRate = false;
        GameController.Instance.musicManager.PlayClickSound();
        Close();
    }

    public void ShowTextThankRate()
    {
        //StartCoroutine(Helper.StartAction(() =>
        //{
        Debug.Log("close");
        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay
    (
       btnConfirm.transform.position,
       Localization.Get("s_thank_review"),
        GameConfig.Instance.blueColor,
        isSpawnItemPlayer: true
    );
        // }, 0.5f));
    }
}