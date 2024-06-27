using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class CopilotScript : MonoBehaviour
{
    public GameObject RecordingIndicator;
    public AudioClip PromptStartAudio;
    public AudioClip PromptEndAudio;

    private ChatService _chatService = ChatService.Instance;
    private KeywordRecognizer _keywordRecognizer;
    private Dictionary<string, System.Action> _keywords = new();

    private int _recordFrequency = 44100;

    public void Start()
    {
        int minFreq, maxFreq;
        Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
        _recordFrequency = minFreq == 0 && maxFreq == 0 ? 44100 : maxFreq;

        _keywords.Add("Watson", () => StartListening());
        _keywords.Add("Hey", () => StartListening());

        _keywordRecognizer = new KeywordRecognizer(_keywords.Keys.ToArray());
        _keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        _keywordRecognizer.Start();
    }

    public void Update()
    {
        
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
        PhraseRecognitionSystem.Shutdown();
        AudioClip clip = Microphone.Start(null, true, 50, _recordFrequency);


        await Task.Delay(5000);
        StopListening();

        StartCoroutine(
                PlayAudio(await _chatService.InvokeAudioPrompt(clip)));
    }

    public void StopListening()
    {
        Microphone.End(null);
        PhraseRecognitionSystem.Restart();
        if (!RecordingIndicator.activeSelf) return;
        RecordingIndicator.SetActive(false);
        PlayChime(PromptEndAudio);
    }

    private void CheckMicrophoneVolume()
    {

    }

    private void PlayChime(AudioClip audioClip)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private IEnumerator PlayAudio(string path)
    {
        var www = new WWW($"file://{path}");
        yield return www;

        var clip = www.GetAudioClip(false, true, AudioType.MPEG);
        
        if (clip != null)
        {
            var audio = gameObject.AddComponent<AudioSource>();
            audio.clip = clip;
            audio.Play();
        }
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
