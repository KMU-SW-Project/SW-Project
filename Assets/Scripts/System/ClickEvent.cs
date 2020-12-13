using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

public enum targetName
{
    Button,
    Key
}

public class ClickEvent : MonoBehaviour
{
    public SteamVR_LaserPointer laserPointer;
    public SteamVR_Action_Boolean gunTrigger;
    public SteamVR_Input_Sources handType;

    [SerializeField] private Transform effectPos;
    [SerializeField] private GameObject bulletMark;
    [SerializeField] private LineRenderer lineRenderer;
    private AudioSource _gunShotSound;
    private ParticleSystem _tempeffect;

    readonly Color32 normalColor = new Color32(255, 255, 255, 255);
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

    private void Start()
    {
        _gunShotSound = GetComponent<AudioSource>();

        gunTrigger.AddOnStateDownListener(TriggerDown, handType);
        gunTrigger.AddOnStateUpListener(TriggerUp, handType);
    }

    private void Update()
    {
        if (!GameManager.GetInstance().gunGuide)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, lineRenderer.transform.position);

        if (laserPointer.bHit) lineRenderer.SetPosition(1, laserPointer.hitPoint);
        else lineRenderer.SetPosition(1, lineRenderer.transform.position + (transform.forward * 50));
    }


    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        Debug.Log("Trigger is up");
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        if ((BackendServerManager.GetInstance().accountData.handType == HandType.left.ToString()
            && fromSource == SteamVR_Input_Sources.RightHand)
            ||
            (BackendServerManager.GetInstance().accountData.handType == HandType.right.ToString()
            && fromSource == SteamVR_Input_Sources.LeftHand)) return;

        _gunShotSound.Play();

        _tempeffect = Player.instance.GetEffect();
        _tempeffect.transform.position = effectPos.position;
        _tempeffect.transform.rotation = transform.rotation;
        _tempeffect.Play();

        Player.instance.ReturnEffect(_tempeffect);

        if (laserPointer.hitPoint != Vector3.zero)
        {
            _tempeffect = Player.instance.GetMark();
            _tempeffect.transform.position = laserPointer.hitPoint;
            _tempeffect.transform.rotation = transform.rotation;
            _tempeffect.Play();

            Player.instance.ReturnMark(_tempeffect);
        }

        Debug.Log("Trigger is down");
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        print(e.target.name);
        // 버튼이면 이미지 색 교체
        if (e.target.name.Contains(targetName.Button.ToString()))
        {
            Image image = e.target.GetComponent<Image>();
            image.color = pressColor;
        }
        // 타겟이면(사격장 모드)
        else if (e.target.name.Contains("Target") && GameManager.GetInstance().modeData.currentPlayMode == GameMode.Training)
        {
            e.target.GetComponent<Target>().HitTarget();
        }
        // 캐릭터면(무한모드, 현상금모드)
        else if (e.target.name.Contains("Character")
            && (GameManager.GetInstance().modeData.currentPlayMode == GameMode.Bounty
            || GameManager.GetInstance().modeData.currentPlayMode == GameMode.Infinity))
        {
            Destroy(e.target.gameObject);
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
