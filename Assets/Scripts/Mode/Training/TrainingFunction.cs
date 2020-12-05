using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum TrainingMode
{
    RandomPos,
    FixedPos,
}

public class TrainingFunction : MonoBehaviour
{
    public GameObject player;
    public Transform[] randomPos;
    [HideInInspector] public TrainingMode currentMode;
    [SerializeField] private Transform _targetParent;
    [SerializeField] private Text _soundText;
    private bool _isMute;
    private int _targetType;
    private Target _target;

    IEnumerator co_RandomPos;

    private void Awake()
    {
        if (GameManager.GetInstance().playMode == GameMode.VRTest) player.SetActive(false);

        GameManager.GetInstance().SetPlayerCameraPosition(player.transform);
    }

    private void Start()
    {
        _isMute = true;

        _targetType = GameManager.GetInstance().modeData.currentSelectedTarget;
    }

    // 모드 변경에 대한 버튼 함수
    public void SelectTrainingMode(string mode)
    {
        ModeReset();

        if (co_RandomPos != null)
            StopCoroutine(co_RandomPos);

        _target = _targetParent.GetChild(_targetType).GetComponent<Target>();

        if (mode == TrainingMode.FixedPos.ToString())
        {
            currentMode = TrainingMode.FixedPos;
            FixedPosTarget();
            return;
        }
        else if (mode == TrainingMode.RandomPos.ToString()) currentMode = TrainingMode.RandomPos;
      
       co_RandomPos = DelayStart();
        StartCoroutine(co_RandomPos);
    }

    // 랜덤으로 타겟의 위치를 바꿔줌
    IEnumerator DelayStart()
    {
        // 각 모드 시작시 머리통에 text 띄워서 아래 말 알려주기
        // 잠시후 시작합니다~~
        print("랜덤 잠시후 시작");

        yield return new WaitForSeconds(3.0f);

        print("랜덤 시작");
        _targetParent.GetChild(_targetType).gameObject.SetActive(true);
        RandomPosTarget();

        while (true)
        {
            yield return null;
            if (_target == null) break;            
            if (_target.active) continue;           

            yield return new WaitForSeconds(Random.Range(1,2.5f));
            RandomPosTarget();
        }
    }
    
    // 랜덤좌표 타겟
    void RandomPosTarget()
    {
        _targetParent.GetChild(_targetType).position = randomPos[Random.Range(0, randomPos.Length)].position - new Vector3(0,3,0);
        _target.active = true;
    }

    // 고정좌표 타겟
     void FixedPosTarget()
    {        
        _targetParent.GetChild(_targetType).gameObject.SetActive(true);
        _target.active = true;
        _target.isFixed = true;
    }

    // 사격장 모드 초기화
    void ModeReset()
    {
        if (_target == null) return;
        
        for (int i = 0; i < _targetParent.childCount; i++)
        {
            _targetParent.GetChild(i).localPosition = Vector3.zero ;
           _targetParent.GetChild(i).gameObject.SetActive(false);
        }    

        _target.active = false;
        _target.isFixed = false;

        _target = null;
    }

    // 신호 소리 끄고 켜기
    public void SignSoundChange()
    {
        _isMute = !_isMute;

        if (!_isMute)
        {
            _soundText.text = "신호 소리 : <color=yellow>켜짐</color>";
            return;
        }
        _soundText.text = "신호 소리 : 꺼짐";
    }

    // 메인메뉴로 가기
    public void GoMainMenu() => SceneManager.LoadSceneAsync(GameMode.MainMenu.ToString());
}
