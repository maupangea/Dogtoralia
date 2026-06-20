using System.Net;
using System.Text;

namespace Dogtoralia.Tests.Maui.Services;

/// <summary>
/// Minimal stub handler that records the last request and returns a canned response,
/// avoiding the need to wire up Moq's Protected() API for HttpMessageHandler.
/// </summary>
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpStatusCode _status;
    private readonly string _json;

    public HttpRequestMessage? LastRequest { get; private set; }

    public MockHttpMessageHandler(HttpStatusCode status, string json = "")
    {
        _status = status;
        _json = json;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        var response = new HttpResponseMessage(_status)
        {
            Content = new StringContent(_json, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
