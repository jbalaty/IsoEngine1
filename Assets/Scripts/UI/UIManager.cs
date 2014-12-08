using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

	public GameObject dlg_shop;
    public GameObject dlg_buildings;
	

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
		dlg_shop.SetActive (true);
	}

	public void HideShopPanel ()
	{
		dlg_shop.SetActive (false);
        dlg_buildings.SetActive(false);
	}

    public bool IsShopPanelVisible
    {
        get
        {
            return dlg_shop.activeInHierarchy;
        }
    }
}
