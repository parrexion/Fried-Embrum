using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorTilePlacer : MonoBehaviour {

	public int selectedIndex;
	public TemplateRecycler tileButtons;

	[Header("Terrain Tiles")]
	public TerrainTile[] tiles;


	public void CreateWindow() {
		tileButtons.Clear();
		for(int i = 0; i < tiles.Length; i++) {
			MapEditorIndexButton button = tileButtons.CreateEntry<MapEditorIndexButton>();
			button.buttonImage.sprite = tiles[i].sprite;
			int index = i;
			button.Setup(() => {
				TileSelected(index);
			});
			if (selectedIndex == index) {
				button.GetComponent<Toggle>().isOn = true;
				Debug.Log("Selected " + index);
			}
		}
	}

	public void TileSelected(int index) {
		Debug.Log("Clicked tile " + index);
		selectedIndex = index;
	}

	public void ColorTile(Image tile) {
		tile.sprite = tiles[selectedIndex].sprite;
	}
}
