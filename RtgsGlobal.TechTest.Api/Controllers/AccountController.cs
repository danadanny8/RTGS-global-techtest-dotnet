using Microsoft.AspNetCore.Mvc;

namespace RtgsGlobal.TechTest.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
	private readonly IAccountProvider _accountProvider;

	public AccountController(IAccountProvider accountProvider)
	{
		_accountProvider = accountProvider;
	}

	[HttpPost("{accountIdentifier}", Name = "Deposit")] //Worth specifying the "Deposit" route explicitly, eg, {accountIdentifier}/deposits
	public IActionResult Deposit(string accountIdentifier, [FromBody] float amount)
	{
		/***************************************************************************************************
		 * Validate "accountIdentifier", eg, valid length, format etc before calling Deposit()
		 *
		 * Create "DepositRequest" class instead of using "float amount"
		 * class DepositRequest
		 * {
		 *		[Required]
		 * 		[Range(10, 10_000)] //example range
		 *		decimal DepositAmount { get; set; }
		 * }
		 *
		 * What about currency code?
		 ****************************************************************************************************/

		if (amount < 0)
		{
			return BadRequest();
		}

		_accountProvider.Deposit(accountIdentifier, amount);
		return Ok();
	}

	[HttpPost("{accountIdentifier}/withdraw", Name = "Withdrawal")] //{accountIdentifier}/withdrawals - "withdrawals" as a resource might be more appropriate.
	public IActionResult Withdraw(string accountIdentifier, [FromBody] float amount)
	{
		//Validate "accountIdentifier", eg, valid length, format etc before calling Withdraw()
		//Similar to "Deposit()", we can create a "WithdrawRequest" class instead of using "float amount".
		_accountProvider.Withdraw(accountIdentifier, amount);
		return Ok();
	}

	[HttpPost("transfer", Name = "Transfer")]
	public IActionResult Transfer(MyTransferDto transfer)
	{
		/********************************************************************************************
		 * Rename "MyTransferDto" to "FundsTransferRequest" to better reflect its purpose/intent.
		 * class FundsTransferRequest
		 * {
		 *		[Required]
		 *		[StringLength(15, Minimum = 10)]
		 *		string DebitorAccountIdentifier {get; set;} //external account ref used by external participants.
		 *
		 *		[Required]
		 *		[StringLength(15, Minimum = 10)]
		 *		string CreditorAccountIdentifier {get; set;} //external account ref used by external participants.
		 *
		 * 		 [Required]
		 * 		[Range(10, 10_000)] //example range
		 *		decimal TransferAmount { get; set; }
		 * }
		 *
		 * Use a mapper class (or Automapper) to map "transfer" to DTO (FundsTransferDto) before calling "Transfer()".
		 ********************************************************************************************/
		
		if(string.Equals(transfer.DebtorAccountIdentifier, transfer.CreditorAccountIdentifier, 
        	StringComparison.InvariantCultureIgnoreCase))
        {
        	return BadRequest();
        }
		
		_accountProvider.Transfer(transfer);
		return Accepted();
	}

	[HttpGet("{accountIdentifier}", Name = "GetBalance")]
	//Validate "accountIdentifier", eg, valid length, format, before calling GetBalance()
    //Consider returning "IActionResult" instead of "MyBalance" type to handle cases where the account does not exist.
	public MyBalance Get(string accountIdentifier) => _accountProvider.GetBalance(accountIdentifier);
}

//Can be a record type to ensure immutability
//Rename "MyTransferDto" to "FundsTransferDto".
//Move to a separate file, eg, MyTransferDto.cs
public class MyTransferDto
{
	//ctor not required as it is a POCO.
	public MyTransferDto(string debtorAccountIdentifier, string creditorAccountIdentifier, float amount)
	{
		DebtorAccountIdentifier = debtorAccountIdentifier;
		CreditorAccountIdentifier = creditorAccountIdentifier;
		Amount = amount;
	}

	//Decorate with [Required], [StringLength(15, Minimum = 10)] attributes
	public string DebtorAccountIdentifier { get; set; }
	
	//Decorate with [Required], [StringLength(15, Minimum = 10)] attributes
	public string CreditorAccountIdentifier { get; set; }
	
	//Decorate with [Required], [Range(10, 10_000)] attributes
	//Use decimal instead of float for "amount" to avoid precision issues with financial calculations.
	public float Amount { get; set; }
}
