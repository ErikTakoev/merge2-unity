using System;
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

        public Character Character { get { return character; } }
        BattleCell nextCell;
        public BattleCell Cell  { get; set; }
        public BattleCell NextCell { 
            get { return nextCell; }
            set
            {
                if (nextCell != null)
                {
                    nextCell.SetTemporaryBusy(false);
                }
                nextCell = value;
                if (nextCell != null)
                {
                    nextCell.SetTemporaryBusy(true);
                }
            }
        }

        public IBattleUnitStrategy Strategy { get; private set; }
        public Dictionary<EquipmentPart, EquipmentItem> Items { get; private set; }

        public bool IsMoving { get { return Strategy.Mover.IsMoving; } }
        public bool IsAttacking { get; set; }
        public bool IsStunning { get; set; }

        const float CooldownToReady = 0.5f;
        float cooldownToReadyLeft = 0.0f;

        public bool NeedTimeToReady
        {
            get
            {
                return cooldownToReadyLeft < CooldownToReady;
            }
            set
            {
                if (value) cooldownToReadyLeft = 0f; 
                else cooldownToReadyLeft = CooldownToReady;
            }
        }

        public bool IsAttackReady { get { return !IsAttacking && !IsStunning && !NeedTimeToReady; } }
        public bool IsDodgeRolling { get; set; }

        public bool IsHero { get; private set; }

        public bool LogEnable = true;

        void SetStyle(BattleHeroStyle? styleValue)
        {
            if (character == null)
            {
                Debug.LogError("BattleHero: character is null");
                return;
            }
            if (styleValue == null)
            {
                Debug.LogWarning("BattleHero: style is null");
                return;
            }
            var style = styleValue.Value;
            character.SetBody(character.SpriteCollection.Hair[style.HairIndex], BodyPart.Hair, style.HairColor);
            character.SetBody(character.SpriteCollection.Eyebrows[style.EyebrowsIndex], BodyPart.Eyebrows);
            character.SetBody(character.SpriteCollection.Eyes[style.EyesIndex], BodyPart.Eyes, style.EyesColor);
            character.SetBody(character.SpriteCollection.Mouth[style.MouthIndex], BodyPart.Mouth);
        }

        void SetEquipments(List<EquipmentItem> items)
        {
            Items = new Dictionary<EquipmentPart, EquipmentItem>();
            if (items == null)
            {
                Debug.LogWarning("BattleHero: items is null");
                return;
            }
            foreach (var item in items)
            {
                Items.Add(item.EquipmentPart, item);
            }
            foreach (var item in items)
            {
                BattleCharacterUtils.EquipItem(character, item);
            }
        }

        public void Init(BattleHeroStyle? style, List<EquipmentItem> items, IBattleUnitStrategy strategy, BattleCell cell, bool isHero)
        {
            SetStyle(style);
            SetEquipments(items);

            Strategy = strategy;
            SetCell(cell);
            IsHero = isHero;
        }


        public void SetCell(BattleCell cell, bool changePos = true)
        {
            Cell = cell;
            NextCell = cell;
            if (changePos)
            {
                transform.position = cell.WorldPosition;
            }
        }

        

        public void AddAttacker(BattleHero unit)
        {
            Strategy.AddAttacker(unit);
        }

        

        

        void Update()
        {
            cooldownToReadyLeft += Time.deltaTime;
            Strategy.Update();
        }

        void LateUpdate()
        {
            Strategy.LateUpdate();
        }



        public void Turn(Vector3 target)
        {
            var localScale = character.transform.localScale;
            var direction = target.x - transform.position.x;

            character.transform.localScale = new Vector3(Mathf.Sign(direction) * localScale.y, localScale.y, 1);
        }
    }
}
