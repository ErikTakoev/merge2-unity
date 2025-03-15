using System;
using UnityEngine;
using UnityEngine.UI;

namespace BattleField
{
    public class BattleFieldEnemySpawner
    {
        string[] enemyDataSpawn;
        float nextTimeSpawn;
        int currentIndex = 0;

        // <PositionY, enemyIndex>
        public event Action<int, int> OnSpawn;

        public BattleFieldEnemySpawner(string[] enemyDataSpawn)
        {
            this.enemyDataSpawn = enemyDataSpawn;
        }

        public void Update()
        {
            if (OnSpawn == null)
            {
                return;
            }
            if (nextTimeSpawn > Time.time)
            {
                return;
            }
            nextTimeSpawn = Time.time + 5;

            for (int i = 0; i < enemyDataSpawn.Length; i++)
            {
                var str = enemyDataSpawn[i];
                if (currentIndex < str.Length)
                {
                    char symbol = str[currentIndex];
                    if (!char.IsDigit(symbol))
                    {
                        continue;
                    }
                    int enemyIndex = (symbol - '0') - 1;
                    OnSpawn(i, enemyIndex);
                }
            }
            currentIndex++;
        }
    }
}