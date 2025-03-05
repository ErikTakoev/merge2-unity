using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.Collections;
using Assets.HeroEditor.InventorySystem.Scripts;
using Assets.HeroEditor.InventorySystem.Scripts.Data;
using HeroEditor.Common;
using UnityEngine;

public class HeroItemIndicator : MonoBehaviour
{
    [SerializeField]
    string itemId;
    [SerializeReference]
    SpriteRenderer spriteRenderer;

    public string GetItemId()
    {
        return spriteRenderer.sprite.name;
    }

    private void OnValidate()
    {
        if (!spriteRenderer)
        {
            Debug.LogError("ChipIconController: spriteRenderer is empty");
            return;
        }
        itemId = GetItemId();
    }
}
