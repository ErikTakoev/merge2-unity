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
        public BattleCell Cell { get; private set; }
        public BattleCell NextCell  { get; private set; }

        IBattleUnitStrategy strategy;
        List<EquipmentItem> items;

        List<BattleCell> path;
        public bool IsMoving { get { return path != null; } }
        public bool IsAttacking { get; private set; }
        public bool IsAttackReady { get; private set; }
        public bool IsDodgeRolling { get; private set; }

        public bool IsHero { get; set; }

        public bool LogEnable;

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
            transform.position = cell.WorldPosition;
            Cell.SetTemporaryBusy(true);
        }

        public void MoveTo(List<BattleCell> path)
        {
            Debug.Log($"{name} MoveTo:{path[path.Count - 1].CellPos}");
            this.path = path;
            if (path.Count > 0)
            {
                character.SetState(CharacterState.Run);
                MoveToNextCell();
            }
        }
        public void MoveStop()
        {
            path = null;
            NextCell = null;
            character.SetState(CharacterState.Idle);
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
            if (NextCell != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, NextCell.WorldPosition, 1 * Time.deltaTime);
                if (Vector3.Distance( transform.position, NextCell.WorldPosition) < 0.02f)
                {
                    if (Cell != null)
                    {
                        Cell.SetTemporaryBusy(false);
                    }
                    Cell = NextCell;
                    if (path.Count > 0)
                    {
                        Cell.SetTemporaryBusy(true);
                        MoveToNextCell();
                    }
                    else
                    {
                        Cell.SetTemporaryBusy(true);
                        character.SetState(CharacterState.Idle);
                        path = null;
                        NextCell = null;
                    }
                }
            }
        }

        private void MoveToNextCell()
        {
            var cell = path[0];
            path.RemoveAt(0);
            NextCell = cell;
            Turn(NextCell.WorldPosition);
        }

        private void Turn(Vector3 target)
        {
            var localScale = character.transform.localScale;
            var direction = target.x - transform.position.x;

            character.transform.localScale = new Vector3(Mathf.Sign(direction) * localScale.y, localScale.y, 1);
        }

    }
}
