using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class UIToggleObj : MonoBehaviour {
    public UIToggle toggle;
    public GameObject on;
    public GameObject off;

	void Start () {
        if (toggle == null)
        {
            toggle = GetComponent<UIToggle>();
        }
        Set(toggle.startsActive);
		EventDelegate.Add(toggle.onChange, OnToggleChange);
	}

	private void OnToggleChange() {
        Set(toggle.value);
    }

    private void Set (bool value)
    {
        on.SetActive(toggle.value);
        off.SetActive(!toggle.value);
    }
}
