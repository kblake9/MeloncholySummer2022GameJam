using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel_pauseMenu;

    private void Start()
    {
        panel_pauseMenu.SetActive(false);
        PlayerController.Instance.PIA.External.Pause.started += PauseInput;
        PlayerController.Instance.PIA.External.Enable();
    }

    private void PauseInput(InputAction.CallbackContext context)
    {
        panel_pauseMenu.SetActive(!panel_pauseMenu.activeInHierarchy);

        if (panel_pauseMenu.activeInHierarchy)
        {
            PlayerController.Instance.PIA.Player.Disable();
        }
        else
        {
            PlayerController.Instance.PIA.Player.Enable();
        }
    }
}
