using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HOSystem
{
    public class HO_Panel_HiddenObject_Slot_Anagram:HO_Panel_HiddenObject_Slot_Text
    {
        protected override void ApproveItem()
        {
            base.ApproveItem();
            if(!IsEmpty())
            {
                Label.text = GetShufledText( Label.text );
            }
        }

        private string GetShufledText(string text)
        {
            string[] words = text.Split( ' ' );
            string newString;
            do
            {
                newString = "";
                for (int y = 0; y < words.Length; y++)
                {
                    char[] str = words[ y ].ToCharArray();
                    for (int i = 0; i < 20; i++)
                    {
                        int a1 = Random.Range( 0, str.Length - 1 );
                        int a2 = Random.Range( 0, str.Length - 1 );
                        char temp = str[ a2 ];
                        str[ a2 ] = str[ a1 ];
                        str[ a1 ] = temp;
                    }
                    words[ y ] = new string( str );
                    newString += words[ y ];
                    if (y != ( words.Length - 1 ))
                        newString += " ";
                }
            } while (text == newString);
            return newString;
        }
    }
}
