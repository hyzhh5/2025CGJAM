using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;
    private bool isPlayerBullet;

    [SerializeField] private LayerMask playerLayer;  // 在Inspector中设置
    [SerializeField] private LayerMask enemyLayer;   // 在Inspector中设置

    public void Initialize(Transform target, float speed, int damage, bool isPlayerBullet)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.isPlayerBullet = isPlayerBullet;

        // 设置子弹的层
        gameObject.layer = isPlayerBullet ? 
            (int)Mathf.Log(playerLayer.value, 2) : 
            (int)Mathf.Log(enemyLayer.value, 2);
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
        int objectLayer = 1 << hitObject.layer;
        Debug.Log($"isPlayerBullet: {isPlayerBullet}, hitObject layer: {hitObject.layer}, mask: {objectLayer}");

        if (isPlayerBullet && (enemyLayer.value & objectLayer) != 0)
        {
            Enemy enemy = hitObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Enemy hit with damage: {damage}");
            }
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && (playerLayer.value & objectLayer) != 0)
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