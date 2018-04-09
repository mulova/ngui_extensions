using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using ngui.ex;
using comunity;

public static class UIButtonEx
{

    public static void SetCallback(this UIButton button, EventDelegate.Callback callback, bool oneshot = false)
    {
        button.onClick.Clear();
        if (callback != null)
        {
            EventDelegate.Add(button.onClick, callback, oneshot);
        }
    }

    public static void AddCallback(this UIButton button, EventDelegate.Callback callback, bool oneshot = false)
    {
        if (callback != null)
        {
            EventDelegate.Add(button.onClick, callback, oneshot);
        }
    }

    public static void SetCallback<T>(this UIButton button, Action<T> method, T param) where T:Object
    {
        button.onClick.Clear();
        if (method != null)
        {
            AddCallback(button, method, param);
        }
    }

    public static void AddCallback<T>(this UIButton button, Action<T> method, T param) where T:Object
    {
        if (method != null)
        {
            AddCallback(button.onClick, method, param);
        }
    }

    public static void AddCallback<T>(List<EventDelegate> callbackList, Action<T> method, T param) where T:Object
    {
        if (method != null)
        {
            EventDelegateUtil.AddCallback(callbackList, method, param);
        }
    }

    public static void SetButtonActive(this UIButton button, bool active, bool instant = true)
    {
        if (button == null)
        {
            return;
        }
        bool includeInactive = true; // this is not necessary. for the special ui (tab may be disabled in YoVillain)
        UIButtonColor[] buttons = button.GetComponentsInChildren<UIButtonColor>(includeInactive);
        foreach (UIButtonColor b in buttons)
        {
            b.isEnabled = active;
            b.SetState(active? UIButtonColor.State.Normal : UIButtonColor.State.Disabled, instant);
        }
    }
    
    public static void SetButtonTextColor(this UIButton button)
    {
        button.SetButtonTextColor(Color.white, new Color32(110, 128, 124, 255));
    }

    public static void SetButtonTextColor(this UIButton button, Color activeTxtColor, Color inactiveTxtColor)
    {
        UIButtonColor[] colors = button.GetComponents<UIButtonColor>();
        UILabel label = button.GetComponentInChildrenEx<UILabel>();
        if (label == null)
        {
            return;
        }
        UIButtonColor c = null;
        if (colors.Length < 2)
        {
            c = button.gameObject.AddComponent<UIButtonColor>();
        } else
        {
            foreach (UIButtonColor col in colors)
            {
                if (col != button)
                {
                    c = col;
                    break;
                }
            }
        }
        c.tweenTarget = label.gameObject;
        c.tweenTarget.GetComponent<UIWidget>().color = activeTxtColor;
        c.hover = activeTxtColor;
        c.pressed = activeTxtColor;
        c.disabledColor = inactiveTxtColor;
    }
}
