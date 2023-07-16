using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class randomSpawner : MonoBehaviour {
    private const int INITIAL_COUNT = 1;
    private bool randomRotationToggle = false;

    private int sensorCloneCounter;

    public GameObject sensorPrefab;
    public GameObject sensorContainer;
    public TextMeshProUGUI lowThresholdValue;
    public TextMeshProUGUI highThresholdValue;
    public TextMeshProUGUI scaleFactorValue;

    private Color minArrowColor = Color.green;
    private Color maxArrowColor = Color.red;


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

    public void setRandRotToggle(bool randomRotationToggle) {
        Debug.Log("Random Rotation " + ((randomRotationToggle) ? "enabled" : "disabled"));
        this.randomRotationToggle = randomRotationToggle;
    }

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

        sensor.GetComponent<ForceSensor>().setParams(float.Parse(lowThresholdValue.text), float.Parse(highThresholdValue.text), float.Parse(scaleFactorValue.text),
                                                     minArrowColor, maxArrowColor);
        

        updateChildlist();
    }

    private void updateChildlist() {
        // Get all the sensor clones in a list
        sensors = new List<GameObject>();
        foreach (Transform child in sensorContainer.transform)
            sensors.Add(child.gameObject);
    }

    public void removeAllSensors() { 
        // Delete all the sensors
        sensors.ForEach(child => Destroy(child));
        // Reset number of clones existing
        sensorCloneCounter = 1;
        // Update the child list
        updateChildlist();
    }

    public void updateMinThreshold() {
        Debug.Log(sensors.Count);
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setMinThreshold(float.Parse(lowThresholdValue.text))
            );
    }

    public void updateMaxThreshold() {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setMaxThreshold(float.Parse(highThresholdValue.text))
            );
    }

    public void updateScaleFactor() {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setScaleFactor(float.Parse(scaleFactorValue.text))
            );
    }

    public void setMinArrowColor(Color min) {
        minArrowColor = min;
        updateArrowColors(minArrowColor, maxArrowColor);
    }

    public void setMaxArrowColor(Color max) {
        maxArrowColor = max;
        updateArrowColors(minArrowColor, maxArrowColor);
    }

    private void updateArrowColors(Color min, Color max) {
        if (sensors.Count != 0)
            sensors.ForEach(sensor =>
                sensor.GetComponent<ForceSensor>().setColorScale(min, max)
            );
    }
}
