#pragma strict
import UnityEngine.UI;

var CameraObject : Animator;
var PanelControls : GameObject;
var PanelVideo : GameObject;
var PanelGame : GameObject;
var PanelKeyBindings : GameObject;
var PanelMovement : GameObject;
var PanelCombat : GameObject;
var PanelGeneral : GameObject;
var hoverSound : GameObject;
var sfxhoversound : GameObject;
var clickSound : GameObject;
var areYouSure : GameObject;

// campaign button sub menu
var continueBtn : GameObject;
var newGameBtn : GameObject;
var loadGameBtn : GameObject;

// highlights
var lineGame : GameObject;
var lineVideo : GameObject;
var lineControls : GameObject;
var lineKeyBindings : GameObject;
var lineMovement : GameObject;
var lineCombat : GameObject;
var lineGeneral : GameObject;

function PlayCampaign(){
	areYouSure.gameObject.SetActive(false);
	continueBtn.gameObject.SetActive(true);
	newGameBtn.gameObject.SetActive(true);
	loadGameBtn.gameObject.SetActive(true);
}

function DisablePlayCampaign(){
	continueBtn.gameObject.SetActive(false);
	newGameBtn.gameObject.SetActive(false);
	loadGameBtn.gameObject.SetActive(false);
}

function Position2(){
	DisablePlayCampaign();
	CameraObject.SetFloat("Animate",1);
}

function Position1(){
	CameraObject.SetFloat("Animate",0);
}

function GamePanel(){
	PanelControls.gameObject.SetActive(false);
	PanelVideo.gameObject.SetActive(false);
	PanelGame.gameObject.SetActive(true);
	PanelKeyBindings.gameObject.SetActive(false);

	lineGame.gameObject.SetActive(true);
	lineControls.gameObject.SetActive(false);
	lineVideo.gameObject.SetActive(false);
	lineKeyBindings.gameObject.SetActive(false);
}

function VideoPanel(){
	PanelControls.gameObject.SetActive(false);
	PanelVideo.gameObject.SetActive(true);
	PanelGame.gameObject.SetActive(false);
	PanelKeyBindings.gameObject.SetActive(false);

	lineGame.gameObject.SetActive(false);
	lineControls.gameObject.SetActive(false);
	lineVideo.gameObject.SetActive(true);
	lineKeyBindings.gameObject.SetActive(false);
}

function ControlsPanel(){
	PanelControls.gameObject.SetActive(true);
	PanelVideo.gameObject.SetActive(false);
	PanelGame.gameObject.SetActive(false);
	PanelKeyBindings.gameObject.SetActive(false);

	lineGame.gameObject.SetActive(false);
	lineControls.gameObject.SetActive(true);
	lineVideo.gameObject.SetActive(false);
	lineKeyBindings.gameObject.SetActive(false);
}

function KeyBindingsPanel(){
	PanelControls.gameObject.SetActive(false);
	PanelVideo.gameObject.SetActive(false);
	PanelGame.gameObject.SetActive(false);
	PanelKeyBindings.gameObject.SetActive(true);

	lineGame.gameObject.SetActive(false);
	lineControls.gameObject.SetActive(false);
	lineVideo.gameObject.SetActive(true);
	lineKeyBindings.gameObject.SetActive(true);
}

function MovementPanel(){
	PanelMovement.gameObject.SetActive(true);
	PanelCombat.gameObject.SetActive(false);
	PanelGeneral.gameObject.SetActive(false);

	lineMovement.gameObject.SetActive(true);
	lineCombat.gameObject.SetActive(false);
	lineGeneral.gameObject.SetActive(false);
}

function CombatPanel(){
	PanelMovement.gameObject.SetActive(false);
	PanelCombat.gameObject.SetActive(true);
	PanelGeneral.gameObject.SetActive(false);

	lineMovement.gameObject.SetActive(false);
	lineCombat.gameObject.SetActive(true);
	lineGeneral.gameObject.SetActive(false);
}

function GeneralPanel(){
	PanelMovement.gameObject.SetActive(false);
	PanelCombat.gameObject.SetActive(false);
	PanelGeneral.gameObject.SetActive(true);

	lineMovement.gameObject.SetActive(false);
	lineCombat.gameObject.SetActive(false);
	lineGeneral.gameObject.SetActive(true);
}

function PlayHover(){
	hoverSound.GetComponent.<AudioSource>().Play();
}

function PlaySFXHover(){
	sfxhoversound.GetComponent.<AudioSource>().Play();
}

function PlayClick(){
	clickSound.GetComponent.<AudioSource>().Play();
}

function AreYouSure(){
	areYouSure.gameObject.SetActive(true);
	DisablePlayCampaign();
}

function No(){
	areYouSure.gameObject.SetActive(false);
}

function Yes(){
	Application.Quit();
}
