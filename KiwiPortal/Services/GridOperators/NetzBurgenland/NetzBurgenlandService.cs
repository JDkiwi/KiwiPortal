using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using AutoMapper;
using KiwiPortal.Models;
using KiwiPortal.Models.NetzBurgenland;

namespace KiwiPortal.Services.GridOperators.NetzBurgenland;

public class NetzBurgenlandService : IGridOperatorService
{
    public bool LoggedIn { get; private set; }
    
    private HttpClient _client = new();
    
    private readonly IMapper _mapper;

    private string _username = string.Empty;
    
    private string _password = string.Empty;

    public NetzBurgenlandService(IMapper mapper)
    {
        _mapper = mapper;
        
        Reset();
    }
    
    public async Task Login(string username, string password)
    {
        try
        {
            _username = username;
            _password = password;
        
            var portalUrl = "https://smartmeter.netzburgenland.at/enView.Portal";
        
            var httpRequestMessage =
                new HttpRequestMessage(HttpMethod.Get, portalUrl);

            var result = await _client.SendAsync(httpRequestMessage);

            if ((result.RequestMessage?.RequestUri?.AbsoluteUri ?? "").Contains(portalUrl))
            {
                LoggedIn = true;
            
                return;
            }
        
            var htmlResult = await result.Content.ReadAsStringAsync();
            var regex = new Regex("<form id=\"kc-form-login\".*action=\"(.*?)\"", RegexOptions.Singleline);
            var match = regex.Match(htmlResult);
            var loginUrl = HttpUtility.HtmlDecode(match.Groups[1].Value);
            var loginHttpRequestMessage = new HttpRequestMessage(HttpMethod.Post, loginUrl);

            loginHttpRequestMessage.Content = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new("username", username),
                new("password", password),
            });

            var loginResult = await _client.SendAsync(loginHttpRequestMessage);

            if (loginResult.StatusCode != HttpStatusCode.OK || !(loginResult.RequestMessage?.RequestUri?.AbsoluteUri ?? "").Contains(portalUrl))
            {
                throw new Exception("Login failed");
            }
            
            LoggedIn = true;
        }
        catch
        {
            Reset();
            
            throw;
        }
    }

    public async Task Logout()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "https://smartmeter.netzburgenland.at/EnView.Portal/Account/LogOff");

        await _client.SendAsync(httpRequestMessage);
        
        Reset();
    }

    public async Task<IEnumerable<ConsumptionResult>?> GetConsumptionData(MeteringPoint meteringPoint, DateOnly startDate, DateOnly endDate)
    {
        await Login(_username, _password);

        var startDateTime = startDate.ToDateTime(new TimeOnly(0, 0, 0));
        var endDateTime = endDate.ToDateTime(new TimeOnly(0, 0, 0));

        var startDateTimeOffset = new DateTimeOffset(startDateTime);
        var endDateTimeOffset = new DateTimeOffset(endDateTime);
        
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            $"https://smartmeter.netzburgenland.at/enView.Portal/api/consumption/date?start={startDateTimeOffset.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fff}Z&end={endDateTimeOffset.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fff}Z&meteringPointIdentifier={meteringPoint.Identifier}");

        var result = await _client.SendAsync(httpRequestMessage);
        var jsonResult = await result.Content.ReadAsStringAsync();
        
        var consumptions = JsonSerializer.Deserialize<List<NetzBurgenlandConsumptionResult>>(jsonResult, DefaultJsonSerializerOptions.Create());
        
        return _mapper.Map<List<ConsumptionResult>>(consumptions);
    }
    
    public async Task<IEnumerable<MeteringPoint>?> GetMeteringPoints()
    {
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get,
            "https://smartmeter.netzburgenland.at/enView.Portal/api/meteringpoints/consumption");

        var result = await _client.SendAsync(httpRequestMessage);
        var jsonResult = await result.Content.ReadAsStringAsync();
        
        var meteringPoints = JsonSerializer.Deserialize<List<NetzBurgenlandMeteringPoint>>(jsonResult, DefaultJsonSerializerOptions.Create());

        return _mapper.Map<List<MeteringPoint>>(meteringPoints);
    }

    private void Reset()
    {
        _username = string.Empty;
        _password = string.Empty;

        LoggedIn = false;
        
        var cookieContainer = new CookieContainer();
        
        var handler = new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = cookieContainer,
        };
        
        _client = new HttpClient(handler);
    }
}
