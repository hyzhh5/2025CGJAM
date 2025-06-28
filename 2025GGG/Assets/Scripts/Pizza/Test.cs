using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.NextEnemy(false);
        LevelManager.Instance.NextEnemy(false);
        LevelManager.Instance.NextEnemy(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
