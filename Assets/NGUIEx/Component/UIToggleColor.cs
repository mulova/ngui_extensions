using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class UIToggleColor : MonoBehaviour
{
    public UIWidget widget;
    public Color onColor = Color.white;
    public Color offColor = Color.white;

    private UIToggle toggle;

    void Start()
    {
        toggle = GetComponent<UIToggle>();
        OnToggleChange();
        EventDelegate.Add(toggle.onChange, OnToggleChange);
    }

    private void OnToggleChange()
    {
        widget.color = toggle.value? onColor : offColor;
    }
}
