using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEngine;

public class AuthService
{
    private static AuthService _instance;
    private HttpClient _client = new() { BaseAddress = new System.Uri("https://xr-solutions.eu.auth0.com/") };

    private readonly string clientId = "dnLGq8sQnIS5dVJjKEFCr0LGgf4Y7EIA";
    private readonly string scope = "offline_access+openid+profile";
    private readonly string grantType = "urn:ietf:params:oauth:grant-type:device_code";

    private AuthService() { }

    public static AuthService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new AuthService();
            }
            return _instance;
        }
    }

    public async Task<DeviceCodeResponse> GetNewDeviceCode()
    {
        var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "scope", scope }
        });

        var response = await _client.PostAsync("oauth/device/code", formContent);
        var json = await response.Content.ReadAsStringAsync();
        return JsonUtility.FromJson<DeviceCodeResponse>(json);
    }

    public async Task<ObtainTokenResponse> GetTokenFromActivatedDevice(string deviceCode)
    {
        var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", clientId },
            { "grant_type", grantType },
            { "device_code", deviceCode }
        });

        var response = await _client.PostAsync("oauth/token", formContent);
        var json = await response.Content.ReadAsStringAsync();
        return JsonUtility.FromJson<ObtainTokenResponse>(json);
    }
}
