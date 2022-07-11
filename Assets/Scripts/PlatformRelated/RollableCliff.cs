using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class RollableCliff : MonoBehaviour
{
    private BoxCollider2D rollableCollider;
    [SerializeField] private Transform topPlatform;
    [SerializeField] private Transform bottomPlatform;

    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerController controller = collision.transform.GetComponent<PlayerController>();
        
        if (controller != null)
        {
            controller.PIA.Player.RollDownCliff.started += OpenTopPlatform;
            controller.PIA.Player.RollDownCliff.Enable();
            controller.cliffBotPlat = bottomPlatform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.transform.GetComponent<PlayerController>()?.PIA.Player.RollDownCliff.Disable();
        topPlatform.GetComponent<BoxCollider2D>().enabled = true;
    }

    private void OpenTopPlatform(InputAction.CallbackContext context)
    {
        topPlatform.GetComponent<BoxCollider2D>().enabled = false;
    }
}
