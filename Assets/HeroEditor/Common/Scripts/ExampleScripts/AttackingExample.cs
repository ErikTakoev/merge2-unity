﻿using System;
using Assets.HeroEditor.Common.Scripts.CharacterScripts;
using Assets.HeroEditor.Common.Scripts.CharacterScripts.Firearms;
using Assets.HeroEditor.Common.Scripts.CharacterScripts.Firearms.Enums;
using HeroEditor.Common.Enums;
using UnityEngine;

namespace Assets.HeroEditor.Common.Scripts.ExampleScripts
{
	/// <summary>
	/// Rotates arms and passes input events to child components like FirearmFire and BowExample.
	/// </summary>
	public class AttackingExample : MonoBehaviour
	{
		public Character Character;
		public BowExample BowExample;
		public Firearm Firearm;
		public Transform ArmL;
		public Transform ArmR;
		public KeyCode FireButton;
		public KeyCode ReloadButton;
		[Header("Check to disable arm auto rotation.")]
		public bool FixedArm;

		public void Start()
		{
			if ((Character.WeaponType == WeaponType.Firearm1H || Character.WeaponType == WeaponType.Firearm2H) && Firearm.Params.Type == FirearmType.Unknown)
			{
				throw new Exception("Firearm params not set.");
			}
		}

		public void Update()
		{
			if (Character.Animator.GetInteger("State") >= (int)CharacterState.DeathB) return;

			switch (Character.WeaponType)
			{
				case WeaponType.Melee1H:
				case WeaponType.Melee2H:
				case WeaponType.MeleePaired:
					if (Input.GetKeyDown(FireButton))
					{
						Character.Slash();
					}
					break;
				case WeaponType.Bow:
					BowExample.ChargeButtonDown = Input.GetKeyDown(FireButton);
					BowExample.ChargeButtonUp = Input.GetKeyUp(FireButton);
					break;
				case WeaponType.Firearm1H:
				case WeaponType.Firearm2H:
					Firearm.Fire.FireButtonDown = Input.GetKeyDown(FireButton);
					Firearm.Fire.FireButtonPressed = Input.GetKey(FireButton);
					Firearm.Fire.FireButtonUp = Input.GetKeyUp(FireButton);
					Firearm.Reload.ReloadButtonDown = Input.GetKeyDown(ReloadButton);
					break;
				case WeaponType.Supplies:
					if (Input.GetKeyDown(FireButton))
					{
						Character.Animator.Play(Time.frameCount % 2 == 0 ? "UseSupply" : "ThrowSupply", 0); // Play animation randomly.
					}
					break;
			}

			if (Input.GetKeyDown(FireButton))
			{
				Character.GetReady();
			}
		}

		/// <summary>
		/// Called each frame update, weapon to mouse rotation example.
		/// </summary>
		public void LateUpdate()
		{
			switch (Character.GetState())
			{
				case CharacterState.DeathB:
				case CharacterState.DeathF:
					return;
			}

			Transform arm;
			Transform weapon;

			switch (Character.WeaponType)
			{
				case WeaponType.Bow:
					arm = ArmL;
					weapon = Character.BowRenderers[3].transform;
					break;
				case WeaponType.Firearm1H:
				case WeaponType.Firearm2H:
					arm = ArmR;
					weapon = Firearm.FireTransform;
					break;
				default:
					return;
			}

			if (Character.IsReady())
			{
				RotateArm(arm, weapon, FixedArm ? arm.position + 1000 * Vector3.right : Camera.main.ScreenToWorldPoint(Input.mousePosition), -40, 40);
			}
		}

		public float AngleToTarget;
		public float AngleToArm;

		/// <summary>
		/// Selected arm to position (world space) rotation, with limits.
		/// </summary>
		public void RotateArm(Transform arm, Transform weapon, Vector2 target, float angleMin, float angleMax) // TODO: Very hard to understand logic.
		{
			target = arm.transform.InverseTransformPoint(target);

			var angleToTarget = Vector2.SignedAngle(Vector2.right, target);
			var angleToArm = Vector2.SignedAngle(weapon.right, arm.transform.right) * Math.Sign(weapon.lossyScale.x);
			var fix = weapon.InverseTransformPoint(arm.transform.position).y / target.magnitude;

			AngleToTarget = angleToTarget;
			AngleToArm = angleToArm;

			if (fix < -1) fix = -1;
			else if (fix > 1) fix = 1;

			var angleFix = Mathf.Asin(fix) * Mathf.Rad2Deg;
			var angle = angleToTarget + angleFix + arm.transform.localEulerAngles.z;

			angle = NormalizeAngle(angle);

			if (angle > angleMax)
			{
				angle = angleMax;
			}
			else if (angle < angleMin)
			{
				angle = angleMin;
			}

			if (float.IsNaN(angle))
			{
				Debug.LogWarning(angle);
			}

			arm.transform.localEulerAngles = new Vector3(0, 0, angle + angleToArm);
		}

		private static float NormalizeAngle(float angle)
		{
			while (angle > 180) angle -= 360;
			while (angle < -180) angle += 360;

			return angle;
		}
	}
}
