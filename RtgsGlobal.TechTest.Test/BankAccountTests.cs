using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using RtgsGlobal.TechTest.Api;
using RtgsGlobal.TechTest.Api.Controllers;
using Xunit;

namespace RtgsGlobal.TechTest.Test;

//Name of test class could be renamed to reflect that it is an inetgration test against AccountController.
//Categorise this test as "Integration"
public class BankAccountTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public BankAccountTests(WebApplicationFactory<Program> fixture)
	{
		_client = fixture
			.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
			{
				//Tests rely on AccountProvider singleton which will share state between tests. 
				//More appropriate to use Scoped lifetime as it is scoped within each HTTP request.
				services.AddSingleton<IAccountProvider, AccountProvider>();
			}))
			.CreateDefaultClient();
	}

	[Fact]
	public async Task GivenAccountExistsWithNoTransactions_ThenGetBalanceShouldReturnZero()
	{
		//Might be worth having a Setup() and TearDown() so that each test state is isolated.
		var result = await _client.GetFromJsonAsync<MyBalance>("/account/account-a");

		//Assert if result is null.
		Assert.Equal(0, result.Balance);
	}

	[Fact]
	public async Task GivenAccountExists_WhenDepositIsAdded_ThenGetBalanceShouldReturnExpected()
	{
		//Passing raw string as JSON body may cause deserialisation issues.
        //Better to pass in a DTO such as DepositRequest instead.
        //It also clarifies the intent. 
		//Have an uri like "/account/account-a/deposits" - deposits as a resource.
		await _client.PostAsJsonAsync("/account/account-a", "1000");
		var result = await _client.GetFromJsonAsync<MyBalance>("/account/account-a");

		Assert.Equal(1000, result.Balance);
	}

	[Fact]
	public async Task GivenAccountExistsAndDepositIsAdded_WhenWithdrawalIsAdded_ThenGetBalanceShouldReturnExpected()
	{
		await _client.PostAsJsonAsync("/account/account-a", "1000");

		//"/account/account-a/withdraw" - "withdraw" is an action. 
		//"/account/account-a/withdrawals" might be more appropriate
		await _client.PostAsJsonAsync("/account/account-a/withdraw", "100");
		var result = await _client.GetFromJsonAsync<MyBalance>("/account/account-a");

		Assert.Equal(900, result.Balance);
	}

	[Fact]
	public async Task GivenAccountExists_WhenMultipleDepositsAreAdded_ThenGetBalanceShouldReturnExpected()
	{
		await _client.PostAsJsonAsync("/account/account-a", "1000");
		await _client.PostAsJsonAsync("/account/account-a", "2000");
		var result = await _client.GetFromJsonAsync<MyBalance>("/account/account-a");

		Assert.Equal(3000, result.Balance);
	}

	[Fact]
	public async Task GivenAccountExists_WhenTransferIsMade_ThenGetBalanceShouldReturnExpected()
	{
		await _client.PostAsJsonAsync("/account/transfer", new MyTransferDto("account-a", "account-b", 1000));
		var accountA = await _client.GetFromJsonAsync<MyBalance>("/account/account-a");
		var accountB = await _client.GetFromJsonAsync<MyBalance>("/account/account-b");

		Assert.Equal(-1000, accountA.Balance);
		Assert.Equal(1000, accountB.Balance);
	}
}
