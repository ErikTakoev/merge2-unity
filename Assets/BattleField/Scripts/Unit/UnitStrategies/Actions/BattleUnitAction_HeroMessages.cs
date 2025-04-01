using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace BattleField
{
	public class BattleUnitAction_HeroMessages : BattleUnitAction
	{
		BattleUnitMover mover;
		[Inject] CloudMessage[] cloudMessagePrefabs;
		struct HeroMessage
		{
			public HeroMessage(string message, int indexCloudPrefab)
			{
				this.message = message;
				this.indexCloudPrefab = indexCloudPrefab;
			}

			public string message;
			public int indexCloudPrefab;
		}

		HeroMessage[] messages;
		float nextMessageTime;
		int messageIndex;

		bool firstSkip;


		public BattleUnitAction_HeroMessages(BattleUnitAbstractStrategy strategy)
			: base(strategy)
		{
			mover = strategy.Mover;
			messages = new HeroMessage[]
			{
				new HeroMessage("Привіт, друже!", 0),
				new HeroMessage("Мені потрібна\nтвоя допомога!", 0),
				new HeroMessage("  Через хвилину, тут   \nбуде повно орків", 0),
				new HeroMessage("  Треба підготуватися!  ", 0),

				new HeroMessage("  Головна будівля\n допоможе тобі з цим", 0),
				new HeroMessage("Генеруй та об'єднуй чіпи \nщоб стати сильнішим", 0),

				new HeroMessage("  Екіпіруй героїв\n по повній!", 0),
				new HeroMessage("  Бо голожопі \n   живуть не довго", 0),
				new HeroMessage("А ще!\nЗРОБИ РЕПОСТ!", 0),
				new HeroMessage("Бо мій автор\nвсе ще безробітний!", 0),
			};
		}


		public override bool Action()
		{
			if (!firstSkip || mover.IsMoving)
			{
				firstSkip = true;
				return false;
			}

			if (nextMessageTime < Time.time && messageIndex != messages.Length)
			{
				nextMessageTime = Time.time + 5f;
				var message = messages[messageIndex];
				var prefab = cloudMessagePrefabs[message.indexCloudPrefab];
				GameObject gameObject = GameObject.Instantiate(prefab.gameObject, Unit.transform);
				var cloudMessage = gameObject.GetComponent<CloudMessage>();
				cloudMessage.Init(message.message, 5f);
				messageIndex++;
			}

			return false;
		}
	}
}
