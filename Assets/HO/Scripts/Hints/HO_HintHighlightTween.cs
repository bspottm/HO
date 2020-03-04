using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HOSystem
{
    public class HO_HintHighlightTween:MonoBehaviour
    {
        private SpriteRenderer image;
        private IHOManager core;
        private float tweenValue = 0;
        private Material imageMaterial;
        private float duration = 3f;
        private int defaultSortingOreder;
        private ParticleSystem particleEffect;

        public void Init(IHOManager manager)
        {
            core = manager;
            image = GetComponent<SpriteRenderer>();
            if (image == null || core == null)
            {
                Debug.Log( "Incorect anchor for tween" );
                Destroy( this );
            }
            imageMaterial = image.material;
            core.OnUpdate += Next;
            defaultSortingOreder = image.sortingOrder;
            image.sortingOrder = HO_HiddenObject.HIGHTLIGHTORDER;
            LoadParticlesEffect();
        }

        private void LoadParticlesEffect()
        {
            if (!core.LoadedItems.HasItem( "HUD_ItemHintEffect" ))
                return;

            var _go = Instantiate( core.LoadedItems.GetItem( "HUD_ItemHintEffect" ) );
            _go.transform.position = image.transform.position;
            particleEffect = _go.GetComponent<ParticleSystem>();
            var _shape = particleEffect.shape;

            if (_shape.shapeType == ParticleSystemShapeType.SpriteRenderer)
            {
                _shape.spriteRenderer = image;
            }
        }

        private void Next()
        {
            tweenValue += Time.deltaTime * 4;
            float _val = (.5f + .5f* Mathf.Sin( tweenValue )) * HO_HiddenObject.HIGLIGHTFACTOR;

            imageMaterial.color = Color.white * ( 1f +  _val);
            image.transform.localScale = Vector3.one + Vector3.one * _val;
            duration -= Time.deltaTime;
            if (duration <= 0 && Mathf.Abs(_val)<=.01f)
                Destroy( this );
        }

        public void StopTween()
        {
            image.sortingOrder = defaultSortingOreder;
            imageMaterial.color = Color.white;
            image.transform.localScale = Vector3.one;
            core.OnUpdate -= Next;
            StopParticles();
        }

        private void StopParticles()
        {
            if (particleEffect == null)
                return;

            particleEffect.Stop();
            Destroy( particleEffect.gameObject );
        }

        private void OnDestroy()
        {
            StopTween();
        }
    }
}
