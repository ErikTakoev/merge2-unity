using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace BattleField
{
    public class BattleHero : MonoBehaviour
    {
        [SerializeReference]
        Character character;
        BattleCell nextCell;
        public BattleCell Cell  { get; private set; }
        public BattleCell NextCell { 
            get { return nextCell; }
            private set
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
        List<EquipmentItem> items;

        List<BattleCell> path;
        public bool IsMoving { get { return path != null; } }
        public bool IsAttacking { get; private set; }
        public bool IsAttackReady { get; private set; }
        public bool IsDodgeRolling { get; private set; }

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
            foreach (var item in items)
            {
                BattleCharacterUtils.EquipItem(character, item);
            }
        }

        void Start()
        {
            strategy = new BattleUnitStrategy(this, items);
            Collider = GetComponent<Collider2D>();
            IsAttackReady = true;
        }

        public void SetCell(BattleCell cell)
        {
            Cell = cell;
            NextCell = cell;
            transform.position = cell.WorldPosition;
        }

        public void MoveTo(List<BattleCell> newPath)
        {
            if (path != null)
            {
                if (path.Count > 0)
                {
                    //path[path.Count - 1].IsReserved = false;
                }
                else
                {
                    //NextCell.IsReserved = false;
                }
            }
            path = newPath;
            if (LogEnable)
            {
                Debug.Log($"{name} MoveTo for:{name}, path:{path.Count}");
            }
            
            if (path.Count > 0)
            {
                //path[path.Count - 1].IsReserved = true;
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
            if (path != null && path.Count > 0)
            {
                //path[path.Count - 1].IsReserved = false;
            }
            path = null;
        }

        public void AddAttacker(BattleHero unit)
        {
            strategy.AddAttacker(unit);
        }

        public void Attack(BattleHero target)
        {
            if (LogEnable)
            {
                Debug.Log($"{name} Attack target:{target.name}");
            }
            Turn(target.transform.position);
            IsAttackReady = false;
            StartCoroutine(AttackCoroutine(target));
        }

        private IEnumerator AttackCoroutine(BattleHero target)
        {
            yield return new WaitUntil(() => !IsDodgeRolling);
            IsAttacking = true;
            target.AddAttacker(this);
            if (UnityEngine.Random.value > 0.5)
            {
                character.Slash();
            }
            else
            {
                character.Jab();
            }
            yield return new WaitForSeconds(0.4f);
            IsAttacking = false;
            yield return new WaitForSeconds(UnityEngine.Random.value + 0.3f);
            IsAttackReady = true;
        }

        public void DodgeRoll(BattleHero target)
        {
            if (LogEnable)
            {
                Debug.Log($"{name} DodgeRoll target:{target.name}");
            }
            StartCoroutine(DodgeRollCoroutine(target));
        }

        private IEnumerator DodgeRollCoroutine(BattleHero target)
        {
            IsDodgeRolling = true;
            character.Animator.SetTrigger("ShieldDefense");
            yield return new WaitForSeconds(1.0f);
            IsDodgeRolling = false;
        }

        void Update()
        {
            strategy.Update();
            if (NextCell != Cell)
            {
                transform.position = Vector3.MoveTowards(transform.position, NextCell.WorldPosition, 0.3f * Time.deltaTime);
                if (Vector3.Distance( transform.position, NextCell.WorldPosition) < 0.02f)
                {
                    Cell = NextCell;

                    if (!MoveToNextCell())
                    {
                        //Cell.IsReserved = false;
                        character.SetState(CharacterState.Idle);
                        path = null;
                        NextCell = Cell;
                    }
                }
            }
        }

        private bool MoveToNextCell()
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

        private void Turn(Vector3 target)
        {
            var localScale = character.transform.localScale;
            var direction = target.x - transform.position.x;

            character.transform.localScale = new Vector3(Mathf.Sign(direction) * localScale.y, localScale.y, 1);
        }

    }
}
