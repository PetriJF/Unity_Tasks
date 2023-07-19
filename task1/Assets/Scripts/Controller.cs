using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Controller : MonoBehaviour {
    private const int INITIAL_COUNT = 1;        //<< Determines how many sensors exist at the start of the scene
    private bool randomRotationToggle = false;  //<< Enables the sensor to randomize rotation when spawning

    private int sensorCloneCounter;             //<< Stores the number of active sensors in the scene 

    public GameObject sensorPrefab;             //<< The sensor prefab
    public GameObject sensorContainer;          //<< A container used to store all the sensors active
    public TextMeshProUGUI lowThresholdValue;   //<< THe Force lower bound threshold
    public TextMeshProUGUI highThresholdValue;  //<< The Force upper bound threshold
    public TextMeshProUGUI scaleFactorValue;    //<< Scale factor for the arrows

    private Color minArrowColor = Color.green;  //<< The color of the force lower bound threshold
    private Color maxArrowColor = Color.red;    //<< The color of the force upper bound threshold

    // List of all the sensors active in the scene
    public List<GameObject> sensors = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        sensorCloneCounter = 1;
        for (int i = 0; i < INITIAL_COUNT; i++)
            addSensor();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("space"))
            addSensor();
    }

    // Determines whether the sensors have random rotation as well on spawn (Enabling it makes things look more messy)
    public void setRandRotToggle(bool randomRotationToggle) {
        Debug.Log("Random Rotation " + ((randomRotationToggle) ? "enabled" : "disabled"));
        this.randomRotationToggle = randomRotationToggle;
    }

    // Adds a new sensor
    public void addSensor() {
        // Compute a random position for the sensor
        Vector3 randPos = new Vector3(Random.Range(-5, 6), Random.Range(1, 3), Random.Range(-5, 6));
        // Instantiate a new clone of the sensor at the random position
        //sensorPrefab
        GameObject sensor = Instantiate(sensorPrefab, randPos, ((this.randomRotationToggle) ? Random.rotation : Quaternion.identity));
        // Make the sensor a child of sensorContainer
        sensor.transform.parent = sensorContainer.transform;
        // Rename the sensor based on the number of clones already existing
        sensor.name = "Sensor_" + (sensorCloneCounter++);
        // Sets the parameters of the new sensor
        sensor.GetComponent<ForceSensor>().setParams(float.Parse(lowThresholdValue.text), float.Parse(highThresholdValue.text), float.Parse(scaleFactorValue.text),
                                                     minArrowColor, maxArrowColor);
        

        updateChildlist();
    }

    // Updates the List of the sensors in the scene
    private void updateChildlist() {
        // Get all the sensor clones in a list
        sensors = new List<GameObject>();
        foreach (Transform child in sensorContainer.transform)
            sensors.Add(child.gameObject);
    }

    // Removes all the sensors from the scenes
    public void removeAllSensors() { 
        // Delete all the sensors
        sensors.ForEach(child => Destroy(child));
        // Reset number of clones existing
        sensorCloneCounter = 1;
        // Update the child list
        sensors = new List<GameObject>();
    }

    // Updates the lower bound for the force threshold of the sensors
    public void updateMinThreshold() {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setMinThreshold(float.Parse(lowThresholdValue.text))
            );
    }

    // Updates the upper bound for the force threshold of the sensors
    public void updateMaxThreshold() {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setMaxThreshold(float.Parse(highThresholdValue.text))
            );
    }

    // Updates the scale factor for the arrows. (Used to make the arrows shorter)
    public void updateScaleFactor() {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setScaleFactor(float.Parse(scaleFactorValue.text))
            );
    }

    // Sets the color representative of the lower bound of the force threshold
    public void setMinArrowColor(Color min) {
        minArrowColor = min;
        updateArrowColors(minArrowColor, maxArrowColor);
    }

    // Sets the color representative of the upper bound of the force threshold
    public void setMaxArrowColor(Color max) {
        maxArrowColor = max;
        updateArrowColors(minArrowColor, maxArrowColor);
    }

    // Updates the colors of all the sensor force arrows based on the bound colors
    private void updateArrowColors(Color min, Color max) {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setColorScale(min, max)
            );
    }
}
