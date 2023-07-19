using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GUIManager : MonoBehaviour
{
    public TextMeshProUGUI CommandHandlerText;  //<< The text box that we are writing to

    // The speech recognizer and the dictionary objects
    private KeywordRecognizer kRec;
    private Dictionary<string, System.Action> dictionary = new Dictionary<string, System.Action>();

    private bool isGUIVisible = false;          //<< Tracks the visibility of the GUI
    private const float FADE_TIME = 0.5f;       //<< Time in seconds for the transparacy change transition (fade in, fade out)
    private const float TEXT_DISPLAY_TIME = 2f; //<< For how long is the text displayed before it is cleared
    
    // Start is called before the first frame update
    void Start()
    {
        // Form the word dictionary for the speech recognizer and initialize it
        formDictionary();

        // Set the GUI initial settings
        GetComponent<CanvasGroup>().alpha = 0f;
        fadeHandler();

        // Set the text box initial settings
        CommandHandlerText.alpha = 0f;
        CommandHandlerText.text = string.Empty;
    }

    // Update is called once per frame
    void Update()
    {
        // The Controls for the GUI. Keyboard and mouse
        if (Input.GetKeyDown(KeyCode.L))
            setCommand("Left");
        else if (Input.GetKeyDown(KeyCode.R))
            setCommand("Right");
        else if (Input.GetKeyDown(KeyCode.Space) || (!isGUIVisible && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))))
            fadeHandler();
    }

    // Displays the text to the textbox for a given time 
    public void setCommand(string cmd) {
        StopAllCoroutines();
        StartCoroutine(fadeText(CommandHandlerText, 0f, 1f, 0f));
        CommandHandlerText.text = cmd;
        StartCoroutine(fadeText(CommandHandlerText, 1f, 0f, TEXT_DISPLAY_TIME));
    }

    // Handles the fade in/fade out transitions
    public void fadeHandler() {
        var canvGroup = GetComponent<CanvasGroup>();
        
        StartCoroutine(fadeGUI(canvGroup, canvGroup.alpha, isGUIVisible ? 0f : 1f));

        isGUIVisible = !isGUIVisible;
    }

    // Forms the canvas group transition between hidden and shown by modifying the alpha value over a given time frame
    IEnumerator fadeGUI(CanvasGroup canvGroup, float startAlpha, float endAlpha, float delayTime = 0.0f) {
        float time = 0.0f;

        while (time < FADE_TIME) {
            time += Time.deltaTime;
            canvGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FADE_TIME);

            yield return null;
        }
    }

    // Forms the text box transition between hidden and shown by modifying the alpha value over a given time frame
    IEnumerator fadeText(TextMeshProUGUI text, float startAlpha, float endAlpha, float delayTime = 0.0f) {
        yield return new WaitForSeconds(delayTime);

        float time = 0.0f;

        while (time < FADE_TIME) {
            time += Time.deltaTime;
            text.alpha = Mathf.Lerp(startAlpha, endAlpha, time / FADE_TIME);

            yield return null;
        }
    }

    // Sets the dictionary for the speech recognizer and initializes it
    private void formDictionary() {
        // Adding the word dictionary and teh action unvoked on detection
        dictionary.Add("Left", () => setCommand("Left"));
        dictionary.Add("Right", () => setCommand("Right"));
        dictionary.Add("Hide", () => {
            if (isGUIVisible) {
                StartCoroutine(fadeGUI(GetComponent<CanvasGroup>(), 1f, 0f));
                isGUIVisible = !isGUIVisible;
            }
        });
        dictionary.Add("Show", () => {
            if (!isGUIVisible) {
                StartCoroutine(fadeGUI(GetComponent<CanvasGroup>(), 0f, 1f));
                isGUIVisible = !isGUIVisible;
            }
        });

        // Initializing the kwyword recognizer
        kRec = new KeywordRecognizer(dictionary.Keys.ToArray());
        // Adding the invoker when words are detected
        kRec.OnPhraseRecognized += recognizedSpeechHandler;
        // Start the recognizer
        kRec.Start();
    }

    // Handles the action taken when a phrase is identified from the word dictionary
    private void recognizedSpeechHandler(PhraseRecognizedEventArgs speech) {
        dictionary[speech.text].Invoke();
    }
}
