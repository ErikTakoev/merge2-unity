using UnityEngine;

using Merge2;

public class HeroField : Field
{
    DraggingHeroLogic draggingHeroToBattleLogic;
    protected override void Awake()
    {
        base.Awake();
        
        if (draggableChip == null)
        {
            Debug.LogError("HeroField: DraggableChip is empty");
            return;
        }

        draggingHeroToBattleLogic = new DraggingHeroLogic();
        draggableChip.OnMerge += draggingHeroToBattleLogic.DraggingHero;
    }
}
