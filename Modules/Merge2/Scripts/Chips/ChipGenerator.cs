using UnityEngine;

namespace Merge2
{
    public partial class ChipGenerator : Chip
    {
        ChipGeneratorData generatorData;
        RuntimeData runtimeData;

        event System.Action<float> OnCharging;
        [SerializeField]
        ChipGeneratorEffect generatorEffect;

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
            if (generatorEffect == null)
            {
                Debug.LogError("ChipGenerator: generatorEffect is null");
                return;
            }
            OnCharging += generatorEffect.OnCharging;
            if (!runtimeData.IsCharged) 
            {
                generatorEffect.Activate(this);
            }
            else
            {
                generatorEffect.Deactivate(this);
            }
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
            
            Chip newChip = ChipFactory.CreateChip(findCell, generateChipData, transform.position);
            newChip.SendTrigger(AnimatorTrigger.Generate);

            runtimeData.ChargeCount--;
            if (runtimeData.ChargeCount == 0)
            {
                runtimeData.IsCharged = false;
                if (generatorData.NextChipData != null)
                {
                    cell.Chip = null;
                    Destroy();
                    ChipFactory.CreateChip(cell, generatorData.NextChipData);
                }
                else
                {
                    generatorEffect.Activate(this);
                }
            }
        }

        private void Update()
        {
            if (runtimeData.IsCharged)
            {
                return;
            }
            runtimeData.ChargingTimeLeft += Time.deltaTime;
            
            OnCharging?.Invoke(Mathf.Min(runtimeData.ChargingTimeLeft / generatorData.ChargingTime, 1f));
            
            if (runtimeData.ChargingTimeLeft >= generatorData.ChargingTime)
            {
                runtimeData.IsCharged = true;
                runtimeData.ChargingTimeLeft = 0;
                runtimeData.ChargeCount = generatorData.ChargeCount;
                generatorEffect.Deactivate(this);

                SendTrigger(AnimatorTrigger.Recharge);
            }
        }
    }
}