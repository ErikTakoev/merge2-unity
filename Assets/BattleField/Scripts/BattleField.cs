using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using UnityEngine;

namespace BattleField
{
    public class BattleField : MonoBehaviour
    {
        [SerializeReference]
        GameObject unitPrefab;
        [SerializeField]
        Transform[] heroesStartPositions;

        [SerializeField]
        BattleGrid grid;

        List<BattleHero> heroes;
        [SerializeField]
        BattleHero heroBoss;
        [SerializeField]
        GameObject heroBossPrefab;

        List<BattleHero> enemies;
        [SerializeField]
        BattleHero enemyBoss;
        [SerializeField]
        GameObject enemyBossPrefab;

        public static BattleField Instance;

        void Start()
        {
            Instance = this;
            heroes = new List<BattleHero>();
            enemies = new List<BattleHero>();

            if (heroBossPrefab == null || enemyBossPrefab == null)
            {
                Debug.LogError("BattleField: heroBossPrefab or enemyBossPrefab is empty");
                return;
            }

            heroBoss = CreateUnit(heroBossPrefab, true);
            enemyBoss = CreateUnit(enemyBossPrefab, false);
        }

        public void OnDraggedHeroToBattleField(BattleHeroStyle style, List<EquipmentItem> items)
        {
            if (unitPrefab == null)
            {
                Debug.LogError("BattleField: heroPrefab is empty");
                return;
            }

            CreateUnit(style, items, true);
        }

        public BattleHero FindTarget(BattleHero unit)
        {
            List<BattleHero> targets = unit.IsHero ? enemies : heroes;

            BattleHero target = null;
            float minDistance = float.MaxValue;

            foreach (BattleHero t in targets)
            {
                float distance = Pathfinding.GetDistance(unit.Cell, t.Cell);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    target = t;
                }
            }
            return target;
        }

        public void FindPath(BattleCell unitCell, BattleCell targetCell, System.Action<List<BattleCell>> callback)
        {
            grid.FindPathAsync(unitCell, targetCell, callback);
        }

        BattleHero CreateUnit(GameObject prefab, bool isHero)
        {
            BattleCell spawnPoint = grid.GetRandomSpawnPoint(isHero);

            BattleHero hero = Instantiate(prefab, transform).GetComponent<BattleHero>();

            if (isHero)
            {
                heroes.Add(hero);
            }
            else
            {
                enemies.Add(hero);
            }
            hero.IsHero = isHero;

            hero.SetCell(spawnPoint);

            return hero;
        }

        private BattleHero CreateUnit(BattleHeroStyle style, List<EquipmentItem> items, bool isHero)
        {
            BattleHero hero = CreateUnit(unitPrefab, isHero);

            hero.SetStyle(style);
            hero.SetEquipments(items);

            return hero;
        }
    }
}
