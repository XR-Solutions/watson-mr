using System;

[Serializable]
public class ObtainTokenResponse
{
    public string access_token;
    public int expires_in;
    public string token_type;
}