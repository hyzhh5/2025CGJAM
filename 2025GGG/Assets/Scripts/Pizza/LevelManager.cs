using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<GameObject> enemies;
    private int enemyNum;
    



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {        
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        enemyNum = 0;
    }


    public void NextEnemy(bool isBoss)
    {

        if (enemyNum >= enemies.Count)
        {
            //TODO:游戏结束，弹出胜利结算UI
        }
        if (isBoss)
        {
            Instantiate(enemies[enemyNum]);
            RoundManager.Instance.SwitchToSelectRound();
        }
        else
        {
            Instantiate(enemies[enemyNum]);
            RoundManager.Instance.SwitchToPlayerRound();
        }
        enemyNum++;
    }


    

}
