using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Audio controller class which plays the background music and sfx.
/// </summary>
public class AudioController : MonoBehaviour {
	
#region Singleton
	private static AudioController instance;

	void Awake() {
		if (instance != null) {
			Destroy(gameObject);
		}
		else {
			DontDestroyOnLoad(gameObject);
			instance = this;
			Startup();
		}
	}
#endregion

	[Header("SFX")]
	public AudioQueueVariable sfxQueue;
	public IntVariable effectVolume;
	public AudioSource[] efxSource;

	[Header("Music")]
	public BoolVariable musicFocusSource;
	public IntVariable musicVolume;
	public AudioVariable mainMusic;
	public AudioVariable subMusic;
	public AudioSource musicMainSource;
	public AudioSource musicSubSource;

	[Header("Variation values")]
	[MinMaxRange(0.75f,1.5f)]
	public RangedFloat pitchRange = new RangedFloat(0.95f,1.05f);
	
	private int currentSfxTrack;


	void Startup() {
		UpdateVolume();
	}

	/// <summary>
	/// Updates the volumes to a value between 0 and 1.
	/// </summary>
	public void UpdateVolume() {
		for (int i = 0; i < efxSource.Length; i++) {
			efxSource[i].volume = 0.01f * Mathf.Clamp(effectVolume.value, 0 ,100);
		}
		musicMainSource.volume = 0.01f * Mathf.Clamp(musicVolume.value, 0 ,100);
		musicSubSource.volume = 0.01f * Mathf.Clamp(musicVolume.value, 0 ,100);
	}

	/// <summary>
	/// Plays the music if the area has changed.
	/// </summary>
	public void PlayMainMusic() {
		PlayBackgroundMusic(true, true);
		PlayBackgroundMusic(false, false);
	}

	/// <summary>
	/// Plays the music if the area has changed.
	/// </summary>
	public void PlaySubMusic() {
		Debug.Log("ASFJSL:   " + subMusic.value);
		PlayBackgroundMusic(true, false);
		PlayBackgroundMusic(false, true);
	}

	/// <summary>
	/// Playes the background music or stops the music if clip is null.
	/// </summary>
	/// <param name="clip">Clip.</param>
	void PlayBackgroundMusic(bool isMain, bool updateClip) {
		// Debug.Log("MUSIC!");
		AudioClip selectedSong = (isMain) ? mainMusic.value : subMusic.value;
		AudioSource source = (isMain) ? musicMainSource : musicSubSource;
		bool useVolume = (isMain && musicFocusSource.value || !isMain && !musicFocusSource.value);
		
		source.volume = (useVolume) ? 0.01f * Mathf.Clamp(musicVolume.value, 0, 100) : 0;
		if (selectedSong == null) {
			source.Stop();
			source.clip = null;
		}
		else if (updateClip) {
			source.clip = selectedSong;
			source.Play();
		}
	}

	/// <summary>
	/// Plays the next sfx clip.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public void PlaySfx() {
		while(sfxQueue.value.Count > 0) {
			AudioClip sfxClip = sfxQueue.value.Dequeue();
			if (sfxClip != null) {
				RandomizePitch();
				efxSource[currentSfxTrack].clip = sfxClip;
				efxSource[currentSfxTrack].Play();
				currentSfxTrack = (currentSfxTrack + 1) % efxSource.Length;
			}
		}
	}

	/// <summary>
	/// Plays the next sfx clip.
	/// </summary>
	/// <param name="clip">Clip.</param>
	public void PlaySfxEntry(SfxEntry entry) {
		if (entry != null && entry.clip != null) {
			RandomizePitch();
			efxSource[currentSfxTrack].clip = entry.clip;
			efxSource[currentSfxTrack].Play();
			currentSfxTrack = (currentSfxTrack + 1) % efxSource.Length;
		}
	}

	/// <summary>
	/// Stops all the sfx currently playing.
	/// </summary>
	public void StopAllMusic() {
		musicMainSource.Stop();
		musicSubSource.Stop();
	}

	/// <summary>
	/// Stops all the sfx currently playing.
	/// </summary>
	public void StopAllSfx() {
		for (int i = 0; i < efxSource.Length; i++) {
			efxSource[i].Stop();
		}
	}

	/// <summary>
	/// Plays a random sfx from the list with a random pitch
	/// </summary>
	/// <param name="clips">Clips.</param>
	public void RandomizePitch() {
		float randomPitch = Random.Range(pitchRange.minValue,pitchRange.maxValue);
		efxSource[currentSfxTrack].pitch = randomPitch;
	}
}
