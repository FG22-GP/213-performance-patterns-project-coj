using UnityEngine;
using UnityEngine.Pool;

public class Castle : MonoBehaviour
{
    public Projectile Projectile;

    private Transform _target;
    private int _enemyLayerMask;
    private float _currentCooldown;
    
    const float _maxCooldown = 0.8f;
    
    private ObjectPool<Projectile> _projectilePool;


    void Start()
    {
        this._enemyLayerMask = LayerMask.GetMask("Enemy");
        _projectilePool = new ObjectPool<Projectile>(() => Instantiate(Projectile),
            projectile => projectile.gameObject.SetActive(true),
            projectile => projectile.gameObject.SetActive(false));
    }

    // Update is called once per frame
    void Update()
    {
        AcquireTargetIfNecessary();
        TryAttack();
    }

    void AcquireTargetIfNecessary()
    {
        if (this._target == null)
        {
            this._target = Physics2D.OverlapCircle(this.transform.position, 5f, this._enemyLayerMask)?.transform;
        }
    }

    void TryAttack()
    {
        _currentCooldown -= Time.deltaTime;
        if (this._target == null || !(_currentCooldown <= 0f)) return;
        this._currentCooldown = _maxCooldown;
        Attack();
    }

    void Attack()
    {
        // Use object pool to get a projectile
        var projectile = _projectilePool.Get();
        projectile.transform.position = transform.position;
        projectile.transform.rotation = GetTargetDirection();
    }

    Quaternion GetTargetDirection()
    {
        var dir = this._target.transform.position - this.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
    
    public void ReturnProjectile(GameObject projectile)
    {
        // Return the projectile to the pool
        _projectilePool.Release(projectile.GetComponent<Projectile>());
    }
}