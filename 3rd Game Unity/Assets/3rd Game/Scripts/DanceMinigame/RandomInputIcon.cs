using UnityEngine;
using UnityEngine.UI;

public class RandomInputIcon : MonoBehaviour
{
    [SerializeField] private Texture[] iconImages; // 0=Up, 1=Down, 2=Left, 3=Right
    [SerializeField] private RawImage theRawImage;

    public int currentIconIndex { get; private set; }

    private MoveInputObjects note;

    private void Awake()
    {
        note = GetComponent<MoveInputObjects>();
    }

    private void Start()
    {
        RandomizeIcons();
    }

    private void RandomizeIcons()
    {
        int randomIndex = Random.Range(0, iconImages.Length);
        currentIconIndex = randomIndex;
        theRawImage.texture = iconImages[randomIndex];

        // Push the index down to the note so the controller knows what input to expect
        if (note != null)
            note.currentIconIndex = randomIndex;
    }
}