using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ShipProject
{
	public class MechineGunBlock : MonoBehaviour
	{
		ShipMechineGun mechineGun;
		private void Start()
		{
			mechineGun = GetComponent<Block>().ParentShipComponent.GetComponent<ShipMechineGun>();
		}
		public void OnShoot()
		{
			mechineGun.OnShoot();
		}
	}
}
