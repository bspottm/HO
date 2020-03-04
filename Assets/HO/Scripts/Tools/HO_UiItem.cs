using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HOSystem
{
    public class HO_UiItem:MonoBehaviour
    {
        private static Dictionary<string, GameObject> elements = new Dictionary<string, GameObject>();
        private static bool isClearActionExist = false;

        [SerializeField]
        private string ElementName;
        private bool isInit = false;

        private void Awake()
        {
            SetName( ElementName );
        }

        public void SetName(string name)
        {
            if (isInit)
                return;

            if (string.IsNullOrEmpty( name ))
                return;

            ElementName = name;

            if (elements.ContainsKey( ElementName ))
            {
                if (elements[ ElementName ] == null)
                {
                    elements[ ElementName ] = gameObject;
                }
                return;
            }

            AddElement( this );

            isInit = true;

            if (!isClearActionExist)
            {
                SceneManager.sceneUnloaded += Clear;
                isClearActionExist = true;
            }
        }

        private static void AddElement(HO_UiItem element)
        {
            if (elements.ContainsKey( element.ElementName ))
            {
                elements[ element.ElementName ] = element.gameObject;
            }
            else
            {
                elements.Add( element.ElementName, element.gameObject );
            }
        }

        public static GameObject GetElement(string elementName)
        {
            return ( elements.ContainsKey( elementName ) ) ? elements[ elementName ] : null;
        }

        public static T GetElement<T>(string elementName) where T : MonoBehaviour
        {
            var obj = GetElement( elementName );
            return ( obj == null ) ? null : obj.GetComponent<T>();
        }

        private void Clear(Scene s)
        {
            elements.Clear();
            SceneManager.sceneUnloaded -= Clear;
            isClearActionExist = false;
        }
    }
}
