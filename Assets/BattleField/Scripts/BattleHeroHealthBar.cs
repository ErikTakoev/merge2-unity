using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleField
{
    public class BattleHeroHealthBar : MonoBehaviour
    {
        [SerializeField]
        RectTransform healthValue;
        Image healthImage;
        [SerializeField]
        TextMeshProUGUI textMesh;
        float originalWidth;

        void Start()
        {
            originalWidth = healthValue.sizeDelta.x;
            healthImage = healthValue.GetComponent<Image>();
        }

        public void OnChangeHealth(int newValue, int fullHealth)
        {
            var value = newValue / (float)fullHealth;
            Color redColor = Color.red;
            Color greenColor = Color.green;
            Color color = Color.Lerp(redColor, greenColor, value);
            
            var size = healthValue.sizeDelta;
            size.x = value * originalWidth;
            healthValue.sizeDelta = size;

            healthImage.color = color;

            textMesh.SetText($"{newValue}/{fullHealth}");
        }
    }
}
