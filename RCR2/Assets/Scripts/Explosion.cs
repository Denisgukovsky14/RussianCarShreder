using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius;
    public float Force;

    public bool Active;

    public GameObject ExplossionEffect;

    private void Update()
    {
        if (Active)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, Radius);
        
        for (int i = 0; i < overlappedColliders.Length; i++)
        {
            Rigidbody rigidbody = overlappedColliders[i].attachedRigidbody;
            if (rigidbody != null)
            {
                Debug.Log(rigidbody);
                rigidbody.AddExplosionForce(Force, transform.position, Radius);
            }
        }
        //Destroy(gameObject);
        var explosion = Instantiate( ExplossionEffect, transform.position,Quaternion.identity  );
        Destroy(explosion, 1f);
        Active = false;
    }



}
