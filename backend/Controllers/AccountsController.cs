using System.Security.Claims;
using BankApi.DTOs;
using BankApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly IAccountService _accountService;
    public AccountsController(IAccountService accountService) => _accountService = accountService;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAccounts()
    {
        var accounts = await _accountService.GetAccountsAsync(GetUserId());
        return Ok(accounts);
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit(DepositWithdrawRequest request)
    {
        try
        {
            var result = await _accountService.DepositAsync(request, GetUserId());
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw(DepositWithdrawRequest request)
    {
        try
        {
            var result = await _accountService.WithdrawAsync(request, GetUserId());
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> Transfer(TransferRequest request)
    {
        try
        {
            await _accountService.TransferAsync(request, GetUserId());
            return Ok(new { message = "Transfer successful." });
        }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
