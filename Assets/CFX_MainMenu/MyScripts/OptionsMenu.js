#pragma strict
import UnityEngine.UI;

// toggle buttons
var fullscreentext : GameObject;
var shadowofftext : GameObject;
var shadowofftextLINE : GameObject;
var shadowlowtext : GameObject;
var shadowlowtextLINE : GameObject;
var shadowhightext : GameObject;
var shadowhightextLINE : GameObject;
var showhudtext : GameObject;
var tooltipstext : GameObject;
var difficultynormaltext : GameObject;
var difficultynormaltextLINE : GameObject;
var difficultyhardcoretext : GameObject;
var difficultyhardcoretextLINE : GameObject;
var cameraeffectstext : GameObject;
var invertmousetext : GameObject;
var vsynctext : GameObject;
var motionblurtext : GameObject;
var ambientocclusiontext : GameObject;
var texturelowtext : GameObject;
var texturelowtextLINE : GameObject;
var texturemedtext : GameObject;
var texturemedtextLINE : GameObject;
var texturehightext : GameObject;
var texturehightextLINE : GameObject;
var aaofftext : GameObject;
var aaofftextLINE : GameObject;
var aa2xtext : GameObject;
var aa2xtextLINE : GameObject;
var aa4xtext : GameObject;
var aa4xtextLINE : GameObject;
var aa8xtext : GameObject;
var aa8xtextLINE : GameObject;

// sliders
var musicSlider : GameObject;
var sfxSlider : GameObject;
var sensitivityXSlider : GameObject;
var sensitivityYSlider : GameObject;
var mouseSmoothSlider : GameObject;

private var sliderValue : float = 0.0;
private var sliderValueSFX : float = 0.0;
private var sliderValueXSensitivity : float = 0.0;
private var sliderValueYSensitivity : float = 0.0;
private var sliderValueSmoothing : float = 0.0;

