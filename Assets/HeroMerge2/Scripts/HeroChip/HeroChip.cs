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
        int[] filterHair = new []{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 17 };
        int[] filterEyebrows = new []{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 21, 22, 23, 25 };
        int[] filterEye = new []{ 3, 5, 13, 15, 16, 17, 18, 23, 24, 25, 28 };
        int[] filterMouth = new []{ 1, 2, 3, 4, 5, 9, 10, 24, 27, 28 };
        style = new BattleHeroStyle();
        style.HairIndex = filterHair[Random.Range(0, filterHair.Length)];
        style.HairColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        style.EyebrowsIndex = filterEyebrows[Random.Range(0, filterEyebrows.Length)];
        style.EyesIndex = filterEye[Random.Range(0, filterEye.Length)];
        style.EyesColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        style.MouthIndex = filterMouth[Random.Range(0, filterMouth.Length)];

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
                    RemoveSlot(EquipmentPart.Bow);
                    break;
                case "Shield":
                    RemoveSlot(EquipmentPart.MeleeWeapon1H);
                    RemoveSlot(EquipmentPart.Bow);
                    break;
                case "Bow":
                    RemoveSlot(EquipmentPart.MeleeWeapon1H);
                    RemoveSlot(EquipmentPart.MeleeWeapon1H);
                    RemoveSlot(EquipmentPart.Shield);
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
