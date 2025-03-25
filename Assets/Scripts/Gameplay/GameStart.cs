using Unity.Netcode;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
    }

    private void Singleton_OnClientConnectedCallback(ulong obj)
    {
        throw new System.NotImplementedException();
    }

    public void ConnectAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void ConnectAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
