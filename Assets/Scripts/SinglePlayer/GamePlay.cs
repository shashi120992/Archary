using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GamePlay : MonoBehaviour {
	
	private Ray mouseRay1;
	private RaycastHit rayHit;
	private float posX, posY;
	GameObject arrow;
	public GameObject bowString, arrowPrefab,gameManager,risingText, target;//Game Objects
	public AudioClip stringPull, stringRelease, arrowSwoosh;//Audio Clip
	bool stringPullSoundPlayed, stringReleaseSoundPlayed, arrowSwooshSoundPlayed;// Bool Variable
	private List<Vector3> bowStringPosition;
	LineRenderer bowStringLinerenderer;
	float arrowStartX, length;
	bool arrowShot, arrowPrepared;
	Vector3 stringPullout;
	Vector3 stringRestPosition = new Vector3 (-0.44f, -0.06f, 2f);

	// game states
	public enum GameStates {
		menu,
		game,
		over,
	};
	public GameStates gameState = GameStates.menu;
	public Canvas menuCanvas, gameCanvas, gameOverCanvas;
	public Text nameText, arrowText, scoreText, endscoreText, actualHighscoreText, newHighscoreText, newHighText;
	public int arrows = 3;
	public int score = 0;
	public string playerName;
	public InputField input;

	void resetGame() {
		arrows = 3;
		score = 0;
		if (GameObject.Find("arrow") == null)
			createArrow (true);
	}


	// Use this for initialization
	void Start () {
		menuCanvas.enabled = true;
		gameCanvas.enabled = false;
		gameOverCanvas.enabled = false;
		initScore ();

		// create an arrow to shoot
		createArrow (true);

		// setup the line renderer representing the bowstring (Reff Code)
		bowStringLinerenderer = bowString.AddComponent<LineRenderer>();
		bowStringLinerenderer.SetVertexCount(3);//not working in multiplayer
		bowStringLinerenderer.SetWidth(0.05F, 0.05F);
		bowStringLinerenderer.useWorldSpace = false;
		bowStringLinerenderer.material = Resources.Load ("Materials/bowStringMaterial") as Material;
		bowStringPosition = new List<Vector3> ();
		bowStringPosition.Add(new Vector3 (-0.44f, 1.43f, 2f));
		bowStringPosition.Add(new Vector3 (-0.44f, -0.06f, 2f));
		bowStringPosition.Add(new Vector3 (-0.43f, -1.32f, 2f));
		bowStringLinerenderer.SetPosition (0, bowStringPosition [0]);
		bowStringLinerenderer.SetPosition (1, bowStringPosition [1]);
		bowStringLinerenderer.SetPosition (2, bowStringPosition [2]);
		arrowStartX = 0.7f;
		stringPullout = stringRestPosition;
	}

	//update in multiplayer
	void Update () {
		switch (gameState) {
		case GameStates.menu:
			if (Input.GetKeyDown(KeyCode.Escape)) {
				Application.Quit();
			}
			break;

		case GameStates.game:
			showArrows();
			showScore();
			showName();

			// return to main menu when back key is pressed (android)
			if (Input.GetKeyDown(KeyCode.Escape)) {
				showMenu();
			}

			// game is steered via mouse
			if (Input.GetMouseButton(0)) {
				if (!stringPullSoundPlayed) {
					// play sound
					GetComponent<AudioSource>().PlayOneShot(stringPull);
					stringPullSoundPlayed = true;
				}
				// detrmine the pullout and set up the arrow
				prepareArrow();
			}

			// ok, player released the mouse
			if (Input.GetMouseButtonUp (0) && arrowPrepared) {
				// play string sound
				if (!stringReleaseSoundPlayed) {
					GetComponent<AudioSource>().PlayOneShot(stringRelease);
					stringReleaseSoundPlayed = true;
				}
				// play arrow sound
				if (!arrowSwooshSoundPlayed) {
					GetComponent<AudioSource>().PlayOneShot(arrowSwoosh);
					arrowSwooshSoundPlayed = true;
				}
				// shot the arrow (rigid body physics)
				shootArrow();
			}
			// in any case: update the bowstring line renderer
			drawBowString();
			break;
		case GameStates.over:
			break;
		}
	}

	public void initScore() {
		if (!PlayerPrefs.HasKey ("Score"))
			PlayerPrefs.SetInt ("Score", 0);
	}

	public void showName()
	{
		nameText.text = "Name: " + input.text;
	}
	public void showScore() {
		scoreText.text = "Score: " + score.ToString();
	}

	public void showArrows() {
		arrowText.text = "Arrows: " + arrows.ToString ();
	}


	public void createArrow(bool hitTarget) {
		Camera.main.GetComponent<camMovement> ().resetCamera ();
		// when a new arrow is created
		stringPullSoundPlayed = false;
		stringReleaseSoundPlayed = false;
		arrowSwooshSoundPlayed = false;
		// does the player has an arrow left ?
		if (arrows > 0) {
			// now instantiate a new arrow
			this.transform.localRotation = Quaternion.identity;
			arrow = Instantiate (arrowPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			arrow.name = "arrow";
			arrow.transform.localScale = this.transform.localScale;
			arrow.transform.localPosition = this.transform.position + new Vector3 (0.7f, 0, 0);
			arrow.transform.localRotation = this.transform.localRotation;
			arrow.transform.parent = this.transform;
			// transmit a reference to the arrow script
			arrow.GetComponent<rotateArrow> ().setBow (gameObject);
			arrowShot = false;
			arrowPrepared = false;
			// subtract one arrow
			arrows --;
		}
		else {
			// no arrow is left so the game is over
			gameState = GameStates.over;
			gameOverCanvas.enabled = true;
			endscoreText.text = "You shot all the arrows and scored " + score + " points.";
		}
	}

	public void shootArrow() {
		if (arrow.GetComponent<Rigidbody>() == null) {
			arrowShot = true;
			arrow.AddComponent<Rigidbody>();
			arrow.transform.parent = gameManager.transform;
			arrow.GetComponent<Rigidbody>().AddForce (Quaternion.Euler (new Vector3(transform.rotation.eulerAngles.x,transform.rotation.eulerAngles.y,transform.rotation.eulerAngles.z))*new Vector3(25f*length,0,0), ForceMode.VelocityChange);
		}
		arrowPrepared = false;
		stringPullout = stringRestPosition;

		// Camera movement
		Camera.main.GetComponent<camMovement> ().resetCamera ();
		Camera.main.GetComponent<camMovement> ().setArrow (arrow);

	}

	public void prepareArrow() {
		mouseRay1 = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(mouseRay1, out rayHit, 1000f) && arrowShot == false)
		{
			// determine the position on the screen
			posX = this.rayHit.point.x;
			posY = this.rayHit.point.y;
			// set the bows angle to the arrow
			Vector2 mousePos = new Vector2(transform.position.x-posX,transform.position.y-posY);
			float angleZ = Mathf.Atan2(mousePos.y,mousePos.x)*Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0,0,angleZ);
			// determine the arrow pullout
			length = mousePos.magnitude / 3f;
			length = Mathf.Clamp(length,0,1);
			// set the bowstrings line renderer
			stringPullout = new Vector3(-(0.44f+length), -0.06f, 2f);
			// set the arrows position
			Vector3 arrowPosition = arrow.transform.localPosition;
			arrowPosition.x = (arrowStartX - length);
			arrow.transform.localPosition = arrowPosition;
		}
		arrowPrepared = true;
	}

	public void drawBowString() {
		bowStringLinerenderer = bowString.GetComponent<LineRenderer>();
		bowStringLinerenderer.SetPosition (0, bowStringPosition [0]);
		bowStringLinerenderer.SetPosition (1, stringPullout);
		bowStringLinerenderer.SetPosition (2, bowStringPosition [2]);
	}
	

	public void setPoints(int points){
		score += points;
		if (points == 50) {
			arrows++;
			GameObject rt1 = (GameObject)Instantiate(risingText, new Vector3(0,0,0),Quaternion.identity);
			rt1.transform.position = this.transform.position + new Vector3(0,0,0);
			rt1.transform.name = "rt1";
			// each target's "ring" is 0.07f wide
			// so it's relatively simple to calculate the ring hit (thus the score)
			rt1.GetComponent<TextMesh>().text= "Bonus arrow";
		}
	}
	public void startGame() {
		menuCanvas.enabled = false;
		gameCanvas.enabled = true;
		gameState = GameStates.game;
	}
	public void showMenu() {
		menuCanvas.enabled = true;
		gameState = GameStates.menu;
		gameOverCanvas.enabled = false;
		resetGame ();
	}
	public void ChangeName(string newName)
	{
	
		playerName = input.text;
	}
}
