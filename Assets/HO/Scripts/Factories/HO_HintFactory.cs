using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HOSystem
{
    public class HO_HintFactory:MonoBehaviour
    {
        public static IHOHint GetHint(HOHintType type)
        {
            var _go = new GameObject( string.Format( "Hint_{0}", type.ToString() ) );

            switch (type)
            {
                default:
                case HOHintType.None:
                return null;
                case HOHintType.Eye:
                return _go.AddComponent<HO_Hint>();
            }
        }

    }
}
