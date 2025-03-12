using System;
using UnityEngine;

namespace BattleField
{
    [System.Serializable]
    public class BattleCell
    {
        public Vector2Int CellPos; // Координати клітинки
        public bool IsWalkable; // Чи є клітинка прохідною
        public float Width; // Ширина клітинки
        public float Height; // Висота клітинки
        public Vector3 WorldPosition; // Позиція клітинки в мирових координатах
        public bool IsTemporaryBusy; // Клітинка тимчасово зайнята

        public BattleCell(int x, int y, bool isWalkable, float width, float height, Vector3 worldPosition)
        {
            CellPos = new Vector2Int(x, y);
            this.IsWalkable = isWalkable;
            this.Width = width;
            this.Height = height;
            this.WorldPosition = worldPosition;
        }

        public bool IsAvailableCell()
        {
            return IsWalkable && !IsTemporaryBusy;
        }

        public void SetTemporaryBusy(bool value)
        {
            IsTemporaryBusy = value;
        }
    }
}