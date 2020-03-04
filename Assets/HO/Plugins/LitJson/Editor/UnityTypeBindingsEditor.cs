#if !JSON_STANDALONE

using System;

namespace LitJson
{
	//just a wrapper to move the UnityTypeBindings work without needing the editor
	[UnityEditor.InitializeOnLoad]
	public static class UnityTypeBindingsEditor 
	{
		static UnityTypeBindingsEditor()
		{
			//calling static constructor of UnityTypeBindings
			Type type = typeof(LitJson.UnityTypeBindings);
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle); 
		}
	}
}
#endif
