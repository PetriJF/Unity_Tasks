using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour, IPointerClickHandler {
    public Color output;
    public GameObject gameController;

    public bool isMinValue;

    public void OnPointerClick(PointerEventData EventData) {
        output = ColorHandler(EventData.position);
        if (isMinValue)
            gameController.GetComponent<randomSpawner>().setMinArrowColor(output);
        else
            gameController.GetComponent<randomSpawner>().setMaxArrowColor(output);
    }

    private Color ColorHandler(Vector2 clickPos) {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, clickPos, null, out localPoint)) {
            Debug.LogError("Failed to convert screen point to local point.");
            return Color.white;
        }

        float normalizedX = Mathf.InverseLerp(0f, rectTransform.rect.width, localPoint.x);
        float normalizedY = Mathf.InverseLerp(0f, rectTransform.rect.height, localPoint.y);

        Texture2D texture = GetComponent<Image>().sprite.texture;
        int x = Mathf.RoundToInt(normalizedX * (texture.width - 1));
        int y = Mathf.RoundToInt(normalizedY * (texture.height - 1));

        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height) {
            Debug.LogError("Invalid texture coordinates.");
            return Color.white;
        }

        return texture.GetPixel(x, y);
    }
}
