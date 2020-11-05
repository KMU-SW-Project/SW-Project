using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.Extras;

public enum targetName
{
    Button,
    Key
}

public class ClickEvent : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;

    readonly Color32 normalColor = new Color32(255, 255, 255,255);
    readonly Color32 highlightColor = new Color32(255, 185, 184, 255);
    readonly Color32 pressColor = new Color32(255, 119, 119, 255);

    void Awake()
    {
        laserPointer = GetComponent<SteamVR_LaserPointer>();

        // 핸들러 추가
        laserPointer.PointerIn += PointerInside;
        laserPointer.PointerOut += PointerOutside;
        laserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        // 버튼이면 이미지 색 교체
        if (e.target.name.Contains(targetName.Button.ToString()))
        {
            Image image = e.target.GetComponent<Image>();
            image.color = pressColor;            
        }

        // 각 버튼 이벤트 실행
        if (e.target.name.Contains(targetName.Button.ToString()) || e.target.name.Contains(targetName.Key.ToString()))
            e.target.GetComponent<Button>().onClick.Invoke();
    }

    public void PointerInside(object sender, PointerEventArgs e)
    { 
        // 버튼이면 이미지 색 교체
        if (e.target.name.Contains(targetName.Button.ToString()) || e.target.name.Contains(targetName.Key.ToString()))
        {
            Image image = e.target.GetComponent<Image>();
           image.color = highlightColor;
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        // 버튼이면 이미지 색 교체
        if (e.target.name.Contains(targetName.Button.ToString()) || e.target.name.Contains(targetName.Key.ToString()))
        {
            Image image = e.target.GetComponent<Image>();
            image.color = normalColor;
        }
    }
}
