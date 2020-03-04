using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using HO.Scripts.Common.Base.Input;

namespace HOSystem
{
    public class HO_ClickableManager:HO_Module
    {

        public override void Init(IHOManager Manager)
        {
            base.Init( Manager );
            //Manager.AddToUpdate( this );
            GameInput.Instance.OnTouch += OnClick;
        }

        private void OnClick()
        {
            if (!Manager.IsPlay)
                return;

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            var mousePosition = Input.mousePosition;

            var _colliders = Physics2D.OverlapPointAll( Camera.main.ScreenToWorldPoint( mousePosition ) );

            if (_colliders.Length > 0)
            {
                var  _cols = _colliders.OrderByDescending( s => s.GetComponent<SpriteRenderer>() != null ?
                     s.GetComponent<SpriteRenderer>().sortingOrder : 0 );

                foreach (var item in _cols)
                {
                    var _item = item.GetComponent<IHOClickableObject>();

                    if (_item != null)
                    {
                        ItemClick( _item );
                        return;
                    }
                }
            }
                FailClick();
        }

        private void ItemClick(IHOClickableObject item)
        {
            Log( "ItemClick" );
            item.Send( HOMessageType.Click );
        }

        private void FailClick()
        {
            Manager.Send( HOMessageType.FailClick );
        }

        public override void Remove()
        {
            base.Remove();
        }
    }
}
