using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Toggle))]
public class UIToggleObject : MonoBehaviour
{
    [SerializeField] private List<GameObject> activate;

    [SerializeField] private List<GameObject> deactivate;

    [SerializeField] private TextMeshProUGUI txtTab;

    [SerializeField] private TextMeshProUGUI txtHighLight;

    private Action<UIToggleObject> onValueChangedEvent;

    private Toggle _toggle;

    private Toggle toggle
    {
        get
        {
            if (_toggle == null)
            {
                _toggle = GetComponent<Toggle>();
            }

            return _toggle;
        }
    }

    public void SetOnValueChange(Action<UIToggleObject> onValueChanged)
    {
        onValueChangedEvent = onValueChanged;
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif
        Toggle(false);
        toggle.onValueChanged.AddListener(Toggle);
    }

    public void SetValue(bool isOn)
    {
        toggle.isOn = isOn;
    }

    public bool IsOn => toggle.isOn;

    public void Toggle(bool isOn = false)
    {
        bool val = isOn;
        if (enabled)
        {
            for (int i = 0; i < activate.Count; ++i)
                Set(activate[i], val);

            for (int i = 0; i < deactivate.Count; ++i)
                Set(deactivate[i], !val);

            if (val)
            {
                onValueChangedEvent?.Invoke(this);
            }
        }
    }

    void Set(GameObject go, bool state)
    {
        if (go != null)
        {
            go.SetActive(state);
        }
    }

    public void SetText(string title)
    {
        if (txtTab)
            txtTab.SetText(title);
        if (txtHighLight)
            txtHighLight.SetText(title);
    }

}

