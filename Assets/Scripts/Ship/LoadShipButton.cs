using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShipProject.Factory.UI
{
	public class LoadShipButton : MonoBehaviour
	{
		public GameObject ButtonPrefab;
		public EventSystem es;
		public float gap;
		[Button("LoadButtons")]
		public void LoadButtons()
		{
			ShipFactory.instance.LoadList(ShipEnum.GameCamp.Player_);
			foreach (Transform t in transform)
			{
				Destroy(t.gameObject);
			}
			List<string> names = ShipFactory.instance.ShipNames[ShipEnum.GameCamp.Player_];
			if (names.Count == 0)
			{
				return;
			}

			for (int i = 0; i < names.Count; i++)
			{
				GameObject buttonObj=Instantiate(ButtonPrefab);
				buttonObj.transform.SetParent(transform);
				buttonObj.transform.GetChild(0).GetComponent<Text>().text = names[i];
				buttonObj.GetComponent<ShipLoaderButton>().index = i;
			}
		}
		public void Up()
		{
			GetComponent<RectTransform>().pivot += new Vector2(0, gap);
		}
		public void Down()
		{
			GetComponent<RectTransform>().pivot += new Vector2(0, -gap);
		}
	}
}
