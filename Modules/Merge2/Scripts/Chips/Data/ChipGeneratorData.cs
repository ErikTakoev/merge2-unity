using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static Merge2.ChipGeneratorData;

namespace Merge2
{
    [System.Serializable]
    public class ChipGeneratorData
    {
        public GeneratorData[] Data;
        public bool IsStartCharged = true;
        public int ChargeCount = 1;
        public int ChargingTime = 10;

        public ChipData NextChipData;

        public ChipData GenerateChipData()
        {
            if (Data.Length == 0)
            {
                Debug.LogError("ChipGeneratorData: Data is empty");
                return null;
            }
            int x = 0;
            StringBuilder sb = new StringBuilder();
            KeyValuePair<int, ChipData>[] tmpArr = new KeyValuePair<int, ChipData>[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                GeneratorData generatorData = Data[i];
                x += generatorData.Weight;
                tmpArr[i] = new KeyValuePair<int, ChipData>(x, generatorData.ChipData);
                sb.AppendLine($"ChipData: {generatorData.ChipData.name}, value: {x}");
            }
            int randomValue = Random.Range(1, x + 1);
            sb.AppendLine($"Random Value: {randomValue}");
            ChipData result = null;
            for (int i = 0; i < tmpArr.Length; i++)
            {
                var pair = tmpArr[i];
                if (randomValue <= pair.Key)
                {
                    result = pair.Value;
                    sb.AppendLine($"Win ChipData: {pair.Value.name}, x: {pair.Key}");
                    break;
                }
            }
            Debug.Log(sb.ToString());

            return result;
        }

        [System.Serializable]
        public class GeneratorData
        {
            [Range(1, 100)]
            public int Weight;
            public ChipData ChipData;
        }
    }
}