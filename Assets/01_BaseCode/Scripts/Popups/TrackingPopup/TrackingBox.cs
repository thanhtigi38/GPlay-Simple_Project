using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackingBox : BaseBox
{
    private static TrackingBox instance;
    public static TrackingBox Setup(bool isSaveBox = false, Action actionOpenBoxSave = null)
    {
        if (instance == null)
        {
            instance = Instantiate(Resources.Load<TrackingBox>(PathPrefabs.TRACKING_BOX));
            instance.Init();
        }
        return instance;
    }

    [SerializeField] private Button yesTrackingBtn;
    [SerializeField] private Button noTrackingBtn;
    private Action actionCloseBox;
    public void Init()
    {
        yesTrackingBtn.onClick.AddListener(OnClickYesTracking);
        noTrackingBtn.onClick.AddListener(OnClickNoTracking);
    }    

    public void Show(Action actionClose)
    {
        this.actionCloseBox = actionClose;
    }    

    private void OnClickYesTracking()
    {
        UseProfile.IsTrackedPremission = true;
        UseProfile.IsAcceptTracker = true;
        this.actionCloseBox?.Invoke();
        Close();
    }

    private void OnClickNoTracking()
    {
        UseProfile.IsTrackedPremission = true;
        UseProfile.IsAcceptTracker = false;
        this.actionCloseBox?.Invoke();
        Close();
    }
}
