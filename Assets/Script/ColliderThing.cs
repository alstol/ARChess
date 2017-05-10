using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderThing : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.tag);
    }
}
