using System.Collections;
using UnityEngine;
using MLAPI;

namespace Assets.Scripts
{
    public class GameScene : MonoSingleton<GameScene>
    {
        public GameObject gameManagerprefab;
        private void Start()
        {
            if(NetworkManager.Singleton.ConnectedClients.TryGetValue(NetworkManager.Singleton.ServerClientId, out var networkClient))
            {
                var player = networkClient.PlayerObject.GetComponent<PlayerController>();
                    if(player)
                {
                    player.criateGameManager();
                }
            }
        }

    }
}