using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Universe
{

    public class MoveSkyByMouse : MonoBehaviour 
    {
        public float moveRatio = 0.1f;
        public Transform moveTarget;
    	void Update () 
        {
            Vector2 pos = Input.mousePosition;
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);
            Vector2 deltaPos = pos - screenSize;
            moveTarget.position = new Vector3(deltaPos.x, deltaPos.y, 0)*moveRatio;
    	}
    }
}