#pragma strict

function Start () {
	// check shadow distance/enabled
	if(PlayerPrefs.GetInt("Shadows") == 0){
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowDistance = 0;
	}
	else if(PlayerPrefs.GetInt("Shadows") == 1){
		QualitySettings.shadowCascades = 2;
		QualitySettings.shadowDistance = 75;
	}
	else if(PlayerPrefs.GetInt("Shadows") == 2){
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowDistance = 500;
	}

	// check vsync
	if(QualitySettings.vSyncCount == 0){
		QualitySettings.vSyncCount = 0;
	}
	else if(QualitySettings.vSyncCount == 1){
		QualitySettings.vSyncCount = 1;
	}

	// check mouse inverse
	if(PlayerPrefs.GetInt("Inverted")==0){
		
	}
	else if(PlayerPrefs.GetInt("Inverted")==1){
		
	}

	// check motion blur
	if(PlayerPrefs.GetInt("MotionBlur")==0){

	}
	else if(PlayerPrefs.GetInt("MotionBlur")==1){

	}

	// check ambient occlusion
	if(PlayerPrefs.GetInt("AmbientOcclusion")==0){
		
	}
	else if(PlayerPrefs.GetInt("AmbientOcclusion")==1){
		
	}

	// check shadow distance/enabled
	if(PlayerPrefs.GetInt("Textures") == 0){
		QualitySettings.masterTextureLimit = 2;
	}
	else if(PlayerPrefs.GetInt("Textures") == 1){
		QualitySettings.masterTextureLimit = 1;
	}
	else if(PlayerPrefs.GetInt("Textures") == 2){
		QualitySettings.masterTextureLimit = 0;
	}
}