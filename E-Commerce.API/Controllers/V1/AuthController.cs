using Asp.Versioning;
using E_Commerce.API.Common.Responses;
using E_Commerce.API.Contracts.Requests.AuthRequests;
using E_Commerce.Application.Features.Auth.Commands.ChangeUserPassword;
using E_Commerce.Application.Features.Auth.Commands.ForgetUserPassword;
using E_Commerce.Application.Features.Auth.Commands.LoginUser;
using E_Commerce.Application.Features.Auth.Commands.RefreshUserToken;
using E_Commerce.Application.Features.Auth.Commands.RegisterUser;
using E_Commerce.Application.Features.Auth.Commands.ResetUserPassword;
using E_Commerce.Application.Features.Auth.Commands.TwoFactorAuth;
using E_Commerce.Application.Features.Auth.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers.V1;

[ApiController]
[ApiVersion(1)]
[Route("api/v{version:apiVersion}/auth")]
public sealed partial class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status409Conflict)]
    public async Task<ApiResult> Register([FromBody] RegisterUserRequest request, CancellationToken ct)
    {
        var command = new RegisterUserCommand(request.FirstName, request.LastName, request.Email, request.Password, request.PhoneNumber);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Account created successfully. Please check your email to verify your account.");
    }

    [HttpPost("email-verification/resend")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> ResendVerificationEmail([FromBody] ResendVerificationEmailRequest request, CancellationToken ct)
    {
        var command = new ResendEmailCommand(request.Email);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "If the account exists and is eligible, a verification email will be sent.");
    }

    [HttpPost("email-verification/confirm")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> VerifyEmail([FromQuery] VerifyEmailRequest request, CancellationToken ct)
    {
        var command = new VerifyEmailCommand(request.Token);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Email verified successfully.");
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<LoginUserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<LoginUserResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<LoginUserResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<LoginUserResponse>> Login([FromBody] LoginUserRequest request, CancellationToken ct)
    {
        var command = new LoginUserCommand(request.Email, request.Password);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Login completed successfully.");
    }

    [Authorize]
    [HttpPost("two-factor/totp/setup")]
    [ProducesResponseType(typeof(ApiResult<SetupTwoFactorAuthResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<SetupTwoFactorAuthResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<SetupTwoFactorAuthResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<SetupTwoFactorAuthResponse>> SetupTotp([FromBody] SetupTwoFactorAuthRequest request, CancellationToken ct)
    {
        var command = new SetupTwoFactorAuthCommand(request.Password);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "TOTP setup data generated successfully.");
    }

    [Authorize]
    [HttpPost("two-factor/totp/confirm")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult> ConfirmTotpSetup([FromBody] VerifyTwoFactorAuthRequest request, CancellationToken ct)
    {
        var command = new VerifyTwoFactorAuthCommand(request.OtpCode);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Two-factor authentication has been enabled successfully.");
    }

    [HttpPost("login/two-factor/totp/confirm")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<FinalizeLoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<FinalizeLoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<FinalizeLoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<FinalizeLoginResponse>> ConfirmLoginTotp([FromBody] CompleteLoginWithTwoFactorRequest request, CancellationToken ct)
    {
        var command = new VerifyLoginTwoFactorAuthCommand(request.ChallengeId, request.OtpCode);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Login completed successfully.");
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResult<RefreshTokensResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult<RefreshTokensResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<RefreshTokensResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult<RefreshTokensResponse>> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Tokens refreshed successfully.");
    }

    [Authorize]
    [HttpPost("password/change")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult> ChangePassword([FromBody] ChangeUserPasswordRequest request, CancellationToken ct)
    {
        var command = new ChangeUserPasswordCommand(request.CurrentPassword, request.NewPassword);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Password changed successfully.");
    }

    [AllowAnonymous]
    [HttpPost("password/forgot")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    public async Task<ApiResult> ForgotPassword([FromBody] ForgetUserPasswordRequest request, CancellationToken ct)
    {
        var command = new ForgetUserPasswordCommand(request.Email);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "If the account exists and is eligible, a password reset email will be sent.");
    }

    [AllowAnonymous]
    [HttpPost("password/reset")]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResult> ResetPassword([FromBody] ResetUserPasswordRequest request, CancellationToken ct)
    {
        var command = new ResetUserPasswordCommand(request.Token, request.NewPassword);
        var result = await _sender.Send(command, ct);
        return this.FromResult(result, "Password reset successfully.");
    }
}
