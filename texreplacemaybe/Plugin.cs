using System;
using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TextureReplacer
{
    [BepInPlugin("com.doug.texturereplacer", "Texture Replacer", "1.0.0")]
    public class TextureReplacer : BaseUnityPlugin
    {
        private ConfigEntry<string> texturePath;
        
        private void Awake()
        {
            texturePath = Config.Bind("General", "Texture path", "put your path here", "put your file path there");
            Loadtexture(texturePath.Value);
            Logger.LogInfo("TextureReplacer loaded");
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Applytexture(Loadtexture(texturePath.Value));
        }

        private Texture2D Loadtexture(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            
            byte[] fileData = System.IO.File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);

            if (tex.LoadImage(fileData))
            {
                return tex;
            }

            return null;
        }

        private void Applytexture(Texture2D tex)
        {
            Renderer[] renderers = FindObjectsOfType<Renderer>();

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (!mat) continue;
                    
                    if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex);
                }
            }
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            Applytexture(Loadtexture(texturePath.Value));
        }
    }
}