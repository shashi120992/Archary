using Assets.Scripts;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    // Networked fields
    public NetworkVariableString playerName = new NetworkVariableString(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });
    public NetworkVariableColor playerColor = new NetworkVariableColor(new NetworkVariableSettings
    {
        WritePermission = NetworkVariablePermission.OwnerOnly,
        ReadPermission = NetworkVariablePermission.Everyone
    });

    // Fields
    private GameObject myPlayerListItem;
    private TextMeshProUGUI playerNameLabel;
    

    public override void NetworkStart()
    {
        RegisterEvents();

        Debug.Log($"NetworkStart:: {NetworkManager.Singleton.LocalClientId} OWNER:{OwnerClientId} OS:{IsOwnedByServer}");
        myPlayerListItem = Instantiate(LobbyScene.Instance.playerListItemPrefab, Vector3.zero, Quaternion.identity);
        myPlayerListItem.transform.SetParent(LobbyScene.Instance.playerListContainer, false);

        playerNameLabel = myPlayerListItem.GetComponentInChildren<TextMeshProUGUI>();

        if (IsOwner)
        {
            playerName.Value = UnityEngine.Random.Range(1000, 9999).ToString();
            playerColor.Value = new Color(UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f), UnityEngine.Random.Range(0.0f, 1.0f));
        }
        else
        {
            playerNameLabel.text = playerName.Value;
        }
    }
    public void OnDestroy()
    {
        Destroy(myPlayerListItem);
        UnregisterEvents();
    }

    public void criateGameManager()
    {
        if(IsServer)
        {
            GameObject go = Instantiate(GameScene.Instance.gameManagerprefab);
            go.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        }
    }

    public void ChangeName(string newName)
    {
        if (IsOwner)
            playerName.Value = newName;
    }

    // Events
    private void RegisterEvents()
    {
        playerName.OnValueChanged += OnPlayerNameChange;
    }
    private void UnregisterEvents()
    {
        playerName.OnValueChanged -= OnPlayerNameChange;
    }

    private void OnPlayerNameChange(string previousValue, string newValue)
    {
        playerNameLabel.text = playerName.Value;
    }
}
