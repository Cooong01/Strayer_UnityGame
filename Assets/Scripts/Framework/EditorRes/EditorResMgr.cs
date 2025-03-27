using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// ��Դ������-�༭�����ذ�
/// �����ڱ༭��ģʽ������Դ
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    //��Ҫ�����AB���е���Դ��·�� 
    private string rootPath = "Assets/Editor/ArtRes/";
    private EditorResMgr() { }

    public T LoadEditorRes<T>(string path) where T:Object
    {
#if UNITY_EDITOR
        string suffixName = "";
        //Ԥ���塢����������ͼƬ������Ч�ȵ�
        if (typeof(T) == typeof(GameObject))
            suffixName = ".prefab";
        else if (typeof(T) == typeof(Material))
            suffixName = ".mat";
        else if (typeof(T) == typeof(Texture))
            suffixName = ".png";
        else if (typeof(T) == typeof(AudioClip))
            suffixName = ".mp3";
        else if (typeof(T) == typeof(Sprite))
            suffixName = ".png";
        T res = AssetDatabase.LoadAssetAtPath<T>(rootPath + path + suffixName);
        return res;
#else
        return null;
#endif
    }

    //ͼ���ڵ�����Դ����
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //����ͼ����������Դ �õ�ͬ��ͼƬ����
        foreach (var item in sprites)
        {
            if (spriteName == item.name)
                return item as Sprite;
        }
        return null;
#else
        return null;
#endif
    }

    //ͼ����Դ����
    public Dictionary<string, Sprite> LoadSprites(string path)
    {
#if UNITY_EDITOR
        Dictionary<string, Sprite> spriteDic = new Dictionary<string, Sprite>();
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        foreach (var item in sprites)
        {
            spriteDic.Add(item.name, item as Sprite);
        }
        return spriteDic;
#else
        return null;
#endif
    }

}

