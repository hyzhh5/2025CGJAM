using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public List<GameObject> enemies;
    private int enemyNum;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private Transform EnemyTransform;

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
            return;
        }

        if (SpawnPoint != null)
        {
            GameObject enemyObj = Instantiate(enemies[enemyNum], SpawnPoint.position, SpawnPoint.rotation);          
        }
        else
        {
            Debug.LogError("SpawnPoint is not assigned in LevelManager!");
            return;
        }
        Enemy enemy= enemies[enemyNum].GetComponent<Enemy>();
        enemy.MoveTo(EnemyTransform.position, 3f); 


        if (isBoss)
        {
            RoundManager.Instance.SwitchToSelectRound();
        }
        else
        {
            RoundManager.Instance.SwitchToPlayerRound();
        }

        enemyNum++;
    }
}