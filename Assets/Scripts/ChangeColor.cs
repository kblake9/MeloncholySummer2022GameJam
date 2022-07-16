using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private Renderer cubeRenderer;

    void Start() {
        cubeRenderer = GetComponent<Renderer>();
    }
    public void ColorCube() {
       cubeRenderer.material.color = new Color(0.5f, 1, 1);
    }
}
