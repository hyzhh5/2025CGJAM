using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("玩家属性")]
    public int coin = 100; // Player's health
    public int attackPower = 10; // Player's attack power

    public static Player Instance { get; private set; }
    // Start is called before the first frame update
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

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    #region 玩家行为
    public void Attack()
    {
        Debug.Log("Player attacks with power: " + attackPower);
        
    }
    public void AddCoin(int amount)//治疗
    {
        coin += amount;
        Debug.Log("Player's coin increased by: " + amount + ", Total coins: " + coin);
    }
    public void AttackUp(int amount)//攻击力提升
    {
        attackPower += amount;
        Debug.Log("Player's attack power increased by: " + amount + ", Total attack power: " + attackPower);
    }
    #endregion
}
