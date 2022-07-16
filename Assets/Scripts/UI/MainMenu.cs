using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject gear;
    public void StartButton()
    {
        gear.GetComponent<CanvasGroup>().alpha = 1;
        SceneManager.LoadSceneAsync("MuseumCutScene", LoadSceneMode.Single);
    }

    public void OptionsButton()
    {

    }

    public void QuitButton()
    {
        Application.Quit();
    }
}
