using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 资源管理器-编辑器加载版
/// 仅用于编辑器模式加载资源
/// </summary>
public class EditorResMgr : BaseManager<EditorResMgr>
{
    //需要打包进AB包中的资源的路径 
    private string rootPath = "Assets/Editor/ArtRes/";
    private EditorResMgr() { }

    public T LoadEditorRes<T>(string path) where T:Object
    {
#if UNITY_EDITOR
        string suffixName = "";
        //预设体、材质球、纹理（图片）、音效等等
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

    //图集内单个资源加载
    public Sprite LoadSprite(string path, string spriteName)
    {
#if UNITY_EDITOR
        Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(rootPath + path);
        //遍历图集里所有资源 得到同名图片返回
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

    //图集资源加载
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

