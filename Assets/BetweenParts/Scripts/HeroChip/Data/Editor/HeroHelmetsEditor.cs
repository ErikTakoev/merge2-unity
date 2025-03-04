using UnityEditor;
using UnityEngine;

using HeroEditor.Common.Enums;

[CustomEditor(typeof(HeroHelmetsData))]
public class HeroHelmetsDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HeroHelmetsData helmetsData = (HeroHelmetsData)target;

        if (GUILayout.Button("Refrash Item Data"))
        {
            RefrashItemData(helmetsData);
        }
        DrawDefaultInspector();
    }

    void RefrashItemData(HeroHelmetsData weaponsData)
    {
        foreach(var helmetData in weaponsData.helmets)
        {
            var itemIndificator = helmetData.ChipData.PrefabLink.GetComponent<HeroItemIndicator>();
            if (itemIndificator == null)
            {
                Debug.LogError($"ChipData: {helmetData.ChipData.PrefabLink.name} is not component HeroItemIndicator");
                continue;
            }
            helmetData.ItemId = itemIndificator.GetItemId();
            helmetData.EquipmentPart = EquipmentPart.Helmet;
        }
    }
}
