using UnityEngine;

using Merge2;

public class DraggingHeroLogic
{
    private System.Action<HeroChip> onBattleChip;
    public DraggingHeroLogic(System.Action<HeroChip> onBattleChip)
    {
        this.onBattleChip = onBattleChip;
    }

    public bool DraggingHero(Cell prevCell, Cell newCell)
    {
        if (newCell)
        {
            return false;
        }
        if (!prevCell)
        {
            Debug.LogError("DraggingHero: prevCell is null");
            return false;
        }

        if (prevCell.Chip is HeroChip heroChip)
        {
            onBattleChip?.Invoke(heroChip);
            prevCell.Chip.Destroy();
            prevCell.Chip = null;
            return true;
        }
        return false;
    }
}