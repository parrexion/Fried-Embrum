using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseShop : InputReceiverDelegate {
	
	private enum State { MAIN, BUY, SELL }

	[Header("Button menu")]
	public MyButtonList buttons;
	public Text menuTitle;

	[Header("Views")]
	public GameObject basicView;
	public GameObject shopView;
	private State currentMenu;

    [Header("Handlers")]
    public ShopBuyController shopController;

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
	}

    public override void OnMenuModeChanged() {
		UpdateState(MenuMode.BASE_SHOP);
		buttons.ForcePosition(0);
	}

    public override void OnUpArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(-1);
			menuMoveEvent.Invoke();
        }
        else if (currentMenu == State.BUY || currentMenu == State.SELL) {
            shopController.MoveVertical(-1);
			menuMoveEvent.Invoke();
        }
    }

    public override void OnDownArrow() {
		if (currentMenu == State.MAIN) {
			buttons.Move(1);
			menuMoveEvent.Invoke();
        }
        else if (currentMenu == State.BUY || currentMenu == State.SELL) {
            shopController.MoveVertical(1);
			menuMoveEvent.Invoke();
        }
    }

    public override void OnOkButton() {
		if (currentMenu == State.MAIN) {
			int currentIndex = buttons.GetPosition();
            if (currentIndex == 0) {
                currentMenu = State.BUY;
                menuTitle.text = "BUY";
				shopController.GenerateShopList(shopList);
                shopView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
            }
            else if (currentIndex == 1) {
                currentMenu = State.SELL;
                menuTitle.text = "SELL";
				shopController.GenerateSellList();
                shopView.SetActive(true);
				basicView.SetActive(false);
				menuAcceptEvent.Invoke();
            }
		}
		else if (currentMenu == State.BUY || currentMenu == State.SELL) {
			shopController.SelectItem();
			menuAcceptEvent.Invoke();
		}
	}

    public override void OnBackButton() {
		if (currentMenu == State.MAIN) {
			MenuChangeDelay(MenuMode.BASE_MAIN);
		}
		else if (currentMenu == State.BUY || currentMenu == State.SELL) {
            if (shopController.DeselectItem()) {
                currentMenu = State.MAIN;
                menuTitle.text = "SHOP";
                basicView.SetActive(true);
                shopView.SetActive(false);
				menuBackEvent.Invoke();
            }
        }
    }

    public override void OnLeftArrow() {
		if (currentMenu == State.BUY || currentMenu == State.SELL) {
            shopController.MoveHorizontal(-1);
			menuMoveEvent.Invoke();
        }
    }

    public override void OnRightArrow() {
		if (currentMenu == State.BUY || currentMenu == State.SELL) {
            shopController.MoveHorizontal(1);
			menuMoveEvent.Invoke();
        }
    }


    public override void OnLButton() { }
    public override void OnRButton() { }
    public override void OnStartButton() { }
    public override void OnXButton() { }
    public override void OnYButton() { }
}

