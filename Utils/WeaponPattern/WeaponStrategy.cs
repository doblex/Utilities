using UnityEngine;

namespace utilities.WeaponPattern
{
    public abstract class WeaponStrategy : ScriptableObject
    {
        [SerializeField] protected int damage = 10;
        [SerializeField] float fireRate = 0.5f;
        [SerializeField] protected float projectileSpeed = 10f;
        [SerializeField] protected float projectileLifeTime = 4f;
        [SerializeField] protected GameObject projectilePrefab;

        public int Damege => damage;
        public float FireRate => fireRate;

        public virtual void Initialize() { }

        public abstract void Fire(Transform firePoint, LayerMask layer);
    }
}
    
