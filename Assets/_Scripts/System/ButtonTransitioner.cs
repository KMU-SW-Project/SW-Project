using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTransitioner : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler
{
    public Color32 m_NormalColor = Color.white;
    public Color m_HoverColor = new Color(1, 0.725f, 0.721f);
    public Color m_DownColor = new Color(1, 0.46f, 0.46f);

    Image m_Image = null;

    // Start is called before the first frame update
    void Start()
    {
        m_Image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_Image.color = m_HoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_Image.color = m_NormalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_Image.color = m_DownColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print(gameObject.name);
        m_Image.color = m_HoverColor;
    }
}
