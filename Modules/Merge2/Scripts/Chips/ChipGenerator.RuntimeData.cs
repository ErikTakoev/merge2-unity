namespace Merge2
{
    public partial class ChipGenerator
    {
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
    }
}