using UnityEngine;

namespace utilities.Projectiles
{
    public abstract class BaseProjectile3D : MonoBehaviour
    {
        [SerializeField] protected float speed;
        [SerializeField] protected GameObject muzzlePrefab;

        [SerializeField] protected GameObject hitPrefab;
        [SerializeField] protected float hitPrefabDuration = 2f;

        protected int damage;

        protected Transform parent;

        public void SetSpeed(float speed) => this.speed = speed;
        public void SetDamage(int damage) => this.damage = damage;
        public void SetParent(Transform parent) => this.parent = parent;
    }
}
    