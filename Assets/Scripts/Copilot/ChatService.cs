
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class ChatService
{
    private static ChatService _instance;
    // TODO: fix API URL
    private HttpClient _client = new() { BaseAddress = new System.Uri("http://localhost:5148") };

    private ChatService() { }

    public static ChatService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ChatService();
            }
            return _instance;
        }
    }

    public async Task<string> InvokeAudioPrompt(AudioClip audio)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.wav";
        var content = new MultipartFormDataContent();
        var fileStream = ConvertAudioToFileStream(audio, fileName);
        content.Add(new StreamContent(fileStream), "audio", fileName);

        var response = await _client.PostAsync("/api/v1/chat/richmessage", content);
        return await SaveHttpResponseAudio(response.Content);
    }

    public FileStream ConvertAudioToFileStream(AudioClip clip, string filename)
    {
        var trimmedClip = SavWav.TrimSilence(clip, 0);
        SavWav.Save(filename, trimmedClip);
        var fileName = Path.Combine(Application.persistentDataPath, filename);

        return File.OpenRead(fileName);
    }

    public async Task<string> SaveHttpResponseAudio(HttpContent content)
    {
        var fileName = $"{Guid.NewGuid().ToString()}.mp3";
        var savePath = Path.Combine(Application.persistentDataPath, fileName);
        FileStream fileStream = new(savePath, FileMode.Create, FileAccess.Write);
        await (await content.ReadAsStreamAsync()).CopyToAsync(fileStream);
        fileStream.Close();

        return savePath;
    }
}