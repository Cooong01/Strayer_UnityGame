using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderABLoad : SingletonAutoMono<ShaderABLoad>
{
    private string path = "shaders/"; //�̶�Ϊ��Resources/shaders
    //public void load(GameObject obj)
    //{
    //    TextMeshProUGUI[] TMP = obj.transform.GetComponentsInChildren<TextMeshProUGUI>();

    //    foreach (var i in TMP)
    //    {

    //        i.font = Resources.Load<TMP_FontAsset>("Fonts/" + i.name);
    //    }
    //}

    public void Test(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                // ������ԶԲ��ʽ��д��������ӡ����
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
