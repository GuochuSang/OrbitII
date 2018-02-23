using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace ShipProject
{
	public class ShipHealthUI : SerializedMonoBehaviour
	{
		public Material UIColorLose;
		public Ship ship;
		public Vector2 RightUpPoint = new Vector2(50, 50), LeftDownPoint = new Vector2(-50, -50);
		public Vector2 Center;
		public float Scale = 300f;
		public Gradient ColorByHealth = new Gradient();
		public Gradient ColorFlash = new Gradient();
		public Sprite HealthSprite = new Sprite();

		public void AddImageAtPos(Sprite sprite, Vector2Int pos)
		{
			GameObject Imager = new GameObject("Imager");
			Imager.transform.parent = transform;
			Image image = Imager.AddComponent<Image>();
			image.material = new Material(UIColorLose);
			image.sprite = sprite;
			Vector2 position = 100 * (Vector2) pos;
			image.rectTransform.localPosition = position;
			image.rectTransform.localScale = Vector3.one;
			ShipHealthUIBlock healthBlock = Imager.AddComponent<ShipHealthUIBlock>();
			healthBlock.health = ship.Blocks[pos].GetComponent<BlockHealth>();
			healthBlock.ui = this;
			UIFix(position);
		}

		[Button("Test")]
		void Test()
		{
			//AddImageAtPos(testSprite, testVec);
			ShowShip(ship);
		}

		public void UIFix(Vector2 position)
		{
			if (position.x > RightUpPoint.x)
			{
				RightUpPoint.x = position.x + 50;
			}

			if (position.y > RightUpPoint.y)
			{
				RightUpPoint.y = position.y + 50;
			}

			if (position.x < LeftDownPoint.x)
			{
				LeftDownPoint.x = position.x - 50;
			}

			if (position.y < LeftDownPoint.y)
			{
				LeftDownPoint.y = position.y - 50;
			}
		}

		public void ShowShip(Ship ship)
		{
			RightUpPoint = new Vector2(50, 50);
			LeftDownPoint = new Vector2(-50, -50);
			transform.localScale = Vector3.one;
			foreach (Transform tr in transform)
			{
				Destroy(tr.gameObject);
			}

			foreach (var block in ship.Blocks.Values)
			{
				if(block!=null)
					AddImageAtPos(HealthSprite, block.Pos);
			}

			float xMax = RightUpPoint.x - LeftDownPoint.x;
			float yMax = RightUpPoint.y - LeftDownPoint.y;
			float Max = xMax >= yMax ? xMax : yMax;
			transform.localScale = new Vector2(Scale / Max, Scale / Max);
			//transform.parent.localPosition =Center - new Vector2(0.5f * (RightUpPoint.x + LeftDownPoint.x), 0.5f * (RightUpPoint.y + LeftDownPoint.y)) * Scale /Max;
		}
	}
}
