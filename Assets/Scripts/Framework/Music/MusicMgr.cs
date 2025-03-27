using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 音乐音效管理器
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource bkMusic = null;
    //背景音乐音量大小
    private float bkMusicValue = 0.4f;
    //音效音量大小
    private float soundValue = 0.4f;
    //音效是否在播放
    private bool soundIsPlay = true;
    //存储正在播放的音效
    private List<AudioSource> soundList = new List<AudioSource>();


    private MusicMgr() 
    {
        MonoMgr.Instance.AddFixedUpdateListener(Update);
    }


    private void Update()
    {
        if (!soundIsPlay)
            return;
        //逐帧检测音效是否播完，播完就移出音效池，clip置空放入缓存池
        for (int i = soundList.Count - 1; i >= 0; --i)
        {
            if(!soundList[i].isPlaying)
            {
                soundList[i].clip = null;
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }


    //播放背景音乐
    public void PlayBKMusic(string name)
    {
        if(bkMusic == null)
        {
            GameObject obj = new GameObject();
            obj.name = "BKMusic";
            GameObject.DontDestroyOnLoad(obj);
            bkMusic = obj.AddComponent<AudioSource>();
        }

        //根据传入的背景音乐名字 来播放背景音乐
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            bkMusic.clip = clip;
            bkMusic.loop = true;
            bkMusic.volume = bkMusicValue;
            bkMusic.Play();
        });
    }

    //停止背景音乐
    public void StopBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Stop();
    }

    //暂停背景音乐
    public void PauseBKMusic()
    {
        if (bkMusic == null)
            return;
        bkMusic.Pause();
    }

    //调整背景音乐大小
    public void ChangeBKMusicValue(float v)
    {
        bkMusicValue = v;
        if (bkMusic == null)
            return;
        bkMusic.volume = bkMusicValue;
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名字</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callBack">加载结束后的回调</param>
    public void PlaySound(string name, bool isLoop = false, bool isSync = false, UnityAction<AudioSource> callBack = null)
    {
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            AudioSource source;
            PoolMgr.Instance.GetObj("music", "soundObj", (t) =>
            {
                source = t.GetComponent<AudioSource>();
                source.Stop();
                source.clip = clip;
                source.loop = isLoop;
                source.volume = soundValue;
                source.Play();
                if(!soundList.Contains(source))
                    soundList.Add(source);
                callBack?.Invoke(source);
            });

        }, isSync);
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    /// <param name="source">音效组件对象</param>
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
            source.clip = null;
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }

    /// <summary>
    /// 调整音效大小
    /// </summary>
    /// <param name="v"></param>
    public void ChangeSoundValue(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = v;
        }
    }

    /// <summary>
    /// 继续播放或者暂停所有音效
    /// </summary>
    /// <param name="isPlay">true为播放 false为暂停</param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if(isPlay)
        {
            soundIsPlay = true;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].Play();
        }
        else
        {
            soundIsPlay = false;
            for (int i = 0; i < soundList.Count; i++)
                soundList[i].Pause();
        }
    }

    /// <summary>
    /// 清空音效相关记录。注意，缓存池清除记录之前必须调用此方法，否则无法通过音乐管理器找到正在播放的音乐。
    /// </summary>
    public void ClearSound()
    {
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        soundList.Clear();
    }
}
