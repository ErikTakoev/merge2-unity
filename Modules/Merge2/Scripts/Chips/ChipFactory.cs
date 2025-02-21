using UnityEngine;

public static class ChipFactory
{
    public static bool CreateChip(Cell cell, ChipData chipData)
    {
        if (chipData == null)
        {
            Debug.LogError("CreateChip: ChipData is empty");
            return false;
        }
        if (chipData.PrefabLink == null)
        {
            Debug.LogError("CreateChip: PrefabLink is empty");
            return false;
        }
        if (cell.Chip != null)
        {
            Debug.LogError("CreateChip: in Cell.Chip is not empty");
            return false;
        }

        GameObject chipGO = GameObject.Instantiate(chipData.PrefabLink);
        Chip chip = chipGO.GetComponent<Chip>();
        chip.Init(chipData);

        cell.Chip = chip;
        return true;
    }
}
