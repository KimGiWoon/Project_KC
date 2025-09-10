using UnityEngine;
using UnityEngine.UI;

namespace SDW
{
    public struct PopupDescription
    {
        public Sprite Sprite;
        public string Name;
        public string Effect;
        public string Description;

        public PopupDescription(Sprite sprite, string name, string effect, string description)
        {
            Sprite = sprite;
            Name = name;
            Effect = effect;
            Description = description;
        }
    }
}