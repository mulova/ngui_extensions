using System.Text.Ex;
using mulova.commons;
using mulova.comunity;
using UnityEngine;

namespace ngui.ex
{
    public class UIText : UILabel
    {
        public string textKey;

        public void Translate()
        {
            if (!textKey.IsEmpty() && Application.isPlaying) {
                text = Lexicon.Get(textKey);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Translate();
        }
    }
}

