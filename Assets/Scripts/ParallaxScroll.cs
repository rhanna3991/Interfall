using UnityEngine;
using UnityEngine.UI;

public class ParallaxScrollUI : MonoBehaviour
{
    public float speed = 30f;        // Upward speed
    public float scrollHeight = 270f; // Canvas height in px
    private RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Move the layer upward each frame
        rt.anchoredPosition += Vector2.up * speed * Time.unscaledDeltaTime;

        if (rt.anchoredPosition.y >= scrollHeight)
        {
            rt.anchoredPosition -= new Vector2(0, scrollHeight);
        }
    }
}
