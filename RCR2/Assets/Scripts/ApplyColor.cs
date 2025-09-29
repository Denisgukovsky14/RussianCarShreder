using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyColor : MonoBehaviour
{
    public FlexibleColorPicker fcp;
    public Material material;

    private void Start()
    {
        //gameObject.GetComponent<Material>() ;
    }

    // Update is called once per frame
    private void Update()
    {
       gameObject.GetComponent<Renderer>().material.color = fcp.color;
    }
}
