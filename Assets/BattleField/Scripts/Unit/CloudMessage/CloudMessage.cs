using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BattleField
{
    public class CloudMessage : MonoBehaviour
    {
        [SerializeReference]
        TextMeshPro textMeshPro;
        Material[] materials;
        float timeToDestroy;

        public void Init(string text, float timeToDestroy)
        {
            textMeshPro.SetText(text);
            this.timeToDestroy = timeToDestroy;
            Destroy(gameObject, timeToDestroy);
        }

        void Start()
        {
            FindAllMaterials();
            SetAlphaZero();
            FadeChildren(1f, 0.5f);
            DOVirtual.DelayedCall(timeToDestroy - 0.3f, () => FadeChildren(0, 0.3f)).Play();
        }

        void SetAlphaZero()
        {
            foreach (Material mat in materials)
            {
                var color = mat.color;
                color.a = 0;
                mat.color = color;
            }
            textMeshPro.alpha = 0;
        }

        void FindAllMaterials()
        {
            var spriteRenderers = gameObject.GetComponentsInChildren<SpriteRenderer>();

            materials = new Material[spriteRenderers.Length];
            int i = 0;
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                materials[i++] = sr.material;
            }
        }

        void FadeChildren(float targetAlpha, float duration)
        {
            foreach (Material mat in materials)
            {
                mat.DOFade(targetAlpha, duration);
            }

            DOVirtual.Float(textMeshPro.alpha, targetAlpha, duration, (value) =>
            {
                textMeshPro.alpha = value;
            }).Play();
        }
    }
}
