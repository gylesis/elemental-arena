using UnityEngine;
using UnityEngine.UI;

namespace Dev.UI
{
    public class DefaultImageButton : DefaultTextReactiveButton
    {
        [SerializeField] private Image _image;
        public void SetImageState(bool isOn)
        {
            _image.enabled = isOn;
        }
    }
}