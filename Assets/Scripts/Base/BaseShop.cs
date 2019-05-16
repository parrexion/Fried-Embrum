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


	private void Start () {
		basicView.SetActive(true);
		shopView.SetActive(false);
		SetupButtons();
	}

	private void SetupButtons() {
		menuTitle.text = "SHOP";
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
		int menuMode = buttons.GetPosition();
		if (menuMode == 0) {
			buttons.Move(-1);
			menuMoveEvent.Invoke();
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveVertical(-1);
			menuMoveEvent.Invoke();
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(-1);
			menuMoveEvent.Invoke();
        }
    }

    public override void OnDownArrow() {
		int menuMode = buttons.GetPosition();
		if (menuMode == 0) {
			buttons.Move(1);
			menuMoveEvent.Invoke();
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveVertical(1);
			menuMoveEvent.Invoke();
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(1);
			menuMoveEvent.Invoke();
        }
    }

    public override void OnOkButton() {
		int menuMode = buttons.GetPosition();
		if (menuMode == 0) {
			int currentIndex = buttons.GetPosition();
            if (currentIndex == 0) {
                menuMode = 1;
                menuTitle.text = "BUY";
				shopController.GenerateShopList(shopList);
                shopView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
            }
            else if (currentIndex == 1) {
                menuMode = 2;
                menuTitle.text = "SELL";
				shopController.GenerateSellList();
                shopView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
            }
			else if (currentIndex == 2) {
				menuMode = 3;
				menuTitle.text = "RESTOCK";
				restockController.ShowRestock();
				shopView.SetActive(false);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
			}
		}
		else if (menuMode == 1 || menuMode == 2) {
			shopController.SelectItem();
			menuAcceptEvent.Invoke();
		}
		else if (menuMode == 3) {
			restockController.SelectItem();
			menuAcceptEvent.Invoke();
		}
	}

    public override void OnBackButton() {
		int menuMode = buttons.GetPosition();
		if (menuMode == 1 || menuMode == 2) {
            if (shopController.DeselectItem()) {
                menuMode = 0;
                menuTitle.text = "SHOP";
                basicView.SetActive(true);
                shopView.SetActive(false);
				menuBackEvent.Invoke();
            }
        }
		else if (menuMode == 3) {
			if (restockController.DeselectItem()) {
				menuMode = 0;
				basicView.SetActive(true);
				SetupButtons();
				buttons.ForcePosition(2);
				menuBackEvent.Invoke();
			}
		}
    }

    public override void OnLeftArrow() {
		int menuMode = buttons.GetPosition();
		if (menuMode == 1 || menuMode == 2) {
            shopController.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
        }
		else if (menuMode == 3) {
			restockController.MoveSide(-1);
			menuMoveEvent.Invoke();
		}
    }

    public override void OnRightArrow() {
		int menuMode = buttons.GetPosition();
		if (menuMode == 1 || menuMode == 2) {
            shopController.MoveHorizontal(1);
			menuMoveEvent.Invoke();
        }
		else if (menuMode == 3) {
			restockController.MoveSide(1);
			menuMoveEvent.Invoke();
		}
    }


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

