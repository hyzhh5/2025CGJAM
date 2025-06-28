using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("基础属性")]
    public float health = 30;
    public int attackPower = 5;

    [Header("攻击设置")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private Transform firePoint; // 发射点

    private Transform playerTransform;

    private Color defaultColor;

    private void OnEnable()
    {
        // 订阅事件
        RoundManager.OnEnemyRoundStart += StartAttacking;
    }

    private void OnDisable()
    {
        // 取消订阅事件
        RoundManager.OnEnemyRoundStart -= StartAttacking;
    }

    private void Start()
    {
        // 获取玩家引用
        playerTransform = Player.Instance.transform;

        defaultColor = GetComponent<SpriteRenderer>().color;
    }

    private void StartAttacking()
    {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(5f);
        if (playerTransform != null)
        {
            FireBullet();
        }
        yield return new WaitForSeconds(5f);
        RoundManager.Instance.SwitchToPlayerRound(); // 攻击后切换到玩家回合
    }
    private void FireBullet()
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(playerTransform, bulletSpeed, attackPower, false); // false表示是敌人的子弹
    }

    public void TakeDamage(int damage)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // 播放受伤动画
            spriteRenderer.color = Color.red; // 简单的受伤效果
            Invoke("ResetColor", 0.1f); // 恢复颜色
        }
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("Enemy defeated!");
            //调用关卡管理器的方法
            Destroy(gameObject);
        }
        Debug.Log("Enemy took damage: " + damage + ", Remaining health: " + health);
    }

    private void ResetColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = defaultColor; // 恢复为默认颜色
        }
    }
}