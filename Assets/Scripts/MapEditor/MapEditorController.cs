using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorController : MonoBehaviour {

	public enum EditState { TILE, CHARACTER, EVENT, SETTINGS}

	public MapEditorTilePlacer tilePlacer;
	public RectTransform mapTile;
	public Transform mapParent;

	[Header("Panels")]
	public GameObject tilePanel;
	public GameObject characterPanel;
	public GameObject eventPanel;
	public GameObject settingsPanel;

	[Header("Map settings")]
	public int sizeX;
	public int sizeY;

	private EditState editMode = EditState.TILE;
	private List<Button> mapTiles = new List<Button>();


	private void Start() {
		GenerateMap();
		tilePlacer.CreateWindow();
	}
	
	private void GenerateMap() {
		mapTile.gameObject.SetActive(true);
		float offsetX = sizeX / 2f - 0.5f;
		float offsetY = sizeY / 2f - 0.5f;
		for(int y = 0; y < sizeY; y++) {
			for(int x = 0; x < sizeX; x++) {
				RectTransform t = Instantiate(mapTile, mapParent);
				t.anchoredPosition = new Vector2((x - offsetX) * 100, (y - offsetY) * 100);
				int posx = x;
				int posy = y;
				Button but = t.GetComponent<Button>();
				but.onClick.AddListener(() => MapTileClicked(posx, posy));
				mapTiles.Add(but);
			}
		}
		mapTile.gameObject.SetActive(false);
	}

	private Button GetTile(int x, int y) {
		return mapTiles[y * sizeX + x];
	}

	public void MapTileClicked(int x, int y) {
		Debug.Log("Clicked tile " + x + " : " + y);
		tilePlacer.ColorTile(GetTile(x, y).GetComponent<Image>());
	}

	public void SetEditMode(int mode) {
		editMode = (EditState)mode;
		tilePanel.SetActive(editMode == EditState.TILE);
		characterPanel.SetActive(editMode == EditState.CHARACTER);
		eventPanel.SetActive(editMode == EditState.EVENT);
		settingsPanel.SetActive(editMode == EditState.SETTINGS);
	}
}
