using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseShop : InputReceiverDelegate {

	[Header("Button menu")]
	public MyButton[] buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject shopView;

    [Header("Handlers")]
    public ShopBuyController shopController;
    public RestockController restockController;

	[Header("Data")]
	public ItemListVariable shopList;
	public SaveListVariable playerData;

	private int menuMode;
	private int currentIndex;
	private int itemIndex;


	private void Start () {
		menuTitle.text = "SHOP";
		menuMode = 0;
		currentIndex = 0;
		basicView.SetActive(true);
		shopView.SetActive(false);
	}

    public override void OnMenuModeChanged() {
		bool prevActive = active;
		active = (currentMenuMode.value == (int)MenuMode.BASE_SHOP);
		UpdateButtons();
		if (prevActive != active)
			ActivateDelegates(active);
	}

    public override void OnUpArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length, currentIndex-1);
			UpdateButtons();
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveSelection(-1);
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(-1);
        }
    }

    public override void OnDownArrow() {
		if (!active)
			return;
		if (menuMode == 0) {
			currentIndex = OPMath.FullLoop(0, buttons.Length, currentIndex+1);
			UpdateButtons();
        }
        else if (menuMode == 1 || menuMode == 2) {
            shopController.MoveSelection(1);
        }
        else if (menuMode == 3) {
            restockController.MoveSelection(1);
        }
    }

    public override void OnOkButton() {
		if (!active)
			return;
		if (menuMode == 0) {
            if (currentIndex == 0) {
                menuMode = 1;
                menuTitle.text = "BUY";
				shopController.GenerateLists(shopList, true);
                shopView.SetActive(true);
				basicView.SetActive(false);
            }
            else if (currentIndex == 1) {
                menuMode = 2;
                menuTitle.text = "SELL";
				shopController.GenerateLists(shopList, false);
                shopView.SetActive(true);
				basicView.SetActive(false);
            }
			else if (currentIndex == 2) {
				menuMode = 3;
				menuTitle.text = "RESTOCK";
				restockController.GenerateLists();
				shopView.SetActive(false);
				basicView.SetActive(false);
			}
		}
		else if (menuMode == 1) {
			shopController.SelectItem(true);
		}
		else if (menuMode == 2) {
			shopController.SelectItem(false);
		}
		else if (menuMode == 3) {
			restockController.SelectItem();
		}
	}

    public override void OnBackButton() {
		if (!active)
			return;
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
			restockController.DeselectItem();
		}
    }

    public override void OnLeftArrow() {
		if (!active)
			return;
		if (menuMode == 1 || menuMode == 2) {
            shopController.ChangeCategory(-1);
        }
		else if (menuMode == 3) {
			restockController.MoveSide(-1);
		}
    }

    public override void OnRightArrow() {
		if (!active)
			return;
		if (menuMode == 1 || menuMode == 2) {
            shopController.ChangeCategory(1);
        }
		else if (menuMode == 3) {
			restockController.MoveSide(1);
		}
    }

	private void UpdateButtons() {
		for (int i = 0; i < buttons.Length; i++) {
			buttons[i].SetSelected(i == currentIndex);
		}
	}


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

