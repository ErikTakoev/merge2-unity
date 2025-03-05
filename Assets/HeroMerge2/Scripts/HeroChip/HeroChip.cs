using UnityEngine;
using Merge2;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using Assets.HeroEditor.Common.Scripts.Common;
using System.Collections.Generic;
using BattleField;

public class HeroChip : ChipContainer
{
    [SerializeField]
    HeroDataBase heroData;

    [SerializeReference]
    Character character;

    BattleHeroStyle style;
    List<EquipmentItem> equipmentItems;

    private void Start()
    {
        if (!heroData)
        {
            Debug.LogError("HeroChipController: heroData is empty");
            return;
        }
        if (!character)
        {
            Debug.LogError("HeroChipController: character is empty");
            return;
        }
        equipmentItems = new List<EquipmentItem>();

        RandomCharacter();

        OnFillContainer += OnFillContainerEvent;
    }

    private void RandomCharacter()
    {
        character.ResetEquipment();
        SetCharacterStyle();

        if (Random.value > 0.5)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void SetCharacterStyle()
    {
        style = new BattleHeroStyle();
        style.HairIndex = Random.Range(0, character.SpriteCollection.Hair.Count);
        style.HairColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        style.EyebrowsIndex = Random.Range(0, character.SpriteCollection.Eyebrows.Count);
        style.EyesIndex = Random.Range(0, character.SpriteCollection.Eyes.Count);
        style.EyesColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        style.MouthIndex = Random.Range(0, character.SpriteCollection.Mouth.Count);

        character.SetBody(character.SpriteCollection.Hair[style.HairIndex], BodyPart.Hair, style.HairColor);
        character.SetBody(character.SpriteCollection.Eyebrows[style.EyebrowsIndex], BodyPart.Eyebrows);
        character.SetBody(character.SpriteCollection.Eyes[style.EyesIndex], BodyPart.Eyes, style.EyesColor);
        character.SetBody(character.SpriteCollection.Mouth[style.MouthIndex], BodyPart.Mouth);
    }

    private void OnFillContainerEvent(Chip chip, bool isFull)
    {
        Debug.Log($"HeroChipController: OnFillContainer: {chip.ToString()}");

        ChipData chipData = chip.Data;

        Equip(chipData);
    }
    
    public override bool ChipSuitableForContainer(Chip chip)
    {
        bool result = base.ChipSuitableForContainer(chip);
        
        /// ��� ������ - ������ �� ���                  // + ���, ������
        /// ��� � ������ - ������ �� ������ ������      // + ���, ������
                                                        // ��� - ������ �� ��� � ��� ������
                                                        // ��������� ������ - ������ �� ���, ��� ������
        if (result)
        {

            switch (chip.Data.Type)
            {
                case "Weapon":
                    if (character.SecondaryMeleeWeapon != null)
                    {
                        RemoveSlot(EquipmentPart.Shield);
                    }
                    break;
                case "Shield":
                    RemoveSlot(EquipmentPart.MeleeWeapon1H);
                    break;
            }
        }
        
        return result;
    }

    bool RemoveSlot(EquipmentPart part)
    {
        string partName = part.ToString();
        if (part == EquipmentPart.MeleeWeapon1H)
        {
            partName = "Weapon";
        }
        foreach (var slot in containers)
        {
            if (slot.Key.TypeOrId == partName)
            {
                containers.Remove(slot.Key);
                return true;
            }
        }
        return false;
    }

    void Equip(ChipData chipData)
    {
        var asset = heroData.Find(chipData);
        if (asset == null)
        {
            Debug.LogError($"Equip: In HeroData not chipData: {chipData.name}");
            return;
        }
        var item = asset.Item;
        if (item == null)
        {
            Debug.LogError($"Equip: In HeroData not item: {chipData.name}");
            return;
        }

        if (BattleCharacterUtils.EquipItem(character, item))
        {
            equipmentItems.Add(item);
        }
    }

    public (BattleHeroStyle, List<EquipmentItem>) GetHeroData()
    {
        return (style, equipmentItems);
    }
}