function Start () {
	// check difficulty
	if(PlayerPrefs.GetInt("NormalDifficulty") == 1){
		//difficultynormaltext.GetComponent (Text).text = "NORMAL";
		difficultynormaltextLINE.gameObject.active = true;
		difficultyhardcoretextLINE.gameObject.active = false;
		//difficultyhardcoretext.GetComponent (Text).text = "hardcore";
	}
	else
	{
		//difficultynormaltext.GetComponent (Text).text = "normal";
		//difficultyhardcoretext.GetComponent (Text).text = "HARDCORE";
		difficultyhardcoretextLINE.gameObject.active = true;
		difficultynormaltextLINE.gameObject.active = false;

	}

	// check slider values
	musicSlider.GetComponent.<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
	sfxSlider.GetComponent.<Slider>().value = PlayerPrefs.GetFloat("SFXVolume");
	sensitivityXSlider.GetComponent.<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
	sensitivityYSlider.GetComponent.<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");
	mouseSmoothSlider.GetComponent.<Slider>().value = PlayerPrefs.GetFloat("MouseSmoothing");

	// check full screen
	if(Screen.fullScreen == true){
		fullscreentext.GetComponent (Text).text = "on";
	}
	else if(Screen.fullScreen == false){
		fullscreentext.GetComponent (Text).text = "off";
	}

	// check hud value
	if(PlayerPrefs.GetInt("ShowHUD")==0){
		showhudtext.GetComponent (Text).text = "off";
	}
	else{
		showhudtext.GetComponent (Text).text = "on";
	}

	// check tool tip value
	if(PlayerPrefs.GetInt("ToolTips")==0){
		tooltipstext.GetComponent (Text).text = "off";
	}
	else{
		tooltipstext.GetComponent (Text).text = "on";
	}

	// check shadow distance/enabled
	if(PlayerPrefs.GetInt("Shadows") == 0){
		QualitySettings.shadowCascades = 0;
		QualitySettings.shadowDistance = 0;
		shadowofftext.GetComponent (Text).text = "OFF";
		shadowlowtext.GetComponent (Text).text = "low";
		shadowhightext.GetComponent (Text).text = "high";
		shadowofftextLINE.gameObject.active = true;
		shadowlowtextLINE.gameObject.active = false;
		shadowhightextLINE.gameObject.active = false;
	}
	else if(PlayerPrefs.GetInt("Shadows") == 1){
		QualitySettings.shadowCascades = 2;
		QualitySettings.shadowDistance = 75;
		shadowofftext.GetComponent (Text).text = "off";
		shadowlowtext.GetComponent (Text).text = "LOW";
		shadowhightext.GetComponent (Text).text = "high";
		shadowofftextLINE.gameObject.active = false;
		shadowlowtextLINE.gameObject.active = true;
		shadowhightextLINE.gameObject.active = false;
	}
	else if(PlayerPrefs.GetInt("Shadows") == 2){
		QualitySettings.shadowCascades = 4;
		QualitySettings.shadowDistance = 500;
		shadowofftext.GetComponent (Text).text = "off";
		shadowlowtext.GetComponent (Text).text = "low";
		shadowhightext.GetComponent (Text).text = "HIGH";
		shadowofftextLINE.gameObject.active = false;
		shadowlowtextLINE.gameObject.active = false;
		shadowhightextLINE.gameObject.active = true;
	}

	// check vsync
	if(QualitySettings.vSyncCount == 0){
		vsynctext.GetComponent (Text).text = "off";
	}
	else if(QualitySettings.vSyncCount == 1){
		vsynctext.GetComponent (Text).text = "on";
	}

	// check mouse inverse
	if(PlayerPrefs.GetInt("Inverted")==0){
		invertmousetext.GetComponent (Text).text = "off";
	}
	else if(PlayerPrefs.GetInt("Inverted")==1){
		invertmousetext.GetComponent (Text).text = "on";
	}

	// check motion blur
	if(PlayerPrefs.GetInt("MotionBlur")==0){
		motionblurtext.GetComponent (Text).text = "off";
	}
	else if(PlayerPrefs.GetInt("MotionBlur")==1){
		motionblurtext.GetComponent (Text).text = "on";
	}

	// check ambient occlusion
	if(PlayerPrefs.GetInt("AmbientOcclusion")==0){
		ambientocclusiontext.GetComponent (Text).text = "off";
	}
	else if(PlayerPrefs.GetInt("AmbientOcclusion")==1){
		ambientocclusiontext.GetComponent (Text).text = "on";
	}

	// check texture quality
	if(PlayerPrefs.GetInt("Textures") == 0){
		QualitySettings.masterTextureLimit = 2;
		texturelowtext.GetComponent (Text).text = "LOW";
		texturemedtext.GetComponent (Text).text = "med";
		texturehightext.GetComponent (Text).text = "high";
		texturelowtextLINE.gameObject.active = true;
		texturemedtextLINE.gameObject.active = false;
		texturehightextLINE.gameObject.active = false;
	}
	else if(PlayerPrefs.GetInt("Textures") == 1){
		QualitySettings.masterTextureLimit = 1;
		texturelowtext.GetComponent (Text).text = "low";
		texturemedtext.GetComponent (Text).text = "MED";
		texturehightext.GetComponent (Text).text = "high";
		texturelowtextLINE.gameObject.active = false;
		texturemedtextLINE.gameObject.active = true;
		texturehightextLINE.gameObject.active = false;
	}
	else if(PlayerPrefs.GetInt("Textures") == 2){
		QualitySettings.masterTextureLimit = 0;
		texturelowtext.GetComponent (Text).text = "low";
		texturemedtext.GetComponent (Text).text = "med";
		texturehightext.GetComponent (Text).text = "HIGH";
		texturelowtextLINE.gameObject.active = false;
		texturemedtextLINE.gameObject.active = false;
		texturehightextLINE.gameObject.active = true;
	}
}

function Update(){
	sliderValue = musicSlider.GetComponent.<Slider>().value;
	sliderValueSFX = sfxSlider.GetComponent.<Slider>().value;
	sliderValueXSensitivity = sensitivityXSlider.GetComponent.<Slider>().value;
	sliderValueYSensitivity = sensitivityYSlider.GetComponent.<Slider>().value;
	sliderValueSmoothing = mouseSmoothSlider.GetComponent.<Slider>().value;
}

function FullScreen(){
	Screen.fullScreen = !Screen.fullScreen;

	if(Screen.fullScreen == true){
		fullscreentext.GetComponent (Text).text = "on";
	}
	else if(Screen.fullScreen == false){
		fullscreentext.GetComponent (Text).text = "off";
	}
}

function MusicSlider(){
	PlayerPrefs.SetFloat("MusicVolume", sliderValue);
}

function SFXSlider(){
	PlayerPrefs.SetFloat("SFXVolume", sliderValueSFX);
}

function SensitivityXSlider(){
	PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
}

function SensitivityYSlider(){
	PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
}

function SensitivitySmoothing(){
	PlayerPrefs.SetFloat("MouseSmoothing", sliderValueSmoothing);
	Debug.Log(PlayerPrefs.GetFloat("MouseSmoothing"));
}

// the playerprefs variable that is checked to enable hud while in game
function ShowHUD(){
	if(PlayerPrefs.GetInt("ShowHUD")==0){
		PlayerPrefs.SetInt("ShowHUD",1);
		showhudtext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("ShowHUD")==1){
		PlayerPrefs.SetInt("ShowHUD",0);
		showhudtext.GetComponent (Text).text = "off";
	}
}

// show tool tips like: 'How to Play' control pop ups
function ToolTips(){
	if(PlayerPrefs.GetInt("ToolTips")==0){
		PlayerPrefs.SetInt("ToolTips",1);
		tooltipstext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("ToolTips")==1){
		PlayerPrefs.SetInt("ToolTips",0);
		tooltipstext.GetComponent (Text).text = "off";
	}
}

function NormalDifficulty(){
	//difficultynormaltext.GetComponent (Text).text = "NORMAL";
	//difficultyhardcoretext.GetComponent (Text).text = "hardcore";
	difficultyhardcoretextLINE.gameObject.active = false;
	difficultynormaltextLINE.gameObject.active = true;
	PlayerPrefs.SetInt("NormalDifficulty",1);
	PlayerPrefs.SetInt("HardCoreDifficulty",0);
}

function HardcoreDifficulty(){
	//difficultynormaltext.GetComponent (Text).text = "normal";
	//difficultyhardcoretext.GetComponent (Text).text = "HARDCORE";
	difficultyhardcoretextLINE.gameObject.active = true;
	difficultynormaltextLINE.gameObject.active = false;
	PlayerPrefs.SetInt("NormalDifficulty",0);
	PlayerPrefs.SetInt("HardCoreDifficulty",1);
}

function ShadowsOff(){
	PlayerPrefs.SetInt("Shadows",0);
	QualitySettings.shadowCascades = 0;
	QualitySettings.shadowDistance = 0;
	shadowofftext.GetComponent (Text).text = "OFF";
	shadowlowtext.GetComponent (Text).text = "low";
	shadowhightext.GetComponent (Text).text = "high";
	shadowofftextLINE.gameObject.active = true;
	shadowlowtextLINE.gameObject.active = false;
	shadowhightextLINE.gameObject.active = false;
}

function ShadowsLow(){
	PlayerPrefs.SetInt("Shadows",1);
	QualitySettings.shadowCascades = 2;
	QualitySettings.shadowDistance = 75;
	shadowofftext.GetComponent (Text).text = "off";
	shadowlowtext.GetComponent (Text).text = "LOW";
	shadowhightext.GetComponent (Text).text = "high";
	shadowofftextLINE.gameObject.active = false;
	shadowlowtextLINE.gameObject.active = true;
	shadowhightextLINE.gameObject.active = false;
}

function ShadowsHigh(){
	PlayerPrefs.SetInt("Shadows",2);
	QualitySettings.shadowCascades = 4;
	QualitySettings.shadowDistance = 500;
	shadowofftext.GetComponent (Text).text = "off";
	shadowlowtext.GetComponent (Text).text = "low";
	shadowhightext.GetComponent (Text).text = "HIGH";
	shadowofftextLINE.gameObject.active = false;
	shadowlowtextLINE.gameObject.active = false;
	shadowhightextLINE.gameObject.active = true;
}

function vsync(){
	if(QualitySettings.vSyncCount == 0){
		QualitySettings.vSyncCount = 1;
		vsynctext.GetComponent (Text).text = "on";
	}
	else if(QualitySettings.vSyncCount == 1){
		QualitySettings.vSyncCount = 0;
		vsynctext.GetComponent (Text).text = "off";
	}
}

function InvertMouse(){
	if(PlayerPrefs.GetInt("Inverted")==0){
		PlayerPrefs.SetInt("Inverted",1);
		invertmousetext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("Inverted")==1){
		PlayerPrefs.SetInt("Inverted",0);
		invertmousetext.GetComponent (Text).text = "off";
	}
}

function MotionBlur(){
	if(PlayerPrefs.GetInt("MotionBlur")==0){
		PlayerPrefs.SetInt("MotionBlur",1);
		motionblurtext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("MotionBlur")==1){
		PlayerPrefs.SetInt("MotionBlur",0);
		motionblurtext.GetComponent (Text).text = "off";
	}
}

function AmbientOcclusion(){
	if(PlayerPrefs.GetInt("AmbientOcclusion")==0){
		PlayerPrefs.SetInt("AmbientOcclusion",1);
		ambientocclusiontext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("AmbientOcclusion")==1){
		PlayerPrefs.SetInt("AmbientOcclusion",0);
		ambientocclusiontext.GetComponent (Text).text = "off";
	}
}

function CameraEffects(){
	if(PlayerPrefs.GetInt("CameraEffects")==0){
		PlayerPrefs.SetInt("CameraEffects",1);
		cameraeffectstext.GetComponent (Text).text = "on";
	}
	else if(PlayerPrefs.GetInt("CameraEffects")==1){
		PlayerPrefs.SetInt("CameraEffects",0);
		cameraeffectstext.GetComponent (Text).text = "off";
	}
}

function TexturesLow(){
	PlayerPrefs.SetInt("Textures",0);
	QualitySettings.masterTextureLimit = 2;
	texturelowtext.GetComponent (Text).text = "LOW";
	texturemedtext.GetComponent (Text).text = "med";
	texturehightext.GetComponent (Text).text = "high";
	texturelowtextLINE.gameObject.active = true;
	texturemedtextLINE.gameObject.active = false;
	texturehightextLINE.gameObject.active = false;
}

function TexturesMed(){
	PlayerPrefs.SetInt("Textures",1);
	QualitySettings.masterTextureLimit = 1;
	texturelowtext.GetComponent (Text).text = "low";
	texturemedtext.GetComponent (Text).text = "MED";
	texturehightext.GetComponent (Text).text = "high";
	texturelowtextLINE.gameObject.active = false;
	texturemedtextLINE.gameObject.active = true;
	texturehightextLINE.gameObject.active = false;
}

function TexturesHigh(){
	PlayerPrefs.SetInt("Textures",2);
	QualitySettings.masterTextureLimit = 0;
	texturelowtext.GetComponent (Text).text = "low";
	texturemedtext.GetComponent (Text).text = "med";
	texturehightext.GetComponent (Text).text = "HIGH";
	texturelowtextLINE.gameObject.active = false;
	texturemedtextLINE.gameObject.active = false;
	texturehightextLINE.gameObject.active = true;
}