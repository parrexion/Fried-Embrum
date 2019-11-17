using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AttackSide { LEFT, RIGHT }
public enum FightSide { ATTACK, DEFENSE }

public class BattleAnimator : MonoBehaviour {

	public class AnimationInfo {
		public AttackSide side;
		public WeaponType weaponType;
		public HitType hitType;
		public int damage;
		public bool leathal;

		public float preHit;
		public float postHit;
		public SfxEntry attackSfx;
	}


	public enum HitType { NORMAL, MISS, CRIT }

	public FloatVariable currentGameSpeed;
	public FloatVariable battleMoveSpeed;

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
	private SpriteRenderer attackRenderer, defenseRenderer;
	private Vector3 leftPos, rightPos;
	private GameObject defendDamageObject;
	private Text defendText;


	public void SetupScene(Sprite attackSprite, Sprite defenseSprite) {
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
		attackRenderer = leftTransform.GetComponent<SpriteRenderer>();
		defenseRenderer = rightTransform.GetComponent<SpriteRenderer>();
		attackRenderer.sprite = attackSprite;
		defenseRenderer.sprite = defenseSprite;
		attackRenderer.color = Color.white;
		defenseRenderer.color = Color.white;
		leftPos = leftTransform.position;
		rightPos = rightTransform.position;
	}

	public void CleanupScene() {
		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);
	}

	public void PlayAttack(AnimationInfo info) {
		attackTransform = (info.side == AttackSide.LEFT) ? leftTransform : rightTransform;
		defenseTransform = (info.side == AttackSide.LEFT) ? rightTransform : leftTransform;
		defendDamageObject = (info.side == AttackSide.LEFT) ? rightDamageObject : leftDamageObject;
		defendText = (info.side == AttackSide.LEFT) ? rightDamageText : leftDamageText;

		leftDamageObject.SetActive(false);
		rightDamageObject.SetActive(false);

		GenerateAnimation(info);

		StartCoroutine(Animating(info));
	}

	IEnumerator Animating(AnimationInfo info) {

		playSfxEvent.Invoke();
		
		if (info.side == AttackSide.LEFT)
			yield return StartCoroutine(MoveForward(info.preHit, leftPos, rightPos));
		else
			yield return StartCoroutine(MoveForward(info.preHit, rightPos, leftPos));

		//Play hit
		switch (info.hitType) {
			case HitType.NORMAL:
				sfxQueue.Enqueue(hitSfx);
				break;
			case HitType.MISS:
				sfxQueue.Enqueue(missSfx);
				break;
			case HitType.CRIT:
				defenseTransform.GetComponent<ParticleSystem>().Play();
				info.postHit += 0.2f;
				sfxQueue.Enqueue(critSfx);
				break;
		}
		playSfxEvent.Invoke();

		if (info.leathal)
			PlayDeath(FightSide.DEFENSE);

		yield return StartCoroutine(DamageDisplay(info.damage, true, info.hitType == HitType.CRIT));


		if (info.postHit > 0) {
			yield return new WaitForSeconds(info.postHit);
		}

		//DONE
		BattleAnimationEvent.Invoke();
		yield break;
	}

	private IEnumerator MoveForward(float duration, Vector3 startPos, Vector3 targetPos) {
		//Move forward
		float f = 0;
		// Debug.Log("Start moving");
		while (f < 0.5f) {
			f += Time.deltaTime * battleMoveSpeed.value / currentGameSpeed.value;
			attackTransform.position = Vector3.Lerp(startPos, targetPos, f);
			yield return null;
		}
		if (duration > 0.5f) {
			yield return new WaitForSeconds(duration - 0.5f);
		}
	}

	private IEnumerator MoveBack(float duration, Vector3 startPos, Vector3 targetPos) {
		//Move forward
		float f = 0.5f;
		// Debug.Log("Start moving");
		while (f > 0f) {
			f -= Time.deltaTime * battleMoveSpeed.value / currentGameSpeed.value;
			attackTransform.position = Vector3.Lerp(startPos, targetPos, f);
			yield return null;
		}
		if (duration > 0.5f) {
			yield return new WaitForSeconds(duration - 0.5f);
		}
	}

	private IEnumerator DamageDisplay(int damage, bool isDamage, bool isCrit) {
		defendText.color = (isDamage) ? Color.black : new Color(0, 0.5f, 0);
		defendText.text = (damage != -1) ? damage.ToString() : "Miss";
		defendDamageObject.transform.localScale = (isCrit) ? new Vector3(2, 2, 2) : new Vector3(1, 1, 1);
		defendDamageObject.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);
		defendDamageObject.gameObject.SetActive(false);
	}

	public void PlayDeath(FightSide side) {
		if (side == FightSide.ATTACK)
			attackRenderer.color = new Color(0.4f, 0.4f, 0.4f);
		else
			defenseRenderer.color = new Color(0.4f, 0.4f, 0.4f);
	}

	private void GenerateAnimation(AnimationInfo info) {
		switch (info.weaponType) {
			case WeaponType.SHOTGUN:
				sfxQueue.Enqueue(attackSfx[0]);
				info.preHit = 0.2f;
				info.postHit = 1f;
				break;
			//case WeaponType.SNIPER:
			//	break;
			case WeaponType.RIFLE:
				sfxQueue.Enqueue(attackSfx[2]);
				info.preHit = 0.4f;
				info.postHit = 1f;
				break;
			//case WeaponType.BAZOOKA:
			//	break;
			//case WeaponType.MACHINEGUN:
			//	break;
			//case WeaponType.PISTOL:
			//	break;
			case WeaponType.ROCKET:
				sfxQueue.Enqueue(attackSfx[6]);
				info.preHit = 1.1f;
				info.postHit = 1f;
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
				info.postHit = 1f;
				break;
		}
	}
}
