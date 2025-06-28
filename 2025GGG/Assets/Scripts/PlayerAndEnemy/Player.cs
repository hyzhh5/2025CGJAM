using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("玩家属性")]
    public int coin = 100;
    public int attackPower = 10;

    [Header("子弹设置")]
    [SerializeField] private GameObject BulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float detectionRadius = 10f; // 检测范围

    public static Player Instance { get; private set; }

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

    #region 玩家行为
    public void Attack()
    {
        Debug.Log("Player attacks with power: " + attackPower);
        GameObject nearestEnemy = FindNearestEnemyByLayer();
        if (nearestEnemy != null)
        {
            FireBullet(nearestEnemy.transform);
        }
        else
        {
            Debug.Log("No enemy found in range!");
        }
    }

    private GameObject FindNearestEnemyByLayer()
    {
        // 创建一个检测范围的碰撞数组
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));

        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Collider2D collider in hitColliders)
        {
            float distance = Vector3.Distance(collider.transform.position, currentPosition);
            if (distance < minDistance)
            {
                nearest = collider.gameObject;
                minDistance = distance;
            }
        }

        return nearest;
    }

    private void FireBullet(Transform target)
    {
        Vector3 spawnPosition = firePoint != null ? firePoint.position : transform.position;
        GameObject bullet = Instantiate(BulletPrefab, spawnPosition, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Initialize(target, bulletSpeed, attackPower, true); // true表示是玩家的子弹
    }

    // 在Scene视图中绘制检测范围（用于调试）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void AddCoin(int amount)
    {
        coin += amount;
        Debug.Log("Player's coin increased by: " + amount + ", Total coins: " + coin);
    }

    public void AttackUp(int amount)
    {
        attackPower += amount;
        Debug.Log("Player's attack power increased by: " + amount + ", Total attack power: " + attackPower);
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
        coin -= damage;
        if (coin <= 0)
        {
            Debug.Log("Game Over! Player defeated!");
            //调用关卡管理器的方法
            Destroy(gameObject);
        }
        Debug.Log("Play took damage: " + damage + ", Remaining health: " + coin);
    }


    private void ResetColor()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; // 恢复为默认颜色
        }
    }
    #endregion
}