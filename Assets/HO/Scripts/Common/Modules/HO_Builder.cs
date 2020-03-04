using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HOSystem
{

    public class HO_Builder:HO_Module, IHOBuilder
    {
        private Transform LocationGO;
        private GameObject SettingGO;
        private HO_LocationSetting Setting = null;
        private Dictionary<string, HOBuildItem> BuildItems;
        private GameObject AnimationGO;

        public void Build()
        {
            LocationGO = Object.Instantiate( Manager.LoadedItems.GetItem( "Location" ) ).transform;
            InitSettings();
            BuildItems = new Dictionary<string, HOBuildItem>();
            CheckContent();//STEP_1
            Shuffle();//STEP_2
            FinishBuildContentObject();//STEP_3
            Remove();
        }

        private void InitSettings()
        {
            if (!Manager.LoadedItems.HasItem( "Setting" ))
                return;

            SettingGO = Manager.LoadedItems.GetItem( "Setting" );
            if (SettingGO != null)
                Setting = SettingGO.GetComponent<HO_LocationSetting>();

        }

        private int CountItems = 0;
        private List<string> ItemsList = new List<string>();
        private Dictionary<string, List<GameObject>> GroupsObject = new Dictionary<string, List<GameObject>>();
        private List<string> UseGroup = new List<string>();
        private Dictionary<string, Dictionary<string, GameObject>> _allHiddenObjects = new Dictionary<string, Dictionary<string, GameObject>>();
        private Dictionary<string, GameObject> allObjects = new Dictionary<string, GameObject>();
        private string[] HiddenObjects;

        #region STEP_1
        private void CheckContent()
        {
            for (int i = 0; i < LocationGO.childCount; i++)
            {
                GameObject item = LocationGO.GetChild( i ).gameObject;
                RepairObject( item );
                if (item.name.IndexOf( '[' ) != -1)
                {
                    CheckItem( item );
                }
            }
        }



        private void CheckItem(GameObject item)
        {
            string _name = item.name.Substring( 1, item.name.IndexOf( ']' ) - 1 ).ToLower();
            //item.name = _name;
            string[] Args = _name.Split( ',' );

            string interactiveName = Args[ 0 ];

            switch (interactiveName)
            {
                case "animation":
                Object.Destroy( item );
                break;
                case "bg":
                Object.Destroy( item );
                break;
                default:
                AddHiddenObjects( item, Args[ 0 ] );
                break;
            }
        }

        private void AddHiddenObjects(GameObject item, string name)
        {
            string _name = GetItemName( name );

            if (!ItemsList.Contains( _name ))
                ItemsList.Add( _name );

            CheckGroup( item );
            string _difficulty = GetItemDifficulty( name );
            var _itemList = GetItemsList( _difficulty );

            if (!_itemList.ContainsKey( _name ))
            {
                _itemList.Add( _name, item );
                ItemConstruct( item, _name );
            }
        }

        private void CheckGroup(GameObject item)
        {
            if (item.name.IndexOf( '@' ) == -1)
                return;

            //var nameParts = item.name.Split( '@' );
            string group = GetNamePart( item.name, 1, '@' );// nameParts[ 1 ].ToLower();
            item.name = GetNamePart( item.name, 0, '@' );//nameParts[ 0 ];
            if (!GroupsObject.ContainsKey( group ))
                GroupsObject.Add( group, new List<GameObject>() );

            GroupsObject[ group ].Add( item );
        }

        private string GetItemGroup(GameObject item)
        {
            foreach (var name in GroupsObject.Keys)
            {
                var group = GroupsObject[ name ];
                for (int i = 0; i < group.Count; i++)
                {
                    if (group[ i ] == item)
                    {
                        return "(" + name + ")";
                    }
                }
            }
            return "";
        }

        private string GetItemDifficulty(GameObject _item)
        {
            string _difficulty = "";
            _difficulty = _item.name.Split( '_' )[ 0 ].ToLower();
            return _difficulty;
        }

        private GameObject ItemConstruct(GameObject _item, string _name)
        {
            HOBuildItem _buildItem = new HOBuildItem();

            var _image = _item.transform.Find( "image" );
            _buildItem.Image = _image.gameObject;

            var _object = _image.gameObject;

            if (Setting)
            {
                var alt = Setting.GetItem( _item.name );
                if (alt != null)
                {
                    if (alt.collider != null)
                    {
                        alt.collider.transform.parent = _image;
                        _buildItem.Collider = alt.collider;
                    }

                    if (alt.silhuette != null)
                    {
                        var sil = new GameObject( "silhouette" );
                        sil.transform.parent = _image;
                        var sr = sil.AddComponent<SpriteRenderer>();
                        sr.sprite = alt.silhuette;
                        sr.enabled = false;
                    }
                }
            }

            Transform _glow = _item.transform.Find( "glow" );
            if (_glow != null)
            {
                SpriteRenderer image = _glow.GetComponent<SpriteRenderer>();
                image.color = new Color( 1, 1, 1, 0 );
                _glow.transform.parent = _object.transform;

                _buildItem.Glow = _glow.gameObject;
            }

            Transform _silhouette = _item.transform.Find( "silhouette" );
            if (_silhouette != null)
            {
                _silhouette.transform.parent = _object.transform;
                _silhouette.gameObject.SetActive( false );
                _buildItem.Silhouette = _silhouette.gameObject;
            }

            Transform _shadow = _item.transform.Find( "shadow" );
            if (_shadow != null)
            {
                _buildItem.Shadow = _shadow.gameObject;
            }

            Transform _over = _item.transform.Find( "over" );
            if (_over != null)
            {
                _buildItem.Over = _over.gameObject;
            }

            BuildItems.Add( _name, _buildItem );
            return _object;
        }
        #endregion

        #region STEP_2
        public virtual void Shuffle()
        {
            string[] searchItems = Manager.LevelData.ActiveItemFormula;
            string[] backItems = Manager.LevelData.InactiveItemFormula;

            CountItems = 0;

            List<string> keys = new List<string>();

            keys = AddItemsByList( searchItems );

            CountItems = keys.Count;
            HiddenObjects = keys.ToArray();
            SendHiddenObjects();

            if (backItems != null && backItems.Length > 0)
                AddItemsByList( backItems );
        }

        private void SendHiddenObjects()
        {
            HOMessage _mess = CreateMessage( HOMessageType.SetHiddenObjectList );
            for (int i = 0; i < HiddenObjects.Length; i++)
            {
                _mess.hash.Add( i, HiddenObjects[ i ] );
            }
            Manager.Send( _mess );
        }

        private List<string> AddItemsByList(string[] data)
        {
            List<string> formulas = new List<string>();
            List<string> itemKeys = new List<string>();

            List<string> keys = new List<string>();
            foreach (var item in data)
            {
                if (IsFormula( item ))
                    formulas.Add( item );
                else
                    itemKeys.Add( item );

            }

            foreach (var item in itemKeys)
            {
                keys.Add( AddItemByKey( item ) );
            }

            if (formulas.Count > 0)
            {
                string formula = formulas[ Random.Range( 0, formulas.Count ) ];
                keys.AddRange( AddItemsByFormula( formula ) );
            }

            return keys;
        }

        private string AddItemByKey(string item)
        {
            GameObject go = GameObject.Find( item );
            var ho = allObjects;
            if (go == null)
            {
                Debug.LogError( "HO is not, name:" + item );
                Debug.Break();
            }

            CheckItemInFreeGroup( go );

            string key = go.transform.parent.name.Replace( "[", "" ).Replace( "]", "" ).ToLower();

            if (!ho.ContainsKey( key ))
            {
                ho.Add( key, go );
                // ItemActivate( go, key);
                ItemsList.Remove( key );
            }
            return key;
        }

        private List<string> AddItemsByFormula(string formula)
        {
            HOFormula _hoFormula = new HOFormula( formula );
            List<string> _keys = new List<string>();

            if (_hoFormula.A > 0)
                _keys.AddRange( FindHidenObject( "a", _hoFormula.A ) );
            if (_hoFormula.B > 0)
                _keys.AddRange( FindHidenObject( "b", _hoFormula.B ) );
            if (_hoFormula.C > 0)
                _keys.AddRange( FindHidenObject( "c", _hoFormula.C ) );

            return _keys;
        }

        protected virtual List<string> FindHidenObject(string _difficulty, int _count, bool isVisible = true)
        {
            List<string> _keys = new List<string>();

            Dictionary<string, GameObject> _itemList = GetItemsList( _difficulty ).ToDictionary();

            if (_itemList.Count <= 0)
            {
                Log( "NOT free items to (" + _difficulty + ") difficulty!" );
                return _keys;
            }

            var dic = allObjects;

            for (int i = 0; i < _count; i++)
            {
                GameObject _itemObj = null;
                string _item = "";

                if (_itemList.Count > 0)
                {
                    _item = _itemList.Keys.ElementAt( Random.Range( 0, _itemList.Count ) );
                    _itemObj = _itemList[ _item ];
                }

                if (_itemObj != null)
                {
                    CheckItemInFreeGroup( _itemObj );
                    _itemList.Remove( _item );
                    ItemsList.Remove( _item );
                    _keys.Add( _item );

                    if (!dic.ContainsKey( _item ))
                    {
                        dic.Add( _item, _itemObj );
                    }
                }
            }

            return _keys;
        }

        private bool CheckItemInFreeGroup(GameObject item, bool isAdded = true)
        {
            foreach (var name in GroupsObject.Keys)
            {
                var group = GroupsObject[ name ];
                for (int i = 0; i < group.Count; i++)
                {
                    if (group[ i ] == item)
                    {
                        if (UseGroup.Contains( name ))
                        {
                            return false;
                        }
                        if (isAdded)
                            UseGroup.Add( name );
                        return true;
                    }
                }
            }
            return true;
        }

        public bool IsVisibleSprite(Transform _transform)
        {
            return true;
        }

        private bool IsFormula(string key)
        {
            return char.IsNumber( key[ 0 ] );
        }
        #endregion

        #region STEP_3
        protected Transform FinishBuildContentObject()
        {
            Transform _content = new GameObject( "CONTENT" ).transform;
            Transform _contentItems = new GameObject( "ITEMS" ).transform;
            Transform _contentBacks = new GameObject( "BACKS" ).transform;
            Transform _backgrounds = new GameObject( "BACKGROUNDS" ).transform;
            _contentItems.parent = _content;
            _contentBacks.parent = _content;
            _backgrounds.parent = _content;

            foreach (var item in allObjects)
            {
                if (HiddenObjects.Contains( item.Key ))
                {
                    ItemActivate( item.Value, item.Key );
                    item.Value.transform.parent = _contentItems;
                }
                else
                {
                    item.Value.transform.parent = _contentBacks;
                    Transform _image = item.Value.transform.Find( "image" );
                    if (_image != null)
                    {
                        for (int i = 0; i < _image.childCount; i++)
                        {
                            Object.Destroy( _image.GetChild( i ).gameObject );
                        }
                    }
                    Collider2D collider = item.Value.transform.GetComponentInChildren<Collider2D>();
                    if (collider != null)
                    {
                        Debug.Log( "Remove Collider:" + item.Value.name );
                        Object.Destroy( collider );
                    }
                }

#if UNITY_EDITOR
                item.Value.name += GetItemGroup( item.Value );
#endif
            }

            /*TODO
            foreach (var item in FinalyBackroundItemsList)
            {
                item.Value.transform.parent = _backgrounds;
            }
            */
            var _back = LocationGO.Find( "background" );
            _back.parent = _content;
            Object.Destroy( LocationGO.gameObject );
            return _content;
        }

        private void ItemActivate(GameObject _item, string _name)
        {
            var _ho = _item.AddComponent<HO_HiddenObject>();
            _ho.Key = _name;

            string _difficulty = GetItemDifficulty( _item );
            _ho.Difficulty = _difficulty;

            var _buildItem = BuildItems[ _name ];

            if (_buildItem.Collider == null)
            {
                _buildItem.Collider = _buildItem.Image;
                _buildItem.Image.AddComponent<CircleCollider2D>();
            }

            if (Manager.LevelData.Mode != HOMode.Silhouette)
            {
                if (_buildItem.Silhouette != null)
                    Object.Destroy( _buildItem.Silhouette );
            }

            if (_buildItem.Glow != null)
            {
                Object.Destroy( _buildItem.Glow );
            }

            _ho.Image = _buildItem.Image;
            _ho.ShadowGO = _buildItem.Shadow;
            _ho.OverGO = _buildItem.Over;
            _ho.Collider = _buildItem.Collider.GetComponent<Collider2D>();

            _ho.Init( Manager );

            IHOClickableObject co;
            co = _buildItem.Collider.AddComponent<HO_ClickableObject>();
            co.Init( _ho );

        }
        #endregion

        #region TOOLS
        private string NameRepair(string name)
        {
            name = name.ToLower();
            name = name.Replace( " ", "" );
            name = name.Replace( 'с', 'c' );
            name = name.Replace( 'а', 'a' );
            name = name.Replace( 'в', 'b' );
            return name;
        }

        private void RepairObject(GameObject _item)
        {
            _item.name = NameRepair( _item.name );
            for (int i = 0; i < _item.transform.childCount; i++)
            {
                var _go = _item.transform.GetChild( i );
                _go.gameObject.name = NameRepair( _go.gameObject.name );
            }
        }

        private string GetItemName(string name)
        {
            return name.Remove( 0, 2 );
        }

        private string GetItemDifficulty(string name)
        {
            return GetNamePart( name, 0 );
        }

        private string GetNamePart(string fullName, int partIndex, char separator = '_')
        {
            string _part = "";
            _part = fullName.Split( separator )[ partIndex ].ToLower();
            return _part;
        }


        private Dictionary<string, GameObject> GetItemsList(string _difficulty)
        {
            return GetList( _difficulty, _allHiddenObjects );
        }

        private Dictionary<string, GameObject> GetList(string _difficulty, Dictionary<string, Dictionary<string, GameObject>> _list)
        {
            Dictionary<string, GameObject> _itemList;
            if (!_list.ContainsKey( _difficulty ))
            {
                _list.Add( _difficulty, new Dictionary<string, GameObject>() );
            }
            _itemList = _list[ _difficulty ];
            return _itemList;
        }
        #endregion
    }
}
