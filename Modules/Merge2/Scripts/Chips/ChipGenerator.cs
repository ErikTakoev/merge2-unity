using UnityEngine;

namespace Merge2
{
    public class ChipGenerator : Chip
    {
        ChipGeneratorData generatorData;
        RuntimeData runtimeData;

        class RuntimeData
        {
            public bool IsCharged;
            public int ChargeCount;
            public float ChargingTimeLeft;

            public RuntimeData(ChipGeneratorData data)
            {
                IsCharged = data.IsStartCharged;
                ChargeCount = data.ChargeCount;
                ChargingTimeLeft = 0;
            }
        }

        public override void Init(ChipData data)
        {
            base.Init(data);

            generatorData = data.ChipGeneratorData;
            if (generatorData == null)
            {
                Debug.LogError("ChipGenerator: generatorData is null");
                return;
            }
            runtimeData = new RuntimeData(data.ChipGeneratorData);
        }

        public override void OnTap(Vector2 position)
        {
            if (!runtimeData.IsCharged)
            {
                return;
            }

            ChipData generateChipData = generatorData.GenerateChipData();
            if (generateChipData == null)
            {
                return;
            }

            Cell cell = transform.parent.GetComponent<Cell>();
            if (cell == null)
            {
                Debug.LogError("ChipGenerator: OnTap cell is null");
                return;
            }
            GameManager gameManager = GameManager.Instance;
            var findCell = gameManager.FindNearestFreeCell(cell.CellPosition);

            if (findCell == null)
            {
                Debug.Log("ChipGenerator: OnTap not find empty cell");
                return;
            }
            Chip newChip = ChipFactory.CreateChip(findCell, generateChipData);


            runtimeData.ChargeCount--;
            if (runtimeData.ChargeCount == 0)
            {
                runtimeData.IsCharged = false;
            }
        }


    }
}