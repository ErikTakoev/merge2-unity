using UnityEngine;
using Merge2;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Unity.VisualScripting;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using Assets.HeroEditor.Common.Scripts.Common;
using static HeroWeaponsData;
using System.Collections.Generic;
public class HeroVisual : MonoBehaviour
{
    [SerializeReference]
    ChipContainer chipPerson;

    [SerializeField]
    HeroData heroData;

    [SerializeReference]
    Character character;

    private void Start()
    {
        if (!chipPerson)
        {
            Debug.LogError("HeroChipController: chipPerson is empty");
            return;
        }
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

        RandomCharacter();

        chipPerson.OnFillContainer += OnFillContainer;
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

    private void OnFillContainer(Chip chip, bool isFull)
    {
        Debug.Log($"HeroChipController: OnFillContainer: {chip.ToString()}");

        ChipData chipData = chip.Data;

        Equip(chipData);
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
        character.Equip(itemSprite, item.EquipmentPart);
    }

}
