using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static Dictionary<N, M> ToDictionary<N, M>(this IEnumerable<KeyValuePair<N, M>> collection)
    {
        Dictionary<N, M> result = new Dictionary<N, M>();
        foreach (KeyValuePair<N, M> pair in collection)
            result.Add( pair.Key, pair.Value );

        return result;
    }
}
