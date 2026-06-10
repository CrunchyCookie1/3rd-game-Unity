using UnityEngine;
using UnityEngine.UI;

public class ImageColorChange : MonoBehaviour
{
    [SerializeField] private Image myImage;

    [SerializeField] private Color myColor;

    private void Start()
    {
        myColor.a = 255;

    }

    public void OnClick()
    {
        myImage.color = myColor;
    }
}
