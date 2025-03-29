using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleArrow : MonoBehaviour
    {
        [SerializeReference]
        SpriteRenderer spriteRenderer;
        [SerializeReference]
        SpriteRenderer redXspriteRenderer;
        Material spriteMaterial;
        Vector2 offset_UV;
        bool hitInUnit;
        [SerializeReference]
        TrailRenderer trail;

        [SerializeReference]
        public CircleCollider2D ArrowCollider;

        [SerializeReference]
        public Rigidbody2D Rigidbody;
        Vector3 ShootPosition;
        public BattleUnit Target;
        BattleUnit Unit;
        float TimeStop;
        float Direction;

        bool isShoot;

        Collider2D[] overlapResults = new Collider2D[5];

        public void Shoot(BattleUnit unit, BattleUnit target)
        {
            isShoot = true;
            ShootPosition = target.transform.position + new Vector3(0, 0.2f, 0);
            Target = target;
            Unit = unit;
            Rigidbody.simulated = true;
            Direction = Mathf.Sign(ShootPosition.x - unit.transform.position.x);
            Rigidbody.linearVelocity = transform.right * 10f * Direction * Random.Range(0.85f, 1.15f);

            TimeStop = Time.time + 0.4f;

            Destroy(gameObject, 1.25f);
            spriteMaterial = spriteRenderer.material;
            DOTween.Sequence().AppendInterval(1f).OnComplete(() =>
            {
                if (redXspriteRenderer.gameObject.activeSelf)
                {
                    redXspriteRenderer.material.DOFade(0, 0.25f);
                }
            }).Append(spriteMaterial.DOFade(0, 0.25f))
            .Play();
        }

        public void HitInUnit(Collider2D collider)
        {
            Destroy(Rigidbody);
            Rigidbody = null;
            transform.parent = collider.transform;
            trail.emitting = false;
            if (collider.transform.name != "Shield")
            {
                redXspriteRenderer.gameObject.SetActive(true);
                hitInUnit = true;
            }
            else
            {
                Target.AddAttacker(Unit);
                HalfArrow();
            }
        }

        private void HalfArrow()
        {
            offset_UV.x = Random.Range(-0.04f, -0.02f);
            spriteMaterial.SetTextureOffset("_BaseMap", offset_UV);
        }

        void Update()
        {
            if (Rigidbody && isShoot)
            {
                if (Target == null)
                {
                    Debug.LogWarning($"BattleArrow: unit: {Unit.name}, target empty");
                }
                else if (PhysicsScene2D.OverlapCollider(ArrowCollider, overlapResults) != 0)
                {
                    var targetColliders = Target.Colliders;
                    foreach (var collider in targetColliders)
                    {
                        if (!collider.gameObject.activeInHierarchy)
                        {
                            continue;
                        }
                        if (overlapResults.Contains(collider))
                        {
                            HitInUnit(collider);
                            break;
                        }
                    }
                }
            }

            if (!hitInUnit)
            {
                return;
            }
            offset_UV.x = offset_UV.x - Time.deltaTime * 0.45f;
            if (offset_UV.x < -0.07f)
            {
                offset_UV.x = -0.07f;
            }
            spriteMaterial.SetTextureOffset("_BaseMap", offset_UV);
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
            if ((TimeStop < Time.fixedTime && transform.position.y < 4f) || transform.position.y < 1.12f)
            {
                Destroy(Rigidbody);
                Rigidbody = null;
                HalfArrow();
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