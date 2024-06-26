using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CopilotScript : MonoBehaviour
{
    public GameObject RecordingIndicator;

    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, System.Action> _keywords = new();

    public void Start()
    {
        _keywords.Add("Watson", () => StartListening());
        _keywords.Add("Hey", () => StartListening());

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    public async void StartListening()
    {
        RecordingIndicator.SetActive(true);
        await Task.Delay(5000);
        StopListening();
    }

    public void StopListening()
    {
        RecordingIndicator.SetActive(false);
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (_keywords.TryGetValue(args.text, out keywordAction))
            keywordAction.Invoke();
    }
    public void OnApplicationQuit()
    {
        if (_keywordRecognizer != null && _keywordRecognizer.IsRunning)
        {
            _keywordRecognizer.OnPhraseRecognized -= KeywordRecognizer_OnPhraseRecognized;
            _keywordRecognizer.Stop();
        }
    }
}
