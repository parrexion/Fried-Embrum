using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainController : MonoBehaviour {

	public BattleMap battleMap;
	public IntVariable cursorX;
	public IntVariable cursorY;

	[Header("UI elements")]
	public Text terrainName;
	public Text terrainDef;
	public Text terrainAvoid;
	public Text terrainHeal;


	public void UpdateTerrainInfo() {
		TerrainTile tile = battleMap.GetTile(cursorX.value, cursorY.value).terrain;
		terrainName.text = tile.name;
		terrainDef.text = tile.defense.ToString();
		terrainAvoid.text = tile.avoid.ToString();
		terrainHeal.text = tile.healPercent.ToString();
	}
}
