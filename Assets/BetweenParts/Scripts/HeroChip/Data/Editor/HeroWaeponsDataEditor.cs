using UnityEditor;
using UnityEngine;

using HeroEditor.Common.Enums;

[CustomEditor(typeof(HeroWeaponsData))]
public class HeroWaeponsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeroWeaponsData weaponsData = (HeroWeaponsData)target;

        if (GUILayout.Button("Refrash Item Data"))
        {
            RefrashItemData(weaponsData);
        }
        DrawDefaultInspector();
    }

    void RefrashItemData(HeroWeaponsData weaponsData)
    {
        foreach(var weaponData in weaponsData.weapons)
        {
            var itemIndificator = weaponData.ChipData.PrefabLink.GetComponent<HeroItemIndicator>();
            if (itemIndificator == null)
            {
                Debug.LogError($"ChipData: {weaponData.ChipData.PrefabLink.name} is not component HeroItemIndicator");
                continue;
            }
            weaponData.ItemId = itemIndificator.GetItemId();
            weaponData.EquipmentPart = EquipmentPart.MeleeWeapon1H;
        }
    }
}
