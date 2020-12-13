using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class PassiveMark : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Passive());
    }

    IEnumerator Passive()
    {
        yield return new WaitForSeconds(0.2f);
      //  Player.instance.ReturnMark(gameObject);
    }
}
