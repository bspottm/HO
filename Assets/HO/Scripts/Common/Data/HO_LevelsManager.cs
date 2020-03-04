using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using LitJson;

namespace HOSystem
{
    public class HO_LevelsManager:MonoBehaviour
    {
        private const string PATHTODATA = "Data/LevelInfo";

        private static HO_LevelsManager _instance;
        public static HO_LevelsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<HO_LevelsManager>();

                if (_instance == null)
                {
                    _instance = new GameObject( "Data" ).AddComponent<HO_LevelsManager>();
                }
                return _instance;
            }
        }

        public HO_Levels LevelsData;

        private void Awake()
        {
            _instance = this;
        }

        public HO_LocationInfo GetLevel(string location)
        {
            Load();
            for(int i = 0; i<LevelsData.levels.Count;i++)
            {
                if (LevelsData.levels[ i ].Location == location)
                    return LevelsData.levels[ i ];
            }

            return null;
        }

#if UNITY_EDITOR
        private static void Save()
        {
            try
            {
                CheckDataFile();



                var _data = JsonMapper.ToJson( Instance.LevelsData );
                StreamWriter streamWriter = new StreamWriter( Application.dataPath + PATHTODATA );
                streamWriter.WriteLine( _data );
                streamWriter.Close();
            }
            catch (Exception e)
            {
                Debug.LogError( e.ToString() );
            }

        }

        [ContextMenu("Save")]
        private void SaveData()
        {
            Save();
        }

        [ContextMenu("Load")]
        private void LoadData()
        {
            Load();
        }
#endif

        public static void Load()
        {
            try
            {
                CheckDataFile();
                TextAsset _d = Resources.Load<TextAsset>( PATHTODATA );
                

                string _data = _d.text;
               // File.ReadAllText( Application.dataPath + PATHTODATA );
                var reader = new JsonReader( _data );

                if (string.IsNullOrEmpty( _data ))
                {
                    Debug.LogError( "Empty levels data!" );
                    return;
                }

                var levels = ( JsonMapper.ToObject<HO_Levels>(reader) );
                Instance.LevelsData = levels;

            }
            catch (Exception e)
            {
                Debug.LogError( e.ToString() );
            }
        }

        private static void CheckDataFile()
        {
            if (!Directory.Exists( "Assets/HO" ))
                Directory.CreateDirectory( "Assets/HOData" );

            if (!Directory.Exists( "Assets/HO/Data" ))
                Directory.CreateDirectory( "Assets/HO/Data" );

           // if (!File.Exists( PATHTODATA ))
           //     File.Create( PATHTODATA );
        }
    }

    [Serializable]
    public class HO_Levels
    {
        public List<HO_LocationInfo> levels;
    }
}
