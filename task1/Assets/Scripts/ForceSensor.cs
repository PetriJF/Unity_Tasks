using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForceSensor : MonoBehaviour
{
    // Used for representing the force reading and its magnitude
    private Vector3 sensorReading = Vector3.zero;
    private float magnitude = 0.0f;

    // Used for representing the color threshold for the arrows
    private const float DEFAULT_MAG_MIN_THRESHOLD = 3f;
    private const float DEFAULT_MAG_MAX_THRESHOLD = 7f;
    public float magMinThreshold = DEFAULT_MAG_MIN_THRESHOLD;
    public float magMaxThreshold = DEFAULT_MAG_MAX_THRESHOLD;

    public Color lowColor = Color.green;                        //<< The lower bound arrow color for the lower threshold
    public Color highColor = Color.red;                         //<< The upper bound arrow color for the upper threshold
    public int maxForceAxis = 3;                                //<< The maximum axis-wise force reading
    public int minForceAxis = -3;                               //<< The minimum axis-wise force reading

    private const float ARROW_SCALE_FACTOR = 1f;                //<< The default scale factor
    public float arrowScaleFactor = ARROW_SCALE_FACTOR;         //<< Stores the scale factor for the force reading

    public GameObject arrowTip;                                 //<< Arrow tip prefab
    public GameObject arrowLine;                                //<< Arrow line prefab

    private GameObject arrowTipInstance;                        //<< Instance of the arrow's tip
    private GameObject arrowLineInstance;                       //<< Instance of the arrow's line

    // Randomize the force reading (and therefor the magnitude) of the sensor
    public void SimulateForceSensor(out Vector3 forceReadings, out float magnitude) {
        // Generate random force vector
        forceReadings = new Vector3(Random.Range(minForceAxis, maxForceAxis + 1),
                                    Random.Range(minForceAxis, maxForceAxis + 1),
                                    Random.Range(minForceAxis, maxForceAxis + 1)
        );
        forceReadings = forceReadings + transform.position;
        // Compute the magnitude of the vector
        magnitude = forceReadings.magnitude;
    }

    // Setting the arrow based on the force reading
    public void SetArrow(Vector3 sensorReading, float magnitude) {
        arrowLineInstance = Instantiate(arrowLine);
        arrowTipInstance = Instantiate(arrowTip);

        arrowLineInstance.transform.parent = transform;
        arrowTipInstance.transform.parent = transform;

        // Adjust the y-axis scale of the arrowLine based on the magnitude variable
        Vector3 scale = arrowLineInstance.transform.localScale;
        scale.y = arrowScaleFactor * (magnitude / 2f);// * ARROW_SCALE_FACTOR;
        arrowLineInstance.transform.localScale = scale;

        Debug.Log("Magnitude: " + magnitude);
        Debug.Log("ForceReading: " + sensorReading);

        // Calculate the rotation to align the arrowLine with the force vector direction
        Quaternion rotation = Quaternion.LookRotation(sensorReading);

        arrowLineInstance.transform.localPosition = (arrowScaleFactor * sensorReading) / 2f;
        arrowLineInstance.transform.localRotation = rotation * Quaternion.Euler(90f, 0f, 0f);

        // Set the position and rotation of the arrowTipInstance to align with the other end of the arrowLineInstance
        arrowTipInstance.transform.localPosition = arrowScaleFactor * sensorReading;
        arrowTipInstance.transform.localRotation = rotation;// * Quaternion.Euler(270f, 0f, 0f);
    }

    public void updateArrowColor() {
        // Calculate the normalized magnitude value (between 0 and 1)
        float normalizedMagnitude = Mathf.Clamp01((magnitude - magMinThreshold) / (magMaxThreshold - magMinThreshold));

        // Interpolate the color between lowColor and highColor based on the normalized magnitude
        Color interpolatedColor = Color.Lerp(lowColor, highColor, normalizedMagnitude);

        // Assign the interpolated color to the arrowLine and arrowTip
        Renderer arrowLineRenderer = arrowLineInstance.GetComponent<Renderer>();
        MeshRenderer arrowTipRenderer = arrowTipInstance.GetComponentInChildren<MeshRenderer>();

        // Render the colors on the arrow instantiations
        arrowLineRenderer.material.color = interpolatedColor;
        arrowTipRenderer.material.color = interpolatedColor;
    }

    // Sets the parameters of the sensor. Ensure to call this after Instantiating a sensor object!
    public void setParams(float magMinThreshold, float magMaxThreshold, float arrowScaleFactor, Color lowColor, Color highColor) {
        this.magMinThreshold = magMinThreshold;
        this.magMaxThreshold = magMaxThreshold;
        this.arrowScaleFactor = arrowScaleFactor;
        this.lowColor = lowColor;
        this.highColor = highColor;

        SimulateForceSensor(out sensorReading, out magnitude);
        SetArrow(this.sensorReading, this.magnitude);
        updateArrowColor();
    }

    // Sets the lower threshold for the arrow's color scale
    public void setMinThreshold(float threshold) {
        magMinThreshold = threshold;
        updateArrowColor();
    }

    // Sets the upper threshold for the arrow's color scale
    public void setMaxThreshold(float threshold) {
        magMaxThreshold = threshold;
        updateArrowColor();
    }

    // Sets the scale factor of the arrow 
    public void setScaleFactor(float scaleFactor) {
        arrowScaleFactor = scaleFactor;
        removeArrows();
        SetArrow(sensorReading, magnitude);
        updateArrowColor();
    }

    // Removes the arrow representation of the sensor's force
    public void removeArrows() {
        if (arrowTipInstance != null)
            Destroy(arrowTipInstance);
        if (arrowLineInstance != null)
            Destroy(arrowLineInstance);
    }

    // Sets the color scale for the sensor's arrow
    public void setColorScale(Color lowColor, Color highColor) {
        this.lowColor = lowColor;
        this.highColor = highColor;

        updateArrowColor();
    }
}
