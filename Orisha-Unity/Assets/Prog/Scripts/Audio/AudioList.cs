using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioList : MonoBehaviour {

	[Header("Main Menu")]
	public List<AudioClip> MainMenuSound;
	public List<AudioClip> MainMenuMusic;

	[Header("Menu")]
	public List<AudioClip> MenuSound;

	[Header("Explo Music")]
	public List<AudioClip> ExploMusic;

	[Header("Town Music")]
	public List<AudioClip> TownMusic;

	[Header("FX Sound")]
	public List<AudioClip> FXSound;


}
