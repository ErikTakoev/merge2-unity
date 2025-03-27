using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleArrow : MonoBehaviour
    {
        [SerializeReference]
        SpriteRenderer spriteRenderer;
        Material spriteMaterial;
        [SerializeReference]
        ParticleSystem bloodParticle;
        [SerializeReference]
        TrailRenderer trail;

        [SerializeReference]
        public CircleCollider2D ArrowCollider;

        [SerializeReference]
        public Rigidbody2D Rigidbody;
        Vector3 ShootPosition;
        public BattleUnit Target;
        float TimeStop;
        float Direction;

        bool isShoot;

        public void Shoot(BattleUnit unit, BattleUnit target)
        {
            isShoot = true;
            ShootPosition = target.transform.position + new Vector3(0, 0.2f, 0);
            Target = target;
            Rigidbody.simulated = true;
            Direction = Mathf.Sign(ShootPosition.x - unit.transform.position.x);
            Rigidbody.linearVelocity = transform.right * 10f * Direction * Random.Range(0.85f, 1.15f);

            TimeStop = Time.time + 0.3f;

            Destroy(gameObject, 1.25f);
            spriteMaterial = spriteRenderer.material;
            DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
            {
                bloodParticle.Pause();
            }).Append(spriteMaterial.DOFade(0, 0.25f))
            .Play();
        }

        public void HitInUnit(Collider2D collider)
        {
            Destroy(Rigidbody);
            Rigidbody = null;
            transform.parent = collider.transform;
            spriteMaterial.SetTextureOffset("_BaseMap", new Vector2(Random.Range(-0.04f, -0.02f), 0));
            trail.emitting = false;
            if (collider.transform.name != "Shield")
            {
                bloodParticle.Play();
            }
        }

        void FixedUpdate()
        {
            if (!Rigidbody)
            {
                return;
            }
            if (!isShoot)
            {
                return;
            }
            if (TimeStop < Time.fixedTime && transform.position.y < 4f)
            {
                Destroy(Rigidbody);
                Rigidbody = null;
                trail.emitting = false;
            }
            else if (Rigidbody.linearVelocity.sqrMagnitude > 0.01f) // Щоб уникнути обертання при зупинці
            {
                float angle = Mathf.Atan2(Rigidbody.linearVelocity.y, Rigidbody.linearVelocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0,
                    angle + (Direction == -1 ? 180 : 0));
            }

        }
    }
}