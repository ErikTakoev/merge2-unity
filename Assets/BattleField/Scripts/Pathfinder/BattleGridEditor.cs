using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BattleField.BattleGrid))]
public class BattleGridEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		BattleField.BattleGrid battleGrid = (BattleField.BattleGrid)target;
		if (GUILayout.Button("Генерувати IsWalkable значення"))
		{
			battleGrid.GenerateIsWalkableValues();
			EditorUtility.SetDirty(battleGrid); // Позначаємо об'єкт як змінений, щоб Unity зберіг зміни
		}
	}
}
