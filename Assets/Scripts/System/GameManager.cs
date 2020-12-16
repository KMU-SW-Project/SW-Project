using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public enum HandType
{
    left,
    right,
}

[Serializable]
public class UserData
{
    // 서버에 저장된 고유 테이블 번호
    public string dataIndate;
    public string rankingUuid;

    // 유저 데이터
    public string userNickname;
    public int userCharacter;
    public int userClearAI;
    public int userInfinityScore;
}

[Serializable]
public class ModeData
{
    public GameMode currentPlayMode;
    public int currentPlayAiIndex;
    public List<EnemyAI> enemyData;
    public int currentSelectedTarget;
}

public static class Sound
{
    private static int bGM = 8;
    private static int sFX = 8;

    public static int BGM
    {
        get => bGM;
        set
        {
            if (value > 8 || value < 0) return;

            bGM = value;
            TitleManager.instance.SetSoundValueText(BGM, SFX);
        }
    }
    public static int SFX
    {
        get => sFX;
        set
        {
            if (value > 8 || value < 0) return;

            sFX = value;
            TitleManager.instance.SetSoundValueText(BGM, SFX);
        }
    }
}


public class GameManager : MonoBehaviour
{
    #region 싱글톤
    private static GameManager instance;

    public GameObject player;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);

            var obj = GameObject.Find("Player");
            if (obj) Destroy(obj.gameObject);
            // Destroy(player);
        }

      //  player = GameObject.Find("Player");
        instance = this;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(player);

        if(userData.dataIndate != string.Empty)
        {
            Sound.BGM = Sound.SFX = 8;
        }

        CreatePool();
        modeData.enemyData = new List<EnemyAI>();
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            print("GameManager 인스턴스 없음");
            return null;
        }

        return instance;
    }
    #endregion

    public UserData userData = new UserData();
    public ModeData modeData = new ModeData();

    // 0,1 = left gun-hand , 2,3 = right gun-hand
    public GameObject[] handModel;
    public GameMode playMode;
    public bool gunGuide, hitEnemy, isShot;
    public AudioMixer audioMixer;
    public AudioClip[] audioArray;

    #region 이펙트 풀링
    private Queue<ParticleSystem> _shotEffectPool;
    private Queue<ParticleSystem> _markEffectPool;
    [SerializeField] private ParticleSystem _markEffectPrefab;
    [SerializeField] private Transform _weaponParent;
    [SerializeField] private ParticleSystem _effectPrefab;

    private void CreatePool()
    {
        _markEffectPool = new Queue<ParticleSystem>();
        _shotEffectPool = new Queue<ParticleSystem>();

        for (int i = 0; i < 20; i++)
        {
            var effect = Instantiate(_effectPrefab, _weaponParent.GetChild(0));
            var mark = Instantiate(_markEffectPrefab, _weaponParent.GetChild(1));

            effect.gameObject.SetActive(false);
            _shotEffectPool.Enqueue(effect);

            mark.gameObject.SetActive(false);
            _markEffectPool.Enqueue(mark);
        }
    }

    public ParticleSystem GetEffect()
    {
        var effect = _shotEffectPool.Dequeue();
        effect.gameObject.SetActive(true);
        return effect;
    }

    public ParticleSystem GetMark()
    {
        var mark = _markEffectPool.Dequeue();
        mark.gameObject.SetActive(true);

        return mark;
    }

    public void ReturnEffect(ParticleSystem obj)
    {
        _shotEffectPool.Enqueue(obj);
    }

    public void ReturnMark(ParticleSystem obj)
    {
        _markEffectPool.Enqueue(obj);
    }

    #endregion
    public void SetUserControllerModel(HandType handtype)
    {
        for (int i = 0; i < handModel.Length; i++)
            handModel[i].SetActive(false);

        switch (handtype)
        {
            case HandType.left:
                handModel[0].SetActive(true);
                handModel[3].SetActive(true);
                break;
            case HandType.right:
                handModel[1].SetActive(true);
                handModel[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void SetPlayerCameraPosition(Transform originPlayer)
    {
        player.transform.position = originPlayer.transform.position;
        player.transform.rotation = originPlayer.transform.rotation;
        player.transform.GetComponentInChildren<Camera>().transform.rotation = originPlayer.transform.rotation;
    }

    public void SetSoundValue(string typeValue)
    {
        int index = typeValue.IndexOf('|');
        var type = typeValue.Substring(0, index);
        var flag = typeValue.Substring(index + 1);

        if (type == "BGM")
        {
            if (flag == "Up") Sound.BGM++;
            else Sound.BGM--;

          audioMixer.SetFloat("BGM", (Sound.BGM*10) - 80);
            return;
        }

        if (flag == "Up") Sound.SFX++;
        else Sound.SFX--;

        audioMixer.SetFloat("SFX", (Sound.SFX*10) - 80);
    }
    
    public void SetBGM(GameMode mode)
    {
        var audioSource = player.GetComponent<AudioSource>();

        switch (mode)
        {
            case GameMode.Infinity:
                audioSource.clip = audioArray[0];                
                break;
            case GameMode.Bounty:
                audioSource.clip = audioArray[1];
                break;
            case GameMode.Training:
                audioSource.clip = audioArray[2];
                break;
            case GameMode.Title:
            case GameMode.MainMenu:
                audioSource.clip = audioArray[3];
                break;
            default:
                break;
        }

        audioSource.Play();
    }

    public void DeleteObject()
    {
        Destroy(player);        
    }
}
