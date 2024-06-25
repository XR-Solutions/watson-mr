using System;

[Serializable]
public class DeviceCodeResponse
{
    public string device_code;
    public string user_code;
    public string verification_uri;
    public int expires_in;
    public int interval;
    public string verification_uri_complete;

}