using E_Commerce.Infrastructure.Shipment.Bosta.Contracts;
using System.Net;
using System.Net.Http.Headers;

public sealed class BostaAuthHandler : DelegatingHandler
{
    private readonly IBostaTokenService _tokenService;

    public BostaAuthHandler(IBostaTokenService tokenService)
    {
        _tokenService = tokenService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetAccessTokenAsync(cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        response.Dispose();

        await _tokenService.RefreshTokenAsync(cancellationToken);

        token = await _tokenService.GetAccessTokenAsync(cancellationToken);

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}