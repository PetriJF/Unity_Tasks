using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour, IPointerClickHandler {
    public Color output;                //<< Color the picker outputs
    public GameObject gameController;   //<< The game controller instance

    public bool isMinValue;             //<< Used as a quick check regarding the bound it the color picker represents

    // The on click event on the color picker. Sets the bounds based on the picker clicked
    public void OnPointerClick(PointerEventData EventData) {
        output = ColorHandler(EventData.position);
        if (isMinValue)
            gameController.GetComponent<Controller>().setMinArrowColor(output);
        else
            gameController.GetComponent<Controller>().setMaxArrowColor(output);
    }

    // Detects the color that was clicked
    private Color ColorHandler(Vector2 clickPos) {
        // Converting the screen point to local point. Used to determine where the click happened on the color picker
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, clickPos, null, out localPoint)) {
            Debug.LogError("Failed to convert screen point to local point.");
            return Color.white;
        }

        // Normalizing the coordinates
        float normalizedX = Mathf.InverseLerp(0f, rectTransform.rect.width, localPoint.x);
        float normalizedY = Mathf.InverseLerp(0f, rectTransform.rect.height, localPoint.y);
        
        // Mapping the coordinates to the texture of the image used for the color picker
        Texture2D texture = GetComponent<Image>().sprite.texture;
        int x = Mathf.RoundToInt(normalizedX * (texture.width - 1));
        int y = Mathf.RoundToInt(normalizedY * (texture.height - 1));

        if (x < 0 || x >= texture.width || y < 0 || y >= texture.height) {
            Debug.LogError("Invalid texture coordinates.");
            return Color.white;
        }
        
        // Returning the pixel color at the mapped coordinates
        return texture.GetPixel(x, y);
    }
}
