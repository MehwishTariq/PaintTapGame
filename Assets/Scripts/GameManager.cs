using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] RotateCamera cameraRef;
    [SerializeField] List<GameObject> levels = new List<GameObject>();
    public static int Level_No;
    public static Action<int> onGameStart;
    GameObject levelObj;

    public void CreateLevel(int levelNo)
    {
        AudioManager.Instance.PlayClick();
        if (levelObj != null)
            Destroy(levelObj);

        Level_No = levelNo;
        levelObj = Instantiate(levels[levelNo - 1],  Vector3.zero,Quaternion.identity);
        levelObj.gameObject.SetActive(true);
        cameraRef.gameObject.SetActive(true);
        cameraRef.target = levelObj.GetComponent<Level>().levelObj.transform;
        StartCoroutine(UIManager.instance.OpenLevel());
    }

}
