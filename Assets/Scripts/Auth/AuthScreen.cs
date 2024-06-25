using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AuthScreen : MonoBehaviour
{
    public GameObject WelcomeContent;
    public GameObject LoginContent;
    public GameObject LoadingIcon;
    public GameObject AuthCodeLabel;

    private bool _hasSubmittedCode = false;
    private AuthService _authService = AuthService.Instance;
    private int _expiresInSeconds = 900;
    private DateTime _lastObtainedCode = DateTime.UtcNow;

    public void Start()
    {
        SubscribeToCode();
    }

    public void Update()
    {
        UpdateTimeIndicator();
    }

    public void OnDestroy()
    {
        _hasSubmittedCode = true;
    }

    private async Task SubscribeToCode()
    {
        while (!_hasSubmittedCode)
        {
            var deviceCode = await RetrieveDeviceCode();
            LongPollDeviceActivation(deviceCode);
            await Task.Delay(_expiresInSeconds * 1000);
        }
    }

    private async Task<DeviceCodeResponse> RetrieveDeviceCode()
    {
        var response = await _authService.GetNewDeviceCode();
        AuthCodeLabel.GetComponent<TextMeshProUGUI>().text = response.user_code;
        _expiresInSeconds = response.expires_in;
        _lastObtainedCode = DateTime.UtcNow;

        return response;
    }

    private async Task LongPollDeviceActivation(DeviceCodeResponse deviceCode)
    {
        for (int i = 0; i < (deviceCode.expires_in / deviceCode.interval); i++)
        {
            await Task.Delay(deviceCode.interval * 1000);
            
            var tokenResponse = await _authService.GetTokenFromActivatedDevice(deviceCode.device_code);
            if (tokenResponse.access_token != null) SignInAndMoveScene(tokenResponse);
        }
    }

    private void UpdateTimeIndicator()
    {
        var now = DateTime.UtcNow;
        TimeSpan difference = now - _lastObtainedCode;
        double seconds = difference.TotalSeconds;

        var percentage = 1 - (seconds / _expiresInSeconds);

        LoadingIcon.GetComponent<Image>().fillAmount = (float)percentage;
    }

    private void SignInAndMoveScene(ObtainTokenResponse tokenResponse)
    {
        SceneManager.LoadScene("MainScene");
    }

    public void GetStarted()
    {
        WelcomeContent.SetActive(false);
        LoginContent.SetActive(true);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
