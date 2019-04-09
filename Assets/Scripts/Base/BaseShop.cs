using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseShop : InputReceiverDelegate {

	[Header("Button menu")]
	public MyButtonList buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject shopView;

    [Header("Handlers")]
    public ShopBuyController shopController;
    public RestockController restockController;

	[Header("Data")]
	public ItemListVariable shopList;
	public PlayerData playerData;

	private int menuMode;
	private int itemIndex;


	private void Start () {
		menuTitle.text = "SHOP";
		menuMode = 0;
		basicView.SetActive(true);
		shopView.SetActive(false);
		buttons.ResetButtons();
		buttons.AddButton("BUY");
		buttons.AddButton("SELL");
		buttons.AddButton("RESTOCK");
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_SHOP);
		buttons.ForcePosition(0);
	}

    public override void OnUpArrow() {
		if (menuMode == 0) {
			buttons.Move(-1);
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveVertical(-1);
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(-1);
        }
    }

    public override void OnDownArrow() {
		if (menuMode == 0) {
			buttons.Move(1);
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveVertical(1);
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(1);
        }
    }

    public override void OnOkButton() {
		if (menuMode == 0) {
			int currentIndex = buttons.GetPosition();
            if (currentIndex == 0) {
                menuMode = 1;
                menuTitle.text = "BUY";
				shopController.GenerateShopList(shopList);
                shopView.SetActive(true);
				basicView.SetActive(false);
            }
            else if (currentIndex == 1) {
                menuMode = 2;
                menuTitle.text = "SELL";
				shopController.GenerateSellList();
                shopView.SetActive(true);
				basicView.SetActive(false);
            }
			else if (currentIndex == 2) {
				menuMode = 3;
				menuTitle.text = "RESTOCK";
				restockController.ShowRestock();
				shopView.SetActive(false);
				basicView.SetActive(false);
			}
		}
		else if (menuMode == 1 || menuMode == 2) {
			shopController.SelectItem();
		}
		else if (menuMode == 3) {
			restockController.SelectItem();
		}
	}

    public override void OnBackButton() {
		if (menuMode == 0) {

        }
        else if (menuMode == 1 || menuMode == 2) {
            if (shopController.DeselectItem()) {
                menuMode = 0;
                menuTitle.text = "SHOP";
                basicView.SetActive(true);
                shopView.SetActive(false);
            }
        }
		else if (menuMode == 3) {
			if (restockController.DeselectItem()) {
				menuMode = 0;
				menuTitle.text = "SHOP";
				basicView.SetActive(true);
			}
		}
    }

    public override void OnLeftArrow() {
		if (menuMode == 1 || menuMode == 2) {
            shopController.MoveHorizontal(-1);
        }
		else if (menuMode == 3) {
			restockController.MoveSide(-1);
		}
    }

    public override void OnRightArrow() {
		if (menuMode == 1 || menuMode == 2) {
            shopController.MoveHorizontal(1);
        }
		else if (menuMode == 3) {
			restockController.MoveSide(1);
		}
    }


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

