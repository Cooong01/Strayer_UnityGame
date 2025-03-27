using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通过AB包加载shader
/// </summary>
public class ShaderABLoad : SingletonAutoMono<ShaderABLoad>
{
    private string path = "shaders/"; //基于Resources

    public void Test(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                // 这里可以对材质进行处理，例如打印名称
                if (material!=null)
                {
                    if (material.name!= "Sprite-Lit-Default")
                    {
                        string name = material.name.Substring(material.name.LastIndexOf('/') + 1);
                        print(name);
                        if (name == "white")
                        {
                            material.shader = Resources.Load<Shader>(path + "White(Shine)");
                        }
                        else if (name == "RainBow")
                        {
                            material.shader = Resources.Load<Shader>(path + "RainBow(Shine)");
                        } else if (name == "NPC1") {

                            material.shader = Resources.Load<Shader>(path + "NPC1");
                        } else if (name == "NPC2") {

                            material.shader = Resources.Load<Shader>(path + "NPC2");
                        } else if (name == "NPC3") {

                            material.shader = Resources.Load<Shader>(path + "NPC3");
                        }
                        else
                        {
                            material.shader = Resources.Load<Shader>(path + name);
                            print(material.shader.name);
                        }

                    }

                }
            }
        }
    }
}
