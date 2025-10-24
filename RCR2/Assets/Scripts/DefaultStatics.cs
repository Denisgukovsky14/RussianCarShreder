using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultStatics : MonoBehaviour
{
    public int Counter = 0 ;

    public void Count()
    {
        Counter = Counter + 1 ;
        //Debug.Log(Counter);
    } 
}
