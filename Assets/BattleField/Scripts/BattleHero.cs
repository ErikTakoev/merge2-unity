using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public class BattleHero : MonoBehaviour
    {
        [SerializeReference]
        Character character;

        public void SetStyle(BattleHeroStyle style)
        {
            if (character == null)
            {
                Debug.LogError("BattleHero: character is empty");
                return;
            }
            character.SetBody(character.SpriteCollection.Hair[style.HairIndex], BodyPart.Hair, style.HairColor);
            character.SetBody(character.SpriteCollection.Eyebrows[style.EyebrowsIndex], BodyPart.Eyebrows);
            character.SetBody(character.SpriteCollection.Eyes[style.EyesIndex], BodyPart.Eyes, style.EyesColor);
            character.SetBody(character.SpriteCollection.Mouth[style.MouthIndex], BodyPart.Mouth);
        }

        public void SetEquipments(List<EquipmentItem> items)
        {
            foreach (var item in items)
            {
                BattleCharacterUtils.EquipItem(character, item);
            }
        }
    }
}
