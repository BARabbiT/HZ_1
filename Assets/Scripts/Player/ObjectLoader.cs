using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLoader : MonoBehaviour
{

 


    //private void OnCollisionEnter(UnityEngine.Collision other)
    //{
    //    if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Water")
    //        return;

    //    other.gameObject.SetActive(true);
    //    Debug.Log(other.gameObject.name + " is active true");
    //}

    //private void OnCollisionExit(UnityEngine.Collision other)
    //{
    //    if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Water")
    //        return;

    //    other.gameObject.SetActive(false);
    //    Debug.Log(other.gameObject.name + " is active false");
    //}

   // SphereCollider _sphereCollider;

    private void Start()
    {
        //_sphereCollider = GetComponent<SphereCollider>();

 

        //Collider[] hitColliders;
        //hitColliders = Physics.OverlapSphere(transform.position, _sphereCollider.radius); // Should probably add layermask and a triggerquery

        //for (int i = hitColliders.Length - 1; i > -1; i--)
        //{
        //    ObjectComponentState(hitColliders[i], true);
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectComponentState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        ObjectComponentState(other, false);
    }

    private void ObjectComponentState(Collider other, bool state)
    {
        if (other.gameObject.tag == "Terrain" || other.gameObject.tag == "Water" || other.gameObject.tag == "StaticObjects" || other.gameObject.tag == "Alive")
            return;

        var meshR = other.gameObject.GetComponent<MeshRenderer>();
        if (meshR != null)
            other.gameObject.GetComponent<MeshRenderer>().enabled = state;

        //other.enabled = state;

        //Debug.Log(other.gameObject.name);
    }
}

