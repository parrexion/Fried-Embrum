using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIExpMeter : MonoBehaviour {

	[Header("Exp Meter")]
	public MyBar expBar;
	public int currentExp;

	
	private void Update () {
		expBar.SetAmount(currentExp, 100);
	}
	
	//private IEnumerator ShowExpGain(TacticsMove player, int exp) {

	//	if (player == null) {
	//		//Debug.Log("Nothing to give exp for");
	//		yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
	//		yield break;
	//	}
		
	//	exp = player.EditValueSkills(SkillActivation.REWARD, exp);
	//	if (exp > 0) {
	//		expMeter.gameObject.SetActive(true);
	//		expMeter.currentExp = player.stats.currentExp;
	//		yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
	//		sfxQueue.Enqueue(levelupFill);
	//		playSfxEvent.Invoke();
	//		while (exp > 0) {
	//			exp--;
	//			expMeter.currentExp++;
	//			if (expMeter.currentExp == 100) {
	//				expMeter.currentExp = 0;
	//				stopSfxEvent.Invoke();
	//				yield return new WaitForSeconds(1f * currentGameSpeed.value);
	//				expMeter.gameObject.SetActive(false);
	//				levelupScript.SetupStats(player.stats, true);
	//				player.stats.GainLevel();
	//				sfxQueue.Enqueue(levelupFanfare);
	//				playSfxEvent.Invoke();
	//				yield return StartCoroutine(levelupScript.RunLevelup(player.stats));
	//				expMeter.gameObject.SetActive(true);
	//				sfxQueue.Enqueue(levelupFill);
	//				playSfxEvent.Invoke();
	//			}
	//			yield return null;
	//		}
	//		stopSfxEvent.Invoke();
	//		yield return new WaitForSeconds(0.5f * currentGameSpeed.value);
	//		expMeter.gameObject.SetActive(false);
	//		player.stats.currentExp = expMeter.currentExp;
	//	}
	//}
}
