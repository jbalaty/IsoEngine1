using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject ShopPanel;
	

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void ShowShopPanel ()
	{
//		var active = ShopPanel.activeInHierarchy;
		ShopPanel.SetActive (true);
	}

	public void HideShopPanel ()
	{
//		var active = ShopPanel.activeInHierarchy;
		ShopPanel.SetActive (false);
	}
}
