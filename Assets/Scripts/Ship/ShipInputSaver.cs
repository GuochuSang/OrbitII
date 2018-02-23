using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipInputSaver : MonoBehaviour
{
	public Text txtc;
	public void SaveShip()
	{
		FactoryListener.Instance.SaveShip(txtc.text);
	}
}
