#pragma strict

function Start () {
	// remember volume level from last time
	this.GetComponent.<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
}

function UpdateVolume(){
	this.GetComponent.<AudioSource>().volume = PlayerPrefs.GetFloat("MusicVolume");
}