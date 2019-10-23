using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleAnimator : MonoBehaviour {
	
	public enum HitType { NORMAL, MISS, CRIT }

	public Transform leftTransform;
	public GameObject leftDamageObject;
	public Text leftDamageText;
	[Space(5)]
	public Transform rightTransform;
	public GameObject rightDamageObject;
	public Text rightDamageText;

	[Header("SFX")]
	public AudioQueueVariable sfxQueue;
	public SfxEntry genericAttackSfx;
	public SfxEntry hitSfx;
	public SfxEntry missSfx;
	public SfxEntry critSfx;
	public SfxEntry deathSfx;
	public SfxEntry[] attackSfx;

	[Header("Events")]
	public UnityEvent BattleAnimationEvent;
	public UnityEvent playSfxEvent;

	private Transform attackTransform, defenseTransform;
	private GameObject defendDamageObject;
	private Text defendText;


	public void PlayAttack(bool isLeft, WeaponType type, HitType hitType, bool leathal, int damage) {
		attackTransform = (isLeft) ? leftTransform : rightTransform;
		defenseTransform = (isLeft) ? rightTransform : leftTransform;
		defendDamageObject = (isLeft) ? rightDamageObject : leftDamageObject;
		defendText = (isLeft) ? rightDamageText : leftDamageText;

		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);

		StartCoroutine(Animating(type, hitType, damage));
	}

	IEnumerator Animating(WeaponType type, HitType hitType, int damage) {
		float preHit = 0f;
		float postHit = 0f;
		switch (type) {
			case WeaponType.SHOTGUN:
				sfxQueue.Enqueue(attackSfx[0]);
				preHit = 0.2f;
				postHit = 1f;
				break;
			//case WeaponType.SNIPER:
			//	break;
			case WeaponType.RIFLE:
				sfxQueue.Enqueue(attackSfx[2]);
				preHit = 0.4f;
				postHit = 1f;
				break;
			//case WeaponType.BAZOOKA:
			//	break;
			//case WeaponType.MACHINEGUN:
			//	break;
			//case WeaponType.PISTOL:
			//	break;
			case WeaponType.ROCKET:
				sfxQueue.Enqueue(attackSfx[6]);
				preHit = 1.1f;
				postHit = 1f;
				break;
			//case WeaponType.PSI_BLAST:
			//	break;
			//case WeaponType.PSI_BLADE:
			//	break;
			//case WeaponType.MEDKIT:
			//	break;
			//case WeaponType.BARRIER:
			//	break;
			//case WeaponType.DEBUFF:
			//	break;
			//case WeaponType.C_HEAL:
			//	break;
			//case WeaponType.C_BOOST:
			//	break;
			//case WeaponType.CLAW:
			//	break;
			default:
				sfxQueue.Enqueue(genericAttackSfx);
				postHit = 1f;
				break;
		}
		playSfxEvent.Invoke();

		if (preHit > 0) {
			yield return new WaitForSeconds(preHit);
		}
		
		//Play hit
		switch (hitType) {
			case HitType.NORMAL:
				sfxQueue.Enqueue(hitSfx);
				break;
			case HitType.MISS:
				sfxQueue.Enqueue(missSfx);
				break;
			case HitType.CRIT:
				defenseTransform.GetComponent<ParticleSystem>().Play();
				postHit += 0.2f;
				sfxQueue.Enqueue(critSfx);
				break;
		}
		playSfxEvent.Invoke();

		StartCoroutine(DamageDisplay(damage, true, hitType == HitType.CRIT));

		if (postHit > 0) {
			yield return new WaitForSeconds(preHit);
		}

		//DONE
		BattleAnimationEvent.Invoke();
		yield break;
	}

	private IEnumerator DamageDisplay(int damage, bool isDamage, bool isCrit) {
		defendText.color = (isDamage) ? Color.black : new Color(0, 0.5f, 0);
		defendText.text = (damage != -1) ? damage.ToString() : "Miss";
		defendDamageObject.transform.localScale = (isCrit) ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);
		defendDamageObject.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);
		defendDamageObject.gameObject.SetActive(false);
	}
}
