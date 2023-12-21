using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour
{
    public Material mat;
    ParticleSystem p_s;
    public static Action<Vector3> changePos;

    void ChangePos(Vector3 pos)
    {
        transform.position = pos;
        if (p_s != null)
        {
            mat.color = UIManager.chosenClr;
            p_s.Play();
        }
    }

    private void OnEnable()
    {
        p_s = GetComponent<ParticleSystem>();
        changePos += ChangePos;
    }

    private void OnDisable()
    {
        changePos -= ChangePos;
    }
}
