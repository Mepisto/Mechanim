using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

    private Button btn;
    // Use this for initialization
    void Start ()
    {
        btn = gameObject.GetComponent<Button>();
        //btn.onClick.AddListener(this.OnButtonEvent);
        //btn.OnPointerDown(this.OnButtonEvent());
        

    }

    // Update is called once per frame
    void Update () {
		
	}


}
