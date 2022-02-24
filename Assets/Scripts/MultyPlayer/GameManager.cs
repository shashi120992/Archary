using System.Collections;
using UnityEngine;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




namespace Assets.Scripts
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private GameObject bowprefab;
        [SerializeField] private GameObject bowstringprefab;
        [SerializeField] private GameObject arrawprefab;
        //[SerializeField] private GameObject arrawtextprefab;
        //[SerializeField] private GameObject scoreprefab;
        public int arrows = 20;
        public int score = 0;
        GameObject arrow;
        bool arrowShot;
        bool arrowPrepared;
        
        //public Canvas menuCanvas, gameCanvas, gameOverCanvas;
        //public Text arrowText;
        //public Text scoreText;
        //public Text endscoreText;
        //public Text actualHighscoreText;
        //public Text newHighscoreText;
        //public Text newHighText;
        
        private List<Vector3> bowStringPosition;
        private LineRenderer bowStringLinerenderer;
        float arrowStartX;
        float length;
        Vector3 stringPullout;
        
        private Ray mouseRay1;
        private RaycastHit rayHit;
        private float posX;
        private float posY;

        //Vector3 stringRestPosition = new Vector3(-0.44f, -0.06f, 2f);
        Vector3 stringRestPosition = new Vector3(0f, 0f, 0f);
        public GameStates gameState = GameStates.game;
        public enum GameStates
        {
            menu,
            instructions,
            game,
            over,
            hiscore,
        };
        public override void NetworkStart()
        {
            
            if (IsServer)
            {
                
                SpawnAllPlayers();
            }
        }

        void Update()
        {
            switch (gameState)
            {
                case GameStates.game:
                    // set UI related stuff
                    //showArrows();
                    //showScore();
                    if (Input.GetMouseButton(0))
                    {
                        prepareArrow();
                    }

                    // ok, player released the mouse
                    // (player released the touch on android)
                    if (Input.GetMouseButtonUp(0) && arrowPrepared)
                    {
                        shootArrow();
                    }
                    break;
                case GameStates.instructions:
                    break;
                case GameStates.over:
                    break;
                case GameStates.hiscore:
                    break;
            }
        }


        private void SpawnAllPlayers()
        {
            int playerIndex = 0;
            foreach (KeyValuePair<ulong, NetworkClient> nc in NetworkManager.Singleton.ConnectedClients)
            {
                PlayerController pc = nc.Value.PlayerObject.GetComponent<PlayerController>();
                for (int i = 0; i< 2; i++)
                {
                    GameObject go1 = GameObject.Instantiate(bowprefab);
                    go1.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);

                    GameObject go2 = GameObject.Instantiate(bowstringprefab);
                    go2.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);

                    GameObject go3 = GameObject.Instantiate(arrawprefab);
                    go3.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
                    
                    //GameObject go4 = GameObject.Instantiate(arrawtextprefab);
                    //go4.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
                    
                    //GameObject go5 = GameObject.Instantiate(scoreprefab);
                    //go5.GetComponent<NetworkObject>().SpawnWithOwnership(nc.Key);
                }
                playerIndex++;
            }
        }

        public void initScore()
        {
            if (!PlayerPrefs.HasKey("Score"))
                PlayerPrefs.SetInt("Score", 0);
        }

        /*
        public void showScore()
        {
            scoreText.text = "Score: " + score.ToString();
        }


        public void showArrows()
        {
            arrowText.text = "Arrows: " + arrows.ToString();
        }

        */
        void resetGame()
        {
            arrows = 3;
            score = 0;
            if (GameObject.Find("arrow") == null)
                createArrow(true);
        }


        public void createArrow(bool hitTarget)
        {
            Camera.main.GetComponent<camMovement>().resetCamera();
            
            if (arrows > 0)
            {
                
                this.transform.localRotation = Quaternion.identity;
                arrow = Instantiate(arrawprefab, Vector3.zero, Quaternion.identity) as GameObject;
                arrow.name = "arrow";
                arrow.transform.localScale = this.transform.localScale;
                arrow.transform.localPosition = this.transform.position + new Vector3(0.7f, 0, 0);
                arrow.transform.localRotation = this.transform.localRotation;
                arrow.transform.parent = this.transform;
                arrow.GetComponent<rotateArrow>().setBow(gameObject);
                arrowShot = false;
                arrowPrepared = false;
                arrows--;
            }
            else
            {
                // no arrow is left,
                // so the game is over
                //gameState = GameStates.over;
                //gameOverCanvas.enabled = true;
                //endscoreText.text = "You shot all the arrows and scored " + score + " points.";
            }
        }

        public void shootArrow()
        {
            if (arrow.GetComponent<Rigidbody>() == null)
            {
                arrowShot = true;
                arrow.AddComponent<Rigidbody>();
                //arrow.transform.parent = gameManager.transform;
                arrow.GetComponent<Rigidbody>().AddForce(Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z)) * new Vector3(25f * length, 0, 0), ForceMode.VelocityChange);
            }
            arrowPrepared = false;
            stringPullout = stringRestPosition;

            // Cam
            Camera.main.GetComponent<camMovement>().resetCamera();
            Camera.main.GetComponent<camMovement>().setArrow(arrow);

        }


        public void prepareArrow()
        {
            // get the touch point on the screen
            mouseRay1 = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay1, out rayHit, 1000f) && arrowShot == false)
            {
                // determine the position on the screen
                posX = this.rayHit.point.x;
                posY = this.rayHit.point.y;
                // set the bows angle to the arrow
                Vector2 mousePos = new Vector2(transform.position.x - posX, transform.position.y - posY);
                float angleZ = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(0, 0, angleZ);
                // determine the arrow pullout
                length = mousePos.magnitude / 3f;
                length = Mathf.Clamp(length, 0, 1);
                // set the bowstrings line renderer
                stringPullout = new Vector3(-(0.44f + length), -0.06f, 2f);
                // set the arrows position
                Vector3 arrowPosition = arrow.transform.localPosition;
                arrowPosition.x = (arrowStartX - length);
                arrow.transform.localPosition = arrowPosition;
            }
            arrowPrepared = true;
        }

       

    }
}