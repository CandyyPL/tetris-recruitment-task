using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public InputActions inputActions;

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

        inputActions = new InputActions();
        inputActions.Default.Enable();
    }
}
