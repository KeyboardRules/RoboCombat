using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicKitController : MonoBehaviour
{
    [SerializeField] MusicKit[] musicKits;
    int currentMusicKitIndex=0;
    public static MusicKitController instance;
    public void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        foreach (MusicKit musicKit in musicKits)
        {
            musicKit.menu.source = gameObject.AddComponent<AudioSource>();
            musicKit.menu.source.clip = musicKit.menu.clip;
            musicKit.menu.source.volume = musicKit.menu.volumn;
            musicKit.menu.source.pitch = musicKit.menu.pitch;
            musicKit.menu.source.loop = musicKit.menu.loop;
            musicKit.startRound.source = gameObject.AddComponent<AudioSource>();
            musicKit.startRound.source.clip = musicKit.startRound.clip;
            musicKit.startRound.source.volume = musicKit.startRound.volumn;
            musicKit.startRound.source.pitch = musicKit.startRound.pitch;
            musicKit.startRound.source.loop = musicKit.startRound.loop;
            musicKit.mvpPlayer.source = gameObject.AddComponent<AudioSource>();
            musicKit.mvpPlayer.source.clip = musicKit.mvpPlayer.clip;
            musicKit.mvpPlayer.source.volume = musicKit.mvpPlayer.volumn;
            musicKit.mvpPlayer.source.pitch = musicKit.mvpPlayer.pitch;
            musicKit.mvpPlayer.source.loop = musicKit.mvpPlayer.loop;
        }
    }
    public void Start()
    {
        musicKits[currentMusicKitIndex].MenuPlay();
    }
    public void SelectKit(int index)
    {
        musicKits[currentMusicKitIndex].MenuStop();
        currentMusicKitIndex = index;
        musicKits[currentMusicKitIndex].MenuPlay();
    }
    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    public void StartRound()
    {
        musicKits[currentMusicKitIndex].StartRoundPlay();
    }
    public void MVPRound(int _index)
    {
        musicKits[_index].MVPPlay();
    }
    public int GetMusicKitIndex()
    {
        return currentMusicKitIndex;
    }
    public string GetMusicKitName()
    {
        return musicKits[currentMusicKitIndex].nameKit;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 0)
        {
            musicKits[currentMusicKitIndex].MenuPlay();
        }
        else
        {
            musicKits[currentMusicKitIndex].MenuStop();
        }
    }
}
