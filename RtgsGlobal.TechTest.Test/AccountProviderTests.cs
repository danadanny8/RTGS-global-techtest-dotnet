using FluentAssertions;
using FluentAssertions.Execution;
using RtgsGlobal.TechTest.Api;
using RtgsGlobal.TechTest.Api.Responses;
using Xunit;

namespace RtgsGlobal.TechTest.Test;

public class AccountProviderTests
{
	[Fact]
	public void Given_InvalidAccountIdentifier_When_DepositIsCalled_Then_ReturnsFalse()
	{
		//Arrange
		var accountProvier = new AccountProvider();
		var invalidAccountIdentifier = "invalidAccountIdentifier";
		var amount = 100f;
		
		//Act
		var result = accountProvier.Deposit(invalidAccountIdentifier, amount);
		
		//Assert
		using (new AssertionScope())
		{
			result.Should().BeOfType<Result>();
			result.IsSuccess.Should().BeFalse();
			result.ErrorMessage.Should().Be("Account does not exist");
		}
	}
}
