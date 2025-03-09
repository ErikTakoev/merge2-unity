using UnityEngine;

namespace BattleField
{
    public class BattleHeroSortingAndScale : MonoBehaviour
    {
        void Start()
        {
        
        }

        void Update()
        {
            var tmpLocalPos = transform.localPosition;
            tmpLocalPos.z = tmpLocalPos.y * 0.01f;
            transform.localPosition = tmpLocalPos;

            var tmpLocalScale = transform.localScale;
            tmpLocalScale.x = 1 - tmpLocalPos.y * 0.1f;
            tmpLocalScale.y = tmpLocalScale.x;
            transform.localScale = tmpLocalScale;

        }
    }
}
