using System;
using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleUnit : MonoBehaviour
    {
        [SerializeReference]
        Character character;
        [SerializeReference]
        BattleUnitStats unitStats;

        public event Action<BattleUnit> OnUnitDeadEvent;

        public Character Character { get { return character; } }
        public BattleUnitStats Stats { get { return unitStats; } }
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

        public BattleUnitAbstractStrategy Strategy { get; private set; }

        public bool IsMoving { get { return Strategy.Mover.IsMoving; } }
        public bool IsAttacking { get; set; }
        public bool IsStunning { get; set; }
        public bool IsDead { get; private set;}

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

        Transform AnimationNode;

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
                Debug.LogWarning("BattleUnit: style is null");
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
            if (items == null)
            {
                Debug.LogWarning("BattleUnit: items is null");
                return;
            }

            foreach (var item in items)
            {
                BattleCharacterUtils.EquipItem(character, item);
            }
        }

        public void Init(BattleHeroStyle? style, List<EquipmentItem> items, BattleCell cell, bool isHero)
        {
            SetStyle(style);
            SetEquipments(items);
            AnimationNode = character.transform.parent;

            if (unitStats == null)
            {
                Debug.LogError("BattleUnit: unitStats is empty");
                return;
            }
            unitStats.Init(items);
            unitStats.OnUnitDeadEvent += OnUnitDead;

            SetCell(cell);
            IsHero = isHero;
            Strategy = GetComponent<BattleUnitAbstractStrategy>();
            if (Strategy == null)
            {
                Strategy = GetStrategy();
            }
            Strategy.Init(this);
        }

        private BattleUnitAbstractStrategy GetStrategy()
        {
            if (Character.WeaponType == WeaponType.Bow)
            {
                return gameObject.AddComponent<BattleUnitBowStrategy>();
            }
            return gameObject.AddComponent<BattleUnitShieldStrategy>();
        }

        void OnUnitDead()
        {
            IsDead = true;
            Strategy.Target = null;
            Character.SetState(CharacterState.DeathB);
            var pos = transform.position;
            pos.z += 1f;
            transform.position = pos;
            transform.DOScale(0.7f, 0.3f);
            transform.DORotate(new Vector3(0, 0, UnityEngine.Random.Range(-20, 20)), 0.3f);
            OnUnitDeadEvent?.Invoke(this);
            OnUnitDeadEvent = null;
            NextCell.SetTemporaryBusy(false);
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

        

        public void AddAttacker(BattleUnit unit)
        {
            Strategy.AddAttacker(unit);
        }

        

        

        void Update()
        {
            if (IsDead) return;

            cooldownToReadyLeft += Time.deltaTime;
        }



        public void Turn(Vector3 target)
        {
            
            var localScale = AnimationNode.localScale;
            var direction = Mathf.Sign(target.x - AnimationNode.position.x);
            if (LogEnable)
            {
                Debug.LogWarning($"Turn direction: {direction}");
            }

            AnimationNode.localScale = new Vector3(direction * localScale.y, localScale.y, 1);
        }
    }
}
