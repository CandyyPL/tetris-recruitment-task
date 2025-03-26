using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    public Action<int> OnNextBlockChosen;
    public Action OnFallTimeDecrease;
    public Action OnLineClear;
    public Action OnGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
