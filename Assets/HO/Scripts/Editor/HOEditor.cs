using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class HOEditor:EditorWindow
{
    #region Constants
    const float WINDOWSIZEWIDTH = 400f;
    const float WINDOWSIZEHEIGHT = 500f;
    const string PATHTOLEVELS = "Assets/_BundledAssets/Levels/";
    const string PATHEDITORSCENE = "Assets/Editor/HOEditor/HoEditorScene.unity";
    const string PATHHOSCENE = "Assets/HO/HOScene.unity";
    const string PATHMAPSCENE = "Assets/MapAsset/Scenes/MapScene.unity";
    #endregion

    public static HOEditor Instance;

    [MenuItem( "HO/Editor", priority = 1 )]
    public static void Open()
    {
        var _window = EditorWindow.GetWindow<HOEditor>( "HO Editor", true );

        _window.Show();

        // _window.maxSize = _window.minSize = new Vector2( WINDOWSIZEWIDTH, WINDOWSIZEHEIGHT );
        Instance = _window;
    }


    private void SceneLoadButton(string name, string path, float buttonWidth = -1)
    {
        if (GUILayout.Button( string.Format( "Go to {0} scene", name ), GUILayout.Width( ( ( buttonWidth > 0 ) ? buttonWidth : 250f ) ) ))
        {
            EditorSceneManager.OpenScene( path );
        }
    }
    private string GetDefaultScenePath
    {
        get
        {
            return EditorBuildSettings.scenes[ 0 ].path;
        }
    }


    private void OnDestroy()
    {

    }

    public void OnGUI()
    {
        SceneLoadButton( "Preloader", GetDefaultScenePath );
        SceneLoadButton( "Map", PATHMAPSCENE );
        //SceneLoadButton( "Editor", PATHEDITORSCENE );
        SceneLoadButton( "HO", PATHHOSCENE );
    }
}
