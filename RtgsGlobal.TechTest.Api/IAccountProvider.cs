using RtgsGlobal.TechTest.Api.Controllers;

namespace RtgsGlobal.TechTest.Api;

public interface IAccountProvider
{
	MyBalance GetBalance(string accountIdentifier);
	void Deposit(string accountIdentifier, float amount);
	void Transfer(MyTransferDto transfer);
	void Withdraw(string accountIdentifier, float amount);
}

//AccountProvider - Move implementation into AccountProvider.cs
public class AccountProvider : IAccountProvider
{
	private readonly IDictionary<string, MyBalance> _accounts;

	public AccountProvider()
	{
		//Fake implementation. Normally, we would inject a repository/command class or DbContext to interact with the DB.
		_accounts = new Dictionary<string, MyBalance> { { "account-a", new MyBalance() }, { "account-b", new MyBalance() } };
	}

	//"accountIdentifier" should be considered as an external reference.
	//Should validate against DB if "accountIdentifier" exists or if account has been suspended/inactive before returning balance.
	//Consider returning AccountBalanceDto to the API/controller layer. 
	public MyBalance GetBalance(string accountIdentifier) => _accounts[accountIdentifier];

	//Use decimal instead of float for "amount"
	//"accountIdentifier", similar to GetBalance(), validate first before adding. 
	public void Deposit(string accountIdentifier, float amount) => AddTransaction(accountIdentifier, amount);

	public void Transfer(MyTransferDto transfer)
	{
		//Validate account status of both accounts.
		//Validate if debitor account has enough funds before transferring
		//Should be wrapped in a transaction to ensure atomicity
		AddTransaction(transfer.DebtorAccountIdentifier, -transfer.Amount);
		AddTransaction(transfer.CreditorAccountIdentifier, transfer.Amount);
	}

	//Validate account status
    	//Validate if account has enough funds before withdrawing
	public void Withdraw(string accountIdentifier, float amount) => AddTransaction(accountIdentifier, -1 * amount);

	//Use decimal instead of float for "amount"
	private void AddTransaction(string accountIdentifier, float amount)
	{
		//Fake implementation. Normally, this will be interacting with the domain entity via a repository/command/DbContext class.
		//This business logic below should be in the Account domain entity class, eg, account.AddTransaction(amount). 
		//This results in a rich domain model. 
		MyBalance accountBalance = _accounts[accountIdentifier];
		_accounts[accountIdentifier] =
			accountBalance with { Balance = accountBalance.Balance + amount };
	}
}

//Rename to "AccountBalanceDto" and move to AccountBalanceDto.cs
//Use decimal instead of float for "amount"
public record MyBalance(float Balance = 0);
