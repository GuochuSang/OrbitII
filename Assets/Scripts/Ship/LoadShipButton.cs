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
		public float Offset;
		public EventSystem es;
		[Button("LoadButtons")]
		public void LoadButtons()
		{
			ShipFactory.instance.LoadList();
			foreach (Transform t in transform)
			{
				Destroy(t.gameObject);
			}
			List<string> names = ShipFactory.instance.PlayerShipNames;
			if (names.Count == 0)
			{
				return;
			}

			for (int i = 0; i < names.Count; i++)
			{
				GameObject buttonObj=Instantiate(ButtonPrefab);
				buttonObj.transform.SetParent(transform);
				buttonObj.transform.GetChild(0).GetComponent<Text>().text = names[i];
			}
		}
		public void Update()
		{
			if(es.currentSelectedGameObject!=null)
			if (es.currentSelectedGameObject.transform.IsChildOf(transform))
			{
				Vector3 p1 = GetComponent<RectTransform>().position;
				Vector3 p2 = es.currentSelectedGameObject.GetComponent<RectTransform>().position;
				GetComponent<RectTransform>().position = new Vector3(p1.x,(p1.y-p2.y)+Offset,p1.z);
			}
		}
	}
}
