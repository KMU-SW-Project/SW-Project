using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public bool active;
    public float weight;
    public bool isFixed;

    private void Update()
    {
        if (active)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(Vector3.zero), Time.deltaTime * weight);
        }
        else
        {
            if (isFixed) return;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(90, 0, 0)), Time.deltaTime * weight);
        }
    }
}
