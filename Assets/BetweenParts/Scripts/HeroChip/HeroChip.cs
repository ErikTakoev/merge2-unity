using UnityEngine;
using Merge2;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using Assets.HeroEditor.Common.Scripts.Common;
using System.Collections.Generic;
public class HeroChip : ChipContainer
{
    [SerializeField]
    HeroData heroData;

    [SerializeReference]
    Character character;

    List<EquipmentPart> equipmentParts;

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
        equipmentParts = new List<EquipmentPart>();

        RandomCharacter();

        OnFillContainer += OnFillContainerEvent;
    }

    private void RandomCharacter()
    {
        character.ResetEquipment();

        Color RandomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1f);
        character.SetBody(character.SpriteCollection.Hair.Random(), BodyPart.Hair, RandomColor);
        character.SetBody(character.SpriteCollection.Eyebrows.Random(), BodyPart.Eyebrows);
        character.SetBody(character.SpriteCollection.Eyes.Random(), BodyPart.Eyes, RandomColor);
        character.SetBody(character.SpriteCollection.Mouth.Random(), BodyPart.Mouth);

        if (Random.value > 0.5)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
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
                    if (equipmentParts.Contains(EquipmentPart.MeleeWeaponPaired))
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
        var item = heroData.Find(chipData);
        if (item == null)
        {
            Debug.LogError($"Equip: In HeroData not chipData: {chipData.name}");
            return;
        }

        Debug.Log($"Equip: ItemId: {item.ItemId}, part: {item.EquipmentPart}");
        List<ItemSprite> collection = null;
        switch (item.EquipmentPart)
        {
            case EquipmentPart.Armor:
                collection = character.SpriteCollection.Armor;
                break;
            case EquipmentPart.MeleeWeapon1H:
                collection = character.SpriteCollection.MeleeWeapon1H;
                break;
            case EquipmentPart.Shield:
                collection = character.SpriteCollection.Shield;
                break;
            case EquipmentPart.Helmet:
                collection = character.SpriteCollection.Helmet;
                break;
        }
        if (collection == null)
        {
            Debug.LogError("Equip: not found collection list");
            return;
        }

        var itemSprite = collection.Find(i => i.Name == item.ItemId);
        if (itemSprite == null)
        {
            Debug.Log($"Not find sprite for itemId: {item.ItemId}");
            return;
        }

        var equipmentPart = item.EquipmentPart;
        if (equipmentPart == EquipmentPart.MeleeWeapon1H && equipmentParts.Contains(EquipmentPart.MeleeWeapon1H))
        {
            equipmentPart = EquipmentPart.MeleeWeaponPaired;
            character.Equip(itemSprite, equipmentPart);
        }
        else
        {
            character.Equip(itemSprite, equipmentPart);
        }
        equipmentParts.Add(equipmentPart);
    }

}
