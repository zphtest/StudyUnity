using System;
using System.Collections.Generic;
using UnityEngine;

namespace UITest
{
    public enum CommonPage
    {
        None = -1,
        Page1 = 0,
        Page2 = 1,
        Page3 = 2,
        Page4 = 3,
    }

    public class UIToggleGroup : MonoBehaviour
    {
        [SerializeField] private List<UIToggleObject> listToggles = new List<UIToggleObject>();

        private Action<int, int> onClickToggleEvent;

        private int curIndex = -1;
        private int lastIndex = -1;
        public bool IsInit { get; private set; }

        private bool needToggleChange;

        public void Init(Action<int, int> toggleEvent)
        {
            needToggleChange = false;
            curIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < listToggles.Count; i++)
            {
                listToggles[i].SetOnValueChange(OnValueChange);
            }

            onClickToggleEvent = toggleEvent;
            IsInit = true;
        }

        public void SetSelect(int index)
        {
            int last = curIndex;
            if (curIndex == index)
            {
                onClickToggleEvent?.Invoke(curIndex, lastIndex);
            }

            curIndex = index;

            for (int i = 0; i < listToggles.Count; i++)
            {
                listToggles[i].SetValue(index == i);
            }

            // 2020 Toggle
            needToggleChange = true;
            if (last == -1)
            {
                onClickToggleEvent?.Invoke(index, last);
            }
        }

        public void SetText(int index, string title)
        {
            if (index >= 0 && index < listToggles.Count)
            {
                listToggles[index].SetText(title);
            }
        }

        public int GetSelectIndex()
        {
            return curIndex;
        }

        private void OnValueChange(UIToggleObject toggle)
        {
            if (!needToggleChange)
                return;
            lastIndex = curIndex;
            curIndex = listToggles.IndexOf(toggle);
            onClickToggleEvent?.Invoke(curIndex, lastIndex);
        }

        public void AddToggleItem(UIToggleObject toggle)
        {
            listToggles.Add(toggle);
        }

        public void SetActive(int index, bool isVisible)
        {
            if (index >= 0 && index < listToggles.Count)
            {
                listToggles[index].gameObject.SetActive(isVisible);
            }
        }

        // 刷新一下状态
        // public void RefreshToggle(int index)
        // {
        //     for (int i = 0; i < listToggles.Count; i++)
        //     {
        //         listToggles[i].SetValue(index == i);
        //     }
        // }

    }

}