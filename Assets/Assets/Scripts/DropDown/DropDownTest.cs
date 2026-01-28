using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownTest : MonoBehaviour
{
    [SerializeField] Dropdown dropdown;
    
    // Start is called before the first frame update
    void Start()
    {
        dropdown.onValueChanged.AddListener(ValChange);
    }

    private void ValChange(int arg0)
    {
        Debug.Log(arg0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
