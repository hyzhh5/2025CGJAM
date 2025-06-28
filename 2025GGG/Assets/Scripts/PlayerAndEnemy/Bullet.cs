using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Transform target;
    private float speed;
    private int damage;
    private bool isPlayerBullet;

    // 声明LayerMask变量
    private static int PlayerLayer;
    private static int EnemyLayer;
    private static LayerMask PlayerMask;
    private static LayerMask EnemyMask;
    private static bool isInitialized = false;

    private void Awake()
    {
        InitializeLayers();
    }

    // 初始化层级
    private static void InitializeLayers()
    {
        if (!isInitialized)
        {
            PlayerLayer = LayerMask.NameToLayer("Player");
            EnemyLayer = LayerMask.NameToLayer("Enemy");
            PlayerMask = 1 << PlayerLayer;
            EnemyMask = 1 << EnemyLayer;
            isInitialized = true;
        }
    }

    public void Initialize(Transform target, float speed, int damage, bool isPlayerBullet)
    {
        this.target = target;
        this.speed = speed;
        this.damage = damage;
        this.isPlayerBullet = isPlayerBullet;

        // 确保层级已初始化
        InitializeLayers();
        
        // 设置子弹的层级
        gameObject.layer = isPlayerBullet ? PlayerLayer : EnemyLayer;
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
        // 使用LayerMask判断
        if (isPlayerBullet && ((1 << hitObject.layer) & EnemyMask) != 0)
        {
            Enemy enemy = hitObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"Enemy hit with damage: {damage}");
            }
            Destroy(gameObject);
        }
        else if (!isPlayerBullet && ((1 << hitObject.layer) & PlayerMask) != 0)
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

    // 可选：用于调试的方法
    private void Start()
    {
        Debug.Log($"Player Layer: {PlayerLayer}");
        Debug.Log($"Enemy Layer: {EnemyLayer}");
        Debug.Log($"Bullet Layer: {gameObject.layer}");
        Debug.Log($"Player Mask: {PlayerMask}");
        Debug.Log($"Enemy Mask: {EnemyMask}");
    }
}