using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CopilotScript : MonoBehaviour
{
    public GameObject RecordingIndicator;
    public AudioClip PromptStartAudio;
    public AudioClip PromptEndAudio;

    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, System.Action> _keywords = new();

    private List<float> tempRecording = new();

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
        if (RecordingIndicator.activeSelf) return;
        RecordingIndicator.SetActive(true);
        PlayChime(PromptStartAudio);
        DoShit();
    }

    private async void DoShit()
    {
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        var recordFrequency = minFreq == 0 && maxFreq == 0 ? 44100 : maxFreq;

        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, 50, recordFrequency);
        audioSource.Play();
        //StopListening();
    }

    public void StopListening()
    {
        if (!RecordingIndicator.activeSelf) return;
        RecordingIndicator.SetActive(false);
        PlayChime(PromptEndAudio);
    }

    private void PlayChime(AudioClip audioClip)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StopListening();
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
