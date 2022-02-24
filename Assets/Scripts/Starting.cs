using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Starting : MonoBehaviour
    {
       public void playSinglePlayer()
        {
            SceneManager.LoadScene("Arcary(SInglePlayer)");
        }

        public void playmultyPlayer()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}