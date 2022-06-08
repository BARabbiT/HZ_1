using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateSomeBeings : MonoBehaviour
{
    public GameObject being;

    void Start()
    {
        Instantiate(being, new Vector3(0f, 1f, 0f), new Quaternion());
    }
}
