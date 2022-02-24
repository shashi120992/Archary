using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MultyPlayer
{
    public class Canwas : MonoBehaviour
    {
        public Canvas menuCanvas, gameCanvas, gameOverCanvas;

        private void Start()
        {
            menuCanvas.enabled = true;
            gameCanvas.enabled = false;
            gameOverCanvas.enabled = false;
        }

        public void startGame()
        {
            menuCanvas.enabled = false;
            gameCanvas.enabled = true;

        }
        public void exitButton()
        {
            SceneManager.LoadScene("Start");
        }

        public void exitgameiferrorOccrs()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                startGame();
            }
        }
    }
}