using System;
using System.Collections;
using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using DG.Tweening;
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

        IBattleUnitStrategy strategy;
        public Dictionary<EquipmentPart, EquipmentItem> Items { get; private set; }

        List<BattleCell> path;
        public bool IsMoving { get { return path != null; } }
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

        public bool IsHero { get; set; }

        public bool LogEnable = true;

        public Collider2D Collider { get; private set; }

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
            Items = new Dictionary<EquipmentPart, EquipmentItem>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    Items.Add(item.EquipmentPart, item);
                }
            }
            foreach (var item in items)
            {
                BattleCharacterUtils.EquipItem(character, item);
            }
        }

        void Start()
        {
            strategy = new BattleUnitStrategy(this);
            Collider = GetComponent<Collider2D>();
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

        public void MoveTo(List<BattleCell> newPath)
        {
            path = newPath;
            if (LogEnable)
            {
                Debug.Log($"{name} MoveTo for:{name}, path:{path.Count}");
            }
            
            if (path.Count > 0)
            {
                if (NextCell == Cell)
                {
                    character.SetState(CharacterState.Run);
                    MoveToNextCell();
                }
            }
        }
        public void MoveStop()
        {
            if (LogEnable)
            {
                Debug.Log($"{name} MoveStop for:{name}");
            }
            path = null;
        }

        public void AddAttacker(BattleHero unit)
        {
            strategy.AddAttacker(unit);
        }

        

        

        void Update()
        {
            cooldownToReadyLeft += Time.deltaTime;
            strategy.Update();
            
        }

        public bool MoveToNextCell()
        {
            if (path == null || path.Count == 0)
            {
                return false;
            }
            var cell = path[0];
            path.RemoveAt(0);
            if (cell == Cell)
            {
                cell = path[0];
                path.RemoveAt(0);
            }
            if (!cell.IsAvailableCell())
            {
                MoveStop();
                return false;
            }
            NextCell = cell;
            Turn(NextCell.WorldPosition);
            return true;
        }

        public void Turn(Vector3 target)
        {
            var localScale = character.transform.localScale;
            var direction = target.x - transform.position.x;

            character.transform.localScale = new Vector3(Mathf.Sign(direction) * localScale.y, localScale.y, 1);
        }


        
    }
}
