using UnityEngine;

namespace utilities.Projectiles
{
    public class BaseProjectile2D : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] GameObject muzzlePrefab;
    
        [SerializeField] GameObject hitPrefab;
        [SerializeField] float hitPrefabDuration = 2f;
    
        int damage;
    
        Transform parent;
    
        public void SetSpeed(float speed) => this.speed = speed;
        public void SetDamage(int damage) => this.damage = damage;
        public void SetParent(Transform parent) => this.parent = parent;
    
    
        private void Start()
        {
            if (muzzlePrefab != null)
            { 
                GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
                muzzleVFX.transform.forward = gameObject.transform.forward;
                muzzleVFX.transform.SetParent(parent);
            }
        }
    
        private void Update()
        {
            transform.SetParent(null);
            transform.position += transform.right * (speed * Time.deltaTime);
        }
    
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (hitPrefab != null)
            { 
                ContactPoint2D contact = collision.contacts[0];
                GameObject hitVFX = Instantiate(hitPrefab, contact.point, Quaternion.identity);
                Destroy(hitVFX, hitPrefabDuration);
            }
    
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
            {
                if (collision.gameObject.TryGetComponent<HealthController>(out HealthController HealtController))
                {
                    HealtController.DoDamage(damage);
                }
            }
    
            Destroy(gameObject);
        }
    }
}