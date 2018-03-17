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
	areYouSure.gameObject.active = false;
	continueBtn.gameObject.active = true;
	newGameBtn.gameObject.active = true;
	loadGameBtn.gameObject.active = true;
}

function DisablePlayCampaign(){
	continueBtn.gameObject.active = false;
	newGameBtn.gameObject.active = false;
	loadGameBtn.gameObject.active = false;
}

function Position2(){
	DisablePlayCampaign();
	CameraObject.SetFloat("Animate",1);
}

function Position1(){
	CameraObject.SetFloat("Animate",0);
}

function GamePanel(){
	PanelControls.gameObject.active = false;
	PanelVideo.gameObject.active = false;
	PanelGame.gameObject.active = true;
	PanelKeyBindings.gameObject.active = false;

	lineGame.gameObject.active = true;
	lineControls.gameObject.active = false;
	lineVideo.gameObject.active = false;
	lineKeyBindings.gameObject.active = false;
}

function VideoPanel(){
	PanelControls.gameObject.active = false;
	PanelVideo.gameObject.active = true;
	PanelGame.gameObject.active = false;
	PanelKeyBindings.gameObject.active = false;

	lineGame.gameObject.active = false;
	lineControls.gameObject.active = false;
	lineVideo.gameObject.active = true;
	lineKeyBindings.gameObject.active = false;
}

function ControlsPanel(){
	PanelControls.gameObject.active = true;
	PanelVideo.gameObject.active = false;
	PanelGame.gameObject.active = false;
	PanelKeyBindings.gameObject.active = false;

	lineGame.gameObject.active = false;
	lineControls.gameObject.active = true;
	lineVideo.gameObject.active = false;
	lineKeyBindings.gameObject.active = false;
}

function KeyBindingsPanel(){
	PanelControls.gameObject.active = false;
	PanelVideo.gameObject.active = false;
	PanelGame.gameObject.active = false;
	PanelKeyBindings.gameObject.active = true;

	lineGame.gameObject.active = false;
	lineControls.gameObject.active = false;
	lineVideo.gameObject.active = true;
	lineKeyBindings.gameObject.active = true;
}

function MovementPanel(){
	PanelMovement.gameObject.active = true;
	PanelCombat.gameObject.active = false;
	PanelGeneral.gameObject.active = false;

	lineMovement.gameObject.active = true;
	lineCombat.gameObject.active = false;
	lineGeneral.gameObject.active = false;
}

function CombatPanel(){
	PanelMovement.gameObject.active = false;
	PanelCombat.gameObject.active = true;
	PanelGeneral.gameObject.active = false;

	lineMovement.gameObject.active = false;
	lineCombat.gameObject.active = true;
	lineGeneral.gameObject.active = false;
}

function GeneralPanel(){
	PanelMovement.gameObject.active = false;
	PanelCombat.gameObject.active = false;
	PanelGeneral.gameObject.active = true;

	lineMovement.gameObject.active = false;
	lineCombat.gameObject.active = false;
	lineGeneral.gameObject.active = true;
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
	areYouSure.gameObject.active = true;
	DisablePlayCampaign();
}

function No(){
	areYouSure.gameObject.active = false;
}

function Yes(){
	Application.Quit();
}
