using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private SpriteRenderer m_SpriteRenderer;
    Color m_newColor;

    void Start() {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
        m_SpriteRenderer.color = Color.white;
        m_newColor = Color.gray;
    }
    public void ColorCube() {
       m_SpriteRenderer.color = m_newColor;
    }
}
