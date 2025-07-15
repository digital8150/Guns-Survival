//=========== Copyright (c) GameBuilders, All rights reserved. ================//

using GameBuilders.FPSBuilder.Editor.Utility;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace GameBuilders.FPSBuilder.Editor.Windows
{
    [InitializeOnLoad]
    public class HelpWindow : EditorWindow
    {
        private const int k_MaxTags = 10000;
        private const int k_MaxLayers = 31;

        private bool m_ShowAtStartup = true;
        private static Texture2D s_CoverImage;
        
        private static ListRequest s_ListRequest;
        private static AddRequest s_AddRequest;

        static HelpWindow()
        {
            EditorApplication.update += ShowAtStartup;
            EditorApplication.quitting += BeforeClose;
        }

        private static void ShowAtStartup()
        {
            if (!Application.isPlaying)
            {
                bool showAtStartup = EditorPrefs.GetInt("showDialogAtStartup", 1) > 0;
                bool alreadyShowed = EditorPrefs.GetInt("showedDialogAtStartup", 0) > 0;

                if (!showAtStartup || alreadyShowed) 
                    return;
                
                EditorPrefs.SetInt("showedDialogAtStartup", 1);
                Init();
            }
            
            // ReSharper disable once DelegateSubtraction
            EditorApplication.update -= ShowAtStartup;
        }

        private static void BeforeClose()
        {
            EditorPrefs.SetInt("showedDialogAtStartup", 0);
            EditorApplication.quitting -= BeforeClose;
        }
        
        [MenuItem("Window/GameBuilders/FPS Builder/Help %#h", false, 51)]
        private static void Init ()
        {
            HelpWindow window = (HelpWindow)GetWindow(typeof(HelpWindow), true, "FPS Builder");
            window.minSize = new Vector2(580, 420);
            window.maxSize = new Vector2(580, 420);
            window.Show();

            s_CoverImage = EditorUtilities.Load<Texture2D>("/FPSBuilder/Editor Resources/", "fps-builder_cover.png");
            s_ListRequest = Client.List();
        }

        private void OnGUI ()
        {
            using (new GUILayout.VerticalScope())
            {
                if (s_CoverImage)
                {
                    GUI.DrawTexture(new Rect(0, 0, 580, 128), s_CoverImage, ScaleMode.StretchToFill, true, 10.0f);
                    GUILayout.Space(138);
                }
                else
                {
                    GUILayout.Space(32);
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("FPS Builder", Styling.headerLabel);
                    GUILayout.FlexibleSpace();
                }
                
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("The ultimate solution for modern first-person shooter games.");
                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(32);
                
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Discord Community", GUILayout.Width(160)))
                    {
                        Application.OpenURL("https://discord.gg/MXV4Vjk");
                    }

                    GUILayout.FlexibleSpace();
                }
                
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Online Documentation", GUILayout.Width(160)))
                    {
                        Application.OpenURL("https://docs.gamebuilders.com.br/");
                    }

                    GUILayout.FlexibleSpace();
                }

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Support E-mail", GUILayout.Width(160)))
                    {
                        Application.OpenURL("mailto:support@gamebuilders.com.br");

                        TextEditor textEditor = new TextEditor
                        {
                            text = "support@gamebuilders.com.br"
                        };
                        textEditor.SelectAll();
                        textEditor.Copy();

                        Debug.Log("Support E-mail copied to clipboard.");
                    }

                    GUILayout.FlexibleSpace();
                }

                GUILayout.Space(32);
                using (new GUILayout.VerticalScope())
                {
                    bool fixTagsAndLayers = !TagExists("Ammo") || !TagExists("Adrenaline Pack") || !LayerExists("Viewmodel") || !LayerExists("Post-Processing");
                    bool fixColorSpace = PlayerSettings.colorSpace != ColorSpace.Linear;
                    bool fixRenderingPath = EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier1).renderingPath != RenderingPath.DeferredShading
                        || EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier2).renderingPath != RenderingPath.DeferredShading
                        || EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone, GraphicsTier.Tier3).renderingPath != RenderingPath.DeferredShading;

                    bool fixPackages = CheckForPackagesMissing();
                    
                    if (s_ListRequest != null && s_ListRequest.Status == StatusCode.InProgress)
                    {
                        EditorGUILayout.HelpBox("Checking project status...", MessageType.Info);
                    }
                    else
                    {
                        if (fixTagsAndLayers || fixColorSpace || fixRenderingPath || fixPackages)
                        {
                            using (new GUILayout.VerticalScope())
                            {
                                EditorGUILayout.HelpBox(
                                    (fixTagsAndLayers
                                        ? "There are Tags/Layers missing in the project \n"
                                        : string.Empty)
                                    + (fixColorSpace ? "Use Linear Color Space \n" : string.Empty)
                                    + (fixRenderingPath ? "Use Deferred Rendering Path \n" : string.Empty)
                                    + (fixPackages ? "There are Packages Missing in your project" : string.Empty),
                                    MessageType.Warning);

                                using (new GUILayout.HorizontalScope())
                                {
                                    GUILayout.FlexibleSpace();
                                    if (GUILayout.Button("Fix now", GUILayout.Width(160)))
                                    {
                                        if (!TagExists("Ammo"))
                                            AddTag("Ammo");

                                        if (!TagExists("Adrenaline Pack"))
                                            AddTag("Adrenaline Pack");

                                        if (!LayerExists("Post-Processing"))
                                            AddLayer("Post-Processing");

                                        if (!LayerExists("Viewmodel"))
                                            AddLayer("Viewmodel");
                                        
                                        if (PlayerSettings.colorSpace != ColorSpace.Linear)
                                        {
                                            PlayerSettings.colorSpace = ColorSpace.Linear;
                                            Debug.Log("Changed color space to Linear");
                                        }

                                        if (fixPackages)
                                        {
                                            s_AddRequest = Client.Add("com.unity.postprocessing");
                                            Debug.Log("Adding 'com.unity.postprocessing' package.");
                                        }

                                        TierSettings tier1 =
                                            EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier1);
                                        if (tier1.renderingPath != RenderingPath.DeferredShading)
                                        {
                                            tier1.renderingPath = RenderingPath.DeferredShading;
                                            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier1, tier1);
                                            Debug.Log("Changed Graphics Tier 1 Rendering Path to Deferred");
                                        }

                                        TierSettings tier2 =
                                            EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier2);
                                        if (tier2.renderingPath != RenderingPath.DeferredShading)
                                        {
                                            tier2.renderingPath = RenderingPath.DeferredShading;
                                            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier2, tier2);
                                            Debug.Log("Changed Graphics Tier 2 Rendering Path to Deferred");
                                        }

                                        TierSettings tier3 =
                                            EditorGraphicsSettings.GetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier3);
                                        if (tier3.renderingPath != RenderingPath.DeferredShading)
                                        {
                                            tier3.renderingPath = RenderingPath.DeferredShading;
                                            EditorGraphicsSettings.SetTierSettings(BuildTargetGroup.Standalone,
                                                GraphicsTier.Tier3, tier3);
                                            Debug.Log("Changed Graphics Tier 3 Rendering Path to Deferred");
                                        }
                                    }
                                    GUILayout.FlexibleSpace();
                                }
                            }
                        }
                    }
                }
                GUILayout.FlexibleSpace();

                m_ShowAtStartup = EditorPrefs.GetInt("showDialogAtStartup", 1) > 0;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Version 1.1.0", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    m_ShowAtStartup = EditorGUILayout.ToggleLeft("Show At Startup", m_ShowAtStartup);
                }
                
                EditorPrefs.SetInt("showDialogAtStartup", m_ShowAtStartup ? 1 : 0);

                GUILayout.Space(5);
            }
        }

        private static bool CheckForPackagesMissing()
        {
            if (s_ListRequest != null)
            {
                if (s_ListRequest.IsCompleted && s_ListRequest.Status == StatusCode.Success)
                {
                    if (!CheckForCustomRenderingPipeline())
                    {
                        foreach (var package in s_ListRequest.Result)
                        {
                            if (package.name == "com.unity.postprocessing")
                                return false;
                        }
                    }

                    return true;
                }
            }
            else
            {
                s_ListRequest = Client.List();
            }

            if (s_AddRequest != null)
            {
                if (s_AddRequest.Status == StatusCode.Success)
                    Debug.Log("Installed: " + s_AddRequest.Result.packageId);

                s_AddRequest = null;
            }

            return true;
        }

        private static bool CheckForCustomRenderingPipeline()
        {
            foreach (PackageInfo package in s_ListRequest.Result)
            {
                switch (package.name)
                {
                    case "com.unity.render-pipelines.high-definition":
                    case "com.unity.render-pipelines.universal":
                        return true;
                }
            }
            
            return false;
        }

        //Original code written by Leslie Young and ctwheels
        //https://answers.unity.com/questions/33597/is-it-possible-to-create-a-tag-programmatically.html

        private static bool AddTag (string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            if (tagsProp.arraySize >= k_MaxTags)
            {
                Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
                return false;
            }

            // if not found, add it
            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;

                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(index);

                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                sp.stringValue = tagName;

                Debug.Log("Tag: " + tagName + " has been added");

                // Save settings
                tagManager.ApplyModifiedProperties();
                return true;
            }
            return false;
        }

        // Checks to see if tag exists
        private static bool TagExists (string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");
            return PropertyExists(tagsProp, 0, k_MaxTags, tagName);
        }

        private static bool AddLayer (string layerName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (!PropertyExists(layersProp, 0, k_MaxLayers, layerName))
            {
                SerializedProperty sp;

                // Start at layer 9th index -> 8 (zero based) => first 8 reserved for unity / greyed out
                for (int i = 8, j = k_MaxLayers; i < j; i++)
                {
                    sp = layersProp.GetArrayElementAtIndex(i);
                    if (sp.stringValue == "")
                    {
                        // Assign string value to layer
                        sp.stringValue = layerName;

                        Debug.Log("Layer: " + layerName + " has been added");

                        // Save settings
                        tagManager.ApplyModifiedProperties();
                        return true;
                    }
                }
            }
            return false;
        }

        // Checks to see if layer exists
        private static bool LayerExists (string layerName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Layers Property
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            return PropertyExists(layersProp, 0, k_MaxLayers, layerName);
        }

        // Checks if the value exists in the property
        private static bool PropertyExists (SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
