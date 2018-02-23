using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ShipProject
{
	public class ShipHealthUIBlock : MonoBehaviour
	{
		public Block block;
		public BlockHealth health;
		public Material material;
		public ShipHealthUI ui;
		public Gradient ColorByHealth, ColorFlash;
		private bool OnFlash = false;
		private float FlashTime = 0.2f;
		private float timer = 0f;
		public float hp;

		public void Start()
		{
			health.OnAttack += Flash;
			material = GetComponent<Image>().material;
			ColorByHealth = ui.ColorByHealth;
			ColorFlash = ui.ColorFlash;
			block = health.GetComponent<Block>();
		}

		public void Update()
		{
			if (!OnFlash)
				material.SetColor("_Color", ColorByHealth.Evaluate(health.healthRate));
			else
			{
				timer += Time.deltaTime;
				material.SetColor("_Color", ColorFlash.Evaluate(timer / FlashTime));
				if (timer >= FlashTime)
				{
					OnFlash = false;
					timer = 0f;
				}
			}
			hp = health.healthRate;
			if (hp == 0 || block == null)
			{
				Destroy(gameObject);
			}
		}

		public void Flash(IDamage damage,float health)
		{
			OnFlash = true;
		}
	}
}
