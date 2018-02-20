using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipProject
{
	public class MissileLoadingEffect : MonoBehaviour
	{
		private Animator animator;
		public bool isReady=true;
		private void Start()
		{
			animator = GetComponent<Animator>();
		}
		public void LoadMissile()
		{
			animator.SetTrigger("Load");
		}
		public void LaunchMissile()
		{
			animator.SetTrigger("Launch");
		}

		public void SetReady()
		{
			isReady = true;
		}

		public void SetUnready()
		{
			isReady = false;
		}

	}
}
