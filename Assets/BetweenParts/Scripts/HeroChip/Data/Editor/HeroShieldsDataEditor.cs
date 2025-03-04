using UnityEditor;
using UnityEngine;

using HeroEditor.Common.Enums;

[CustomEditor(typeof(HeroShieldsData))]
public class HeroShieldsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeroShieldsData weaponsData = (HeroShieldsData)target;

        if (GUILayout.Button("Refrash Item Data"))
        {
            RefrashItemData(weaponsData);
        }
        DrawDefaultInspector();
    }

    void RefrashItemData(HeroShieldsData data)
    {
        foreach(var shild in data.shields)
        {
            var itemIndificator = shild.ChipData.PrefabLink.GetComponent<HeroItemIndicator>();
            if (itemIndificator == null)
            {
                Debug.LogError($"ChipData: {shild.ChipData.PrefabLink.name} is not component HeroItemIndicator");
                continue;
            }
            shild.ItemId = itemIndificator.GetItemId();
            shild.EquipmentPart = EquipmentPart.Shield;
        }
    }
}
