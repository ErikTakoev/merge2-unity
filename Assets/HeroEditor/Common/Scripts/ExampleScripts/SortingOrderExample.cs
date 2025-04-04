﻿using System.Collections.Generic;
using System.Linq;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;

namespace Assets.HeroEditor.Common.Scripts.ExampleScripts
{
	/// <summary>
	/// Handle character overlapping example.
	/// </summary>
	public class SortingOrderExample
	{
		/// <summary>
		/// Order characters by sorting order. // TODO: Legacy. Use SortingGroup!
		/// </summary>
		public static void OrderByIndex(Character front, Character back)
		{
			front.LayerManager.SetSortingGroupOrder(200);
			back.LayerManager.SetSortingGroupOrder(100);
		}

		/// <summary>
		/// Use it when you have a static scene with multiple characters and you want to sort them by Y.
		/// </summary>
		public void SetupSortingOrderByY(List<Character> character)
		{
			var list = character.OrderBy(i => i.transform.position.x - 1000 * i.transform.position.y).ToList();

			for (var i = 0; i < list.Count; i++)
			{
				list[i].LayerManager.SortingGroup.sortingOrder = 100 + 10 * i; // May be from -32767 to 32767!
			}
		}
	}
}
