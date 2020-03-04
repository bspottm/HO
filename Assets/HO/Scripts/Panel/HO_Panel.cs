using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_Panel:HO_ModuleBehaviour, IHOModule
    {
        private IHOPanelItem hiddenObjectsPanel;
        private IHOPanelItem hintsPanel;

        private HO_UiProgressBar levelProgressBar;

        private Canvas canvas;

        protected override void RegisterModule()
        {
            Manager.ModuleRegister<HO_Panel>( this );
        }

        public override void Init(IHOManager Manager)
        {
            base.Init( Manager );
            InitCanvas();
            InitPanels();
        }

        private void InitCanvas()
        {
            canvas = GetComponent<Canvas>();
            canvas.worldCamera = Camera.main;
        }

        private void InitPanels()
        {
            hiddenObjectsPanel = HO_UiItem.GetElement<HO_Panel_HiddenObjects>("PanelHiddenObjects");
            hiddenObjectsPanel?.Init( this );

            hintsPanel = HO_UiItem.GetElement<HO_Panel_Hints>( "PanelHints" );
            hintsPanel?.Init( this );

            levelProgressBar = HO_UiItem.GetElement<HO_UiProgressBar>( "HOProgressBar" );
            levelProgressBar.Init( Manager );
            levelProgressBar.SetValue( 0, true );
        }


        public override void Send(HOMessage mess)
        {
            ReadMessagesInPlay(mess);
            ReadMessagesAlways(mess);
        }

        private void ReadMessagesInPlay(HOMessage mess)
        {
            if (!Manager.IsPlay)
                return;

            switch ((HOMessageType)mess.key)
            {
                case HOMessageType.Find:
                {
                    hiddenObjectsPanel.Send( mess );
                }
                break;

                case HOMessageType.FailClick:
                {
                    FailClickEffect();
                }
                break;
                case HOMessageType.Click:
                {
                    ReadButtonCommandsInPlay( mess );
                }
                break;
                case HOMessageType.QueryPanelHiddenObject:
                {
                    ReadQueryHiddenObject( mess );
                }
                break;

            }
        }

        private void ReadMessagesAlways(HOMessage mess)
        {
            switch ((HOMessageType)mess.key)
            {
                case HOMessageType.SetHiddenObjectList:
                {
                    if (hiddenObjectsPanel != null)
                    {
                        hiddenObjectsPanel.Send( mess );
                    }

                    InitButtons();
                }
                break;

                case HOMessageType.Click:
                {
                    ReadButtonCommandsAlways( mess );
                } 
                break;

                case HOMessageType.LevelProgressBarUpdate:
                {
                    if (!mess.hash.ContainsKey( "Value" ))
                        break;

                    levelProgressBar.SetValue( ( float )mess.hash[ "Value" ] );
                }
                break;

                case HOMessageType.HintButtonSetLock:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint(this);
#endif
                    hintsPanel.Send( mess );
                }
                break;

                case HOMessageType.HintSetAmount:
                {
#if UNITY_EDITOR
                    mess.AddTracePoint( this );
#endif
                    hintsPanel.Send( mess );
                }
                break;

            }
        }

        private void ReadButtonCommandsInPlay(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Command" ))
                return;

            switch (( HOClickCommand )mess.hash[ "Command" ])
            {
                case HOClickCommand.Hint:
                SendHintUse( mess.hash );
                break;
            }

        }

        private void ReadButtonCommandsAlways(HOMessage mess)
        {
            if (!mess.hash.ContainsKey( "Command" ))
                return;

            switch ((HOClickCommand)mess.hash[ "Command" ])
            {
                case HOClickCommand.Pause:
                Manager.Send( HOMessageType.Pause );
                break;


            }
        }

        private void FailClickEffect()
        {
            
            if (!Manager.LoadedItems.HasItem( "HUD_EffectMissClick" ))
                return;

            Vector3 _position = Input.mousePosition;
            _position.x -= canvas.pixelRect.width / 2f;
            _position.y -= canvas.pixelRect.height / 2f;
            _position.z = 0;
            var _go = Instantiate( Manager.LoadedItems.GetItem( "HUD_EffectMissClick" ), transform );
            _go.AddComponent<UIParticleAutodestroy>();
            _go.GetComponent<RectTransform>().anchoredPosition = _position;
        }

        private void InitButtons()
        {
            var _pauseButton = HO_UiItem.GetElement<HO_Button>( "btnPause" );
            _pauseButton.Init( this );


        }

        private void SendHintUse(Hashtable hash)
        {
            if (!hash.ContainsKey( "Hint" ))
                return;

            HOMessage _mess = CreateMessage( HOMessageType.HintUse );
            _mess.hash.Add( "Hint", hash[ "Hint" ] );
                Manager.Send( _mess );
        }

        private void ReadQueryHiddenObject(HOMessage mess)
        {
            switch ( mess.hash[ "Type" ].ToString() )
                {
                case "Get":
#if UNITY_EDITOR
                mess.AddTracePoint( this );
#endif
                hiddenObjectsPanel.Send( mess );
                break;
                case "Return":
#if UNITY_EDITOR
                mess.AddTracePoint( this );
#endif
                mess.hash[ "Type" ] = "Set";
                Manager.Send( mess );
                break;
                }
        }
    }
}
