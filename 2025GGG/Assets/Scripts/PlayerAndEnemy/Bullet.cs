using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;
    private bool isPlayerBullet;

    // Layer常量 - 使用具体的层级数值
    private const int PLAYER_LAYER = 6;  // Player层
    private const int ENEMY_LAYER = 7;   // Enemy层

    public void Initialize(Transform target, float speed, int damage, bool isPlayerBullet)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.isPlayerBullet = isPlayerBullet;

        // 设置子弹的层级
        gameObject.layer = isPlayerBullet ? PLAYER_LAYER : ENEMY_LAYER;
    }
    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 移动子弹朝向目标
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // 旋转子弹朝向目标
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // 检查是否击中目标
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.1f)
        {
            HandleHit(target.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject hitObject)
    {
        if (isPlayerBullet && hitObject.layer == ENEMY_LAYER)
        {
            Enemy enemy = hitObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Enemy hit with damage: {damage}");
            }
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && hitObject.layer == PLAYER_LAYER)
        {
            Player player = hitObject.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Player hit with damage: {damage}");
            }
            Destroy(gameObject);
        }
    }
}