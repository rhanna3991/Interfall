using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIParallaxLooper : MonoBehaviour
{
    [SerializeField] private RectTransform copy;
    [SerializeField] private float scrollSpeed = 30f;
    [SerializeField] private bool debugMode = false;
    
    private RectTransform self;
    private float height;
    private bool initialized = false;

    void Start()
    {
        self = GetComponent<RectTransform>();
        
        if (copy == null)
        {
            Debug.LogError("Copy RectTransform not assigned!");
            return;
        }
        
        StartCoroutine(InitializeAfterFrame());
    }

    private IEnumerator InitializeAfterFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        
        height = self.rect.height;
        
        if (height <= 0f)
        {
            Debug.LogError($"Invalid height: {height}. Check image/content size.");
            yield break;
        }
        
        // Position copy exactly one height below
        copy.anchoredPosition = self.anchoredPosition - new Vector2(0, height);
        initialized = true;
        
        if (debugMode)
            Debug.Log($"Initialized - Height: {height}, Self Y: {self.anchoredPosition.y}, Copy Y: {copy.anchoredPosition.y}");
    }

    void Update()
    {
        if (!initialized) return;

        float move = scrollSpeed * Time.unscaledDeltaTime;

        // Move both upward
        self.anchoredPosition += Vector2.up * move;
        copy.anchoredPosition += Vector2.up * move;

        // Wrap when fully scrolled past
        if (self.anchoredPosition.y >= height)
        {
            self.anchoredPosition -= new Vector2(0, height * 2);
            if (debugMode) Debug.Log("Self wrapped");
        }

        if (copy.anchoredPosition.y >= height)
        {
            copy.anchoredPosition -= new Vector2(0, height * 2);
            if (debugMode) Debug.Log("Copy wrapped");
        }
    }
}