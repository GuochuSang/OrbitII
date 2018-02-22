using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace ShipProject.Factory.UI
{
	public class ComponentToggle : MonoBehaviour
	{
		public Image image;
		public ShipComponent shipComponent;
		public Toggle toggle;
		private void Start()
		{
			toggle = GetComponent<Toggle>();
		}
		[Button("SetToggle")]
		public void SetToggle()
		{
			if(shipComponent.SpriteOnUI!=null)
			image.sprite = shipComponent.SpriteOnUI;
			image.transform.localScale = new Vector3(image.sprite.rect.width/16, image.sprite.rect.height/16);
		}
		public void Update()
		{
			if (toggle.isOn)
			{
				FactoryListener.Instance.currentComponent = shipComponent;
			}
		}
	}
}
