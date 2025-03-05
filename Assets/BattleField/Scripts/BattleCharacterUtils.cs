using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Data;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public static class BattleCharacterUtils
    {
        public static bool EquipItem(Character character, EquipmentItem item)
        {
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
                return false;
            }

            var itemSprite = collection.Find(i => i.Name == item.ItemId);
            if (itemSprite == null)
            {
                Debug.Log($"Not find sprite for itemId: {item.ItemId}");
                return false;
            }

            var equipmentPart = item.EquipmentPart;
            if (equipmentPart == EquipmentPart.MeleeWeapon1H && character.PrimaryMeleeWeapon != null)
            {
                equipmentPart = EquipmentPart.MeleeWeaponPaired;
                character.Equip(itemSprite, equipmentPart);
            }
            else
            {
                character.Equip(itemSprite, equipmentPart);
            }
            return true;
        }
    }
}
