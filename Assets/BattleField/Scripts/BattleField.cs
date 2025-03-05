using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleField : MonoBehaviour
    {
        [SerializeReference]
        GameObject heroPrefab;
        [SerializeField]
        Transform[] heroesStartPositions;

        List<BattleHero> heroes;

        void Start()
        {
            heroes = new List<BattleHero>();
        }

        public void OnDraggedHeroToBattleField(BattleHeroStyle style, List<EquipmentItem> items)
        {
            if (heroPrefab == null)
            {
                Debug.LogError("BattleField: heroPrefab is empty");
                return;
            }

            BattleHero hero = Instantiate(heroPrefab, transform).GetComponent<BattleHero>();
            heroes.Add(hero);

            hero.SetStyle(style);
            hero.SetEquipments(items);
        }
    }
}
