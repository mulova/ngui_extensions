using UnityEngine;
using System.Collections;
using commons;
using comunity;

namespace ngui.ex
{
    public class UIText : UILabel
    {
        public string textKey;

        public void Translate()
        {
            if (textKey.IsNotEmpty() && Application.isPlaying) {
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

