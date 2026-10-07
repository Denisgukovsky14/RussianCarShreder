using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Material material;


    // Update вызывается каждый кадр и считывает данные с цветого ползунка

    private void Update()
    {
       gameObject.GetComponent<Renderer>().material.color = fcp.color;
    }
}
