using System;
using BattleField;
using HeroEditor.Common.Enums;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(HeroDataBase))]
public class HeroDataBaseEditor : Editor
{
	public override void OnInspectorGUI()
	{
		HeroDataBase dataBase = (HeroDataBase)target;

		if (GUILayout.Button("Refresh Item Data"))
		{
			RefreshItemData(dataBase);
		}
		DrawDefaultInspector();
	}

	void RefreshItemData(HeroDataBase dataBase)
	{
		foreach (var data in dataBase.Armors)
		{
			AddInfo(data, EquipmentPart.Armor);
		}
		foreach (var data in dataBase.Helmets)
		{
			AddInfo(data, EquipmentPart.Helmet);
		}
		foreach (var data in dataBase.Shields)
		{
			AddInfo(data, EquipmentPart.Shield);
		}
		foreach (var data in dataBase.Weapons)
		{
			AddInfo(data, EquipmentPart.MeleeWeapon1H);
		}
		foreach (var data in dataBase.Bows)
		{
			AddInfo(data, EquipmentPart.Bow);
		}
		EditorUtility.SetDirty(dataBase);
		AssetDatabase.SaveAssets();
	}

	void AddInfo(HeroDataBase.Data data, EquipmentPart part)
	{
		// Шукаєм відповідний EquipmentItem для ChipData
		if (data.ChipData != null && data.Item == null)
		{
			string[] guids = AssetDatabase.FindAssets($"{data.ChipData.name} t:EquipmentItem");
			if (guids.Length > 0)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[0]);
				data.Item = AssetDatabase.LoadAssetAtPath<EquipmentItem>(path);
			}
		}

		// Якщо EquipmentItem не знайдено, то створюємо його
		if (data.Item == null)
		{
			string path = $"Assets/BattleField/Data/{part}Item/{data.ChipData.name}.asset";
			Type type = null;
			switch (part)
			{
				case EquipmentPart.Armor:
					type = typeof(EquipmentArmorItem);
					break;
				case EquipmentPart.Helmet:
					type = typeof(EquipmentHelmetItem);
					break;
				case EquipmentPart.Shield:
					type = typeof(EquipmentShieldItem);
					break;
				case EquipmentPart.MeleeWeapon1H:
				case EquipmentPart.Bow:
					type = typeof(EquipmentWeaponItem);
					break;
				default:
					Debug.LogError($"AddInfo: Not found type for EquipmentPart: {part}");
					return;
			}

			var item = ScriptableObject.CreateInstance(type);
			AssetDatabase.CreateAsset(item, path);

			data.Item = item as EquipmentItem;
		}

		// Якщо EquipmentItem знайдено, то додаємо йому ItemId
		if (data.Item != null)
		{
			var chipData = data.ChipData;
			if (chipData.PrefabLink == null)
			{
				Debug.LogError($"AddInfo: In HeroDataBase not PrefabLink: {data.ChipData.name}");
				return;
			}
			if (data.ChipData.PrefabLink.GetComponent<HeroItemIndicator>() is var indicator)
			{
				data.Item.ItemId = indicator.GetItemId();
			}

			data.Item.EquipmentPart = part;
			EditorUtility.SetDirty(data.Item);
		}

	}
}
