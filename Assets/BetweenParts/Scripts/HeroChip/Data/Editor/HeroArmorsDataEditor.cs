using UnityEditor;
using UnityEngine;

using HeroEditor.Common.Enums;

[CustomEditor(typeof(HeroArmorsData))]
public class HeroArmorsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeroArmorsData armorsData = (HeroArmorsData)target;

        if (GUILayout.Button("Refrash Item Data"))
        {
            RefrashItemData(armorsData);
        }
        DrawDefaultInspector();
    }

    void RefrashItemData(HeroArmorsData weaponsData)
    {
        foreach(var armorData in weaponsData.armors)
        {
            var itemIndificator = armorData.ChipData.PrefabLink.GetComponent<HeroItemIndicator>();
            if (itemIndificator == null)
            {
                Debug.LogError($"ChipData: {armorData.ChipData.PrefabLink.name} is not component HeroItemIndicator");
                continue;
            }
            armorData.ItemId = itemIndificator.GetItemId();
            armorData.EquipmentPart = EquipmentPart.Armor;
        }
    }
}
