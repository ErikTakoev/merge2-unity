using System.Collections.Generic;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using DG.Tweening;
using UnityEngine;

namespace BattleField
{
    public class BattleUnitAction_MoveKeepDistance : BattleUnitAction
    {
        
        bool isPathfinding;
        BattleCell movingToCell;

        BattleUnitMover mover;

        bool isJumping;
        const int CellJumpCount = 2;
        const float CooldownToReady = 5f;
        float cooldownToReadyTimeLeft = CooldownToReady;

        
        public BattleUnitAction_MoveKeepDistance(IBattleUnitStrategy strategy)
            : base (strategy)
        {
            mover = strategy.Mover;
        }


        public override bool Action()
        {
            if (isJumping)
            {
                return true;
            }
            bool result = false;

            if (Jump())
            {
                return true;
            }
            
            var distance = Pathfinding.GetManhattanDistance(Target.NextCell, Unit.NextCell);
            if (distance <= 5)
            {
                strategy.Mover.MoveStop();
                return true;
            }


            return result;
        }

        private bool Jump()
        {
            if ( cooldownToReadyTimeLeft > 0)
            {
                return false;
            }

            var followers = strategy.Followers;
            int minDistance = int.MaxValue;

            BattleHero nearestFollower = null;
            for (int i = 0; i < followers.Count; i++)
            {
                var follower = followers[i];
                var distance = Pathfinding.GetManhattanDistance(follower.NextCell, Unit.NextCell);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestFollower = follower;
                }
            }
            if (nearestFollower == null)
            {
                return false;
            }
            var diff = nearestFollower.NextCell.CellPos - Unit.NextCell.CellPos;
            if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1)
            {
                BattleCell cell = null;

                diff.x = diff.x * -1;
                
                int randomDirection = Random.value > 0.5f ? 1 : -1;
                int maxDistance = 0;
                diff.y = randomDirection;
                FindJumpCell(diff, ref maxDistance, ref cell);

                if (maxDistance != CellJumpCount)
                {
                    diff.y *= -1;
                    FindJumpCell(diff, ref maxDistance, ref cell);
                }
                
                if (maxDistance != CellJumpCount)
                {
                    diff.x *= -1;
                    FindJumpCell(diff, ref maxDistance, ref cell);
                }

                if (maxDistance != CellJumpCount)
                {
                    diff.y *= -1;
                    FindJumpCell(diff, ref maxDistance, ref cell);
                }

                if (cell != null)
                {
                    cooldownToReadyTimeLeft = CooldownToReady;
                    isJumping = true;
                    strategy.Mover.MoveStop();
                    Unit.SetCell(cell, false);
                    Unit.transform.DOMove(cell.WorldPosition, 0.5f).SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            isJumping = false;
                        });
                    return true;
                }
            }

            return false;
        }

        public override void Update()
        {
            cooldownToReadyTimeLeft -= Time.deltaTime;
        }

        private void FindJumpCell(Vector2Int diff, ref int maxDistance, ref BattleCell cell)
        {
            for (int i = 1; i <= CellJumpCount; i++)
            {
                Vector2Int cellPos = Unit.NextCell.CellPos - diff * i;
                var tmpCell = BattleField.Instance.GetCell(cellPos.x, cellPos.y);
                if (tmpCell == null || !tmpCell.IsAvailableCell())
                {
                    break;
                }
                maxDistance = i;
                cell = tmpCell;
            }
        }
    }
}