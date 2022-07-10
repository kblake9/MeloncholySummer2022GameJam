using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panel_pauseMenu;

    private void Start()
    {
        panel_pauseMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
}
