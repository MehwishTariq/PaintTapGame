using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] RotateCamera cameraRef;
    [SerializeField] List<GameObject> levels = new List<GameObject>();
    public static int levelNo;
    [SerializeField]
    int levelIndex;
    // Start is called before the first frame update
    void Start()
    {
        levelNo = levelIndex;
        GameObject g = Instantiate(levels[levelNo - 1],  Vector3.zero,Quaternion.identity);
        g.gameObject.SetActive(true);
        cameraRef.target = g.GetComponent<Level>().levelObj.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
