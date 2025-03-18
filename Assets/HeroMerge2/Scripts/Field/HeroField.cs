using UnityEngine;

using Merge2;

public class HeroField : Field
{
	[SerializeReference]
	BattleField.BattleField battleField;
	DraggingHeroLogic draggingHeroToBattleLogic;
	protected override void Awake()
	{
		base.Awake();

		if (draggableChip == null)
		{
			Debug.LogError("HeroField: DraggableChip is empty");
			return;
		}

		draggingHeroToBattleLogic = new DraggingHeroLogic(OnDraggedHeroToBattleField);
		draggableChip.OnMerge += draggingHeroToBattleLogic.DraggingHero;
	}

	public void OnDraggedHeroToBattleField(HeroChip onBattleChip)
	{
		var (style, items) = onBattleChip.GetHeroData();
		battleField.OnDraggedHeroToBattleField(style, items);
	}
}
