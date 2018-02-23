using System;
using System.Collections;
using System.Collections.Generic;
using ShipProject;
using ShipProject.Factory;
using ShipProject.Factory.UI;
using ShipProject.ShipEnum;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class FactoryListener : MonoBehaviour
{
	public static FactoryListener Instance;
	public Vector2Int currentPos;
	public ShipUnitRotation rotation;
	public ShipUnitMirror mirror;
	private ShipComponent currentcomponent;

	public ShipComponent currentComponent;
	public Ship currentShip;
	public Sprite PointSprite;
	public string MenuButton = "Menu";
	public Animator menu;
	public GameObject EventSystem;
	public ShipFactory factory;
	private float inputTimer=0;
	public ShipFactoryTypeSelector selector;
	public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
	public bool Testing;
	public Camera camera;
	private void Awake()
	{
		Instance = this;
	}
	private void Start()
	{
		NewShip();
	}

	public void NewShip()
	{
		if(currentShip!=null)
		Destroy(currentShip.gameObject);
		currentShip = Instantiate(factory.EmptyShip.gameObject).GetComponent<Ship>();
		currentShip.AddComponent(0, 1, new Vector2Int(0, 0));
	}

	public void SaveShip(string shipName)
	{
		currentShip.ShipName = shipName;
		factory.SaveShip(currentShip,GameCamp.Player_);
	}

	public void LoadShip(string shipName)
	{

		if (currentShip != null)
			Destroy(currentShip.gameObject);
		currentShip = factory.LoadShipAtTransform(shipName,factory.transform,GameCamp.Player_);
	}
	public void Update()
	{

		camera.orthographicSize += Input.GetAxis("Scroll");
		if (!Testing)
		{
			transform.position = 1.6f*(Vector2) currentPos;
			if (Input.GetButtonDown(MenuButton))
			{
				EventSystem.SetActive(!EventSystem.activeInHierarchy);
				menu.SetBool("work", EventSystem.activeInHierarchy);
				SetColor();
				if (EventSystem.activeInHierarchy)
				{
					EventSystem.GetComponent<EventSystem>()
						.SetSelectedGameObject((selector.toggle as ComponentTypeSelectToggle).Panel.transform.GetChild(0).gameObject);
				}
			}

			if (currentcomponent
			    != currentComponent)
			{
				ShowCurrentComponent();
				currentcomponent = currentComponent;
			}
			if (inputTimer <= 0&& !EventSystem.activeInHierarchy)
			{
				if (Input.GetAxis("Horizontal") > 0)
				{
					inputTimer = 0.2f;
					currentPos += new Vector2Int(1, 0);
					SetColor();

				}
				if (Input.GetAxis("Horizontal") < 0)
				{
					inputTimer = 0.2f;
					currentPos += new Vector2Int(-1, 0);
					SetColor();
				}
				if (Input.GetAxis("Vertical") > 0)
				{
					inputTimer = 0.2f;
					currentPos += new Vector2Int(0, 1);
					SetColor();

				}
				if (Input.GetAxis("Vertical") < 0)
				{
					inputTimer = 0.2f;
					currentPos += new Vector2Int(0, -1);
					SetColor();
				}
				camera.transform.position += (Vector3)new Vector2(Input.GetAxis("CameraH"), Input.GetAxis("CameraV"));
			}
			else if(inputTimer>0)
			{
				inputTimer -= Time.deltaTime;
			}

			if (!EventSystem.activeInHierarchy)
			{
				if (Input.GetButtonDown("A"))
				{
					currentShip.AddComponent(currentComponent.Id, currentComponent.ShipCompoionentLevel, currentPos, rotation,
						mirror);
					SetColor();
				}

				if (Input.GetButtonDown("X"))
				{
					currentShip.RemoveBlockAtPosition(currentPos);
					SetColor();
				}

				if (Input.GetButtonDown("Mirror"))
				{
					int m = (int)mirror;
					m++;
					if (m > 1)
					{
						m = 0;
					}

					mirror = (ShipUnitMirror) Enum.ToObject(typeof(ShipUnitMirror), m);
					ShowCurrentComponent();
				}
				if (Input.GetButtonDown("Rotation"))
				{
					int m = (int)rotation;
					m++;
					if (m > 3)
					{
						m = 0;
					}

					rotation = (ShipUnitRotation)Enum.ToObject(typeof(ShipUnitRotation), m);
					ShowCurrentComponent();
				}
			}

		}

		if (Testing)
		{
			camera.transform.position = currentShip.transform.position+new Vector3(0,0,-10f);
			if (Input.GetButtonDown(MenuButton))
			{
				Testing = false;
				currentShip.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				currentShip.transform.position = factory.transform.position;
				currentShip.transform.rotation = Quaternion.identity;
				currentShip.GetComponent<ShipControl>().mode = ShipControlMode.Building;
				foreach (Transform t in transform)
				{
					t.gameObject.SetActive(true);
				}
			}
		}
	}

	public void TurnToTest()
	{
		EventSystem.SetActive(false);
		menu.SetBool("work", false);
		SetColor();
		Testing = true;
		currentShip.GetComponent<ShipControl>().mode = ShipControlMode.Player;
		foreach (Transform t in transform)
		{
			t.gameObject.SetActive(false);
		}
	}
	public void SetColor()
	{
		if (currentShip == null)
		{
			return;
		}
		if (currentShip.ComponentPutable(currentComponent.Id, currentComponent.ShipCompoionentLevel, currentPos, rotation,
			mirror))
		{
			foreach (var sr in renderers)
			{
				sr.color=new Color(0.8f, 0.8f, 0.8f, 0.8f);
			}
		}
		else
		{
			foreach (var sr in renderers)
			{
				sr.color = new Color(0.5f, 0.5f, 0.5f, 0.27f);
			}
		}
	}
	[Button("ShowCurrentComponent")]
	public void ShowCurrentComponent()
	{
		foreach (Transform t in transform)
		{
			Destroy(t.gameObject);
		}
		renderers.Clear();
			GameObject obj = new GameObject("Point");
			obj.transform.SetParent(transform);
			SpriteRenderer rend= obj.AddComponent<SpriteRenderer>();
		if (currentComponent.SpriteOnFactory != null)
			rend.sprite = currentComponent.SpriteOnFactory;
		else
			rend.sprite = currentComponent.SpriteOnUI;
		var rot = Vector3.zero;
		//镜像处理
		switch (mirror)
		{
			case ShipUnitMirror.Normal:
				break;
			case ShipUnitMirror.MirrorX:
				rot.x = 180;
				break;
			case ShipUnitMirror.MirrorY:
				rot.y = 180;
				break;
			case ShipUnitMirror.MirrorBoth:
				rot.x = 180;
				rot.y = 180;
				break;
		}
		//旋转处理
		switch (rotation)
		{
			case ShipUnitRotation.d0:
				rot.z = 0;
				break;
			case ShipUnitRotation.d90:
				rot.z = 90;
				break;
			case ShipUnitRotation.d180:
				rot.z = 180;
				break;
			case ShipUnitRotation.d270:
				rot.z = 270;
				break;
		}
		obj.transform.localPosition = Vector3.zero;
		obj.transform.rotation = Quaternion.Euler(rot);
			renderers.Add(rend);

		SetColor();
	}
}
