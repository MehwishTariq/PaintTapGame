using System;
using UnityEngine;

public struct SizeData
{
    public float value;
    public bool zoomed;
    public SizeData(float value, bool zoomed) : this()
    {
        this.value = value;
        this.zoomed = zoomed;
    }
}

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

    void ChangeSize(SizeData sizeData)
    {
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * (sizeData.zoomed? 0.5f : 2f), sizeData.value);
    }

    private void OnEnable()
    {
        p_s = GetComponent<ParticleSystem>();
        EventManager.SubscribeToEvent<Vector3>(EventNames.OnChangeParticlePos, ChangePos);
        EventManager.SubscribeToEvent<SizeData>(EventNames.OnChangeParticleSize, ChangeSize);
    }

    private void OnDisable()
    {
        EventManager.UnsubscribeFromEvent<Vector3>(EventNames.OnChangeParticlePos, ChangePos);
        EventManager.UnsubscribeFromEvent<SizeData>(EventNames.OnChangeParticleSize, ChangeSize);
    }
}
