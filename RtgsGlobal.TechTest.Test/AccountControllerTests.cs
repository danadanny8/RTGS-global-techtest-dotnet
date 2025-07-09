using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RtgsGlobal.TechTest.Api;
using RtgsGlobal.TechTest.Api.Controllers;
using RtgsGlobal.TechTest.Api.Responses;
using Xunit;

namespace RtgsGlobal.TechTest.Test;

public class AccountControllerTests
{
	[Fact]
	public void Given_NegativeAmount_When_DepositIsCalled_ReturnsBadRequest()
	{
		//Arrange
		var fakeAccountProvider = new Mock<IAccountProvider>();
		var controller = new AccountController(fakeAccountProvider.Object);
		var testAccountId = "testAccountId";
		var negativeAmount = -100f;
		
		//Act
		var response = controller.Deposit(testAccountId, negativeAmount);
		
		//Assert
		response.Should().BeOfType<BadRequestResult>();
	}
	
	[Fact]
	public void Given_InvalidAccountIdentifier_When_DepositIsCalled_Then_ReturnsBadRequest()
	{
		//Arrange
		var result = new Result { IsSuccess = false, ErrorMessage = "Account does not exist" };
		var fakeAccountProvider = new Mock<IAccountProvider>();
		fakeAccountProvider
			.Setup(x => x.Deposit(It.IsAny<string>(), It.IsAny<float>()))
			.Returns(result);

		var controller = new AccountController(fakeAccountProvider.Object);
		var invalidAccountIdentifier = "invalidAccountIdentifier";
		var amount = 100f;
		
		//Act
		var response = controller.Deposit(invalidAccountIdentifier, amount);
		
		//Assert
		using (new AssertionScope())
		{
			response.Should().BeOfType<BadRequestObjectResult>();
			response
				.As<BadRequestObjectResult>()
				.Value.Should().BeOfType<AccountApiError>();
			response
				.As<BadRequestObjectResult>()
				.Value.As<AccountApiError>()
				.Message.Should().Be("Account does not exist");
		}
		
	}
	
	[Fact]
    public void
    	Given_DebtorAccountIdentifierAndCreditorAccountIdentifier_AreTheSame_When_TransferIsCalled_Then_ReturnsBadRequest()
    {
    	//Arrange
    	var fakeAccountProvider = new Mock<IAccountProvider>();
    	var controller = new AccountController(fakeAccountProvider.Object);
    	var debtorAccountIdentifier = "SameAccountId";
    	var creditorAccountIdentifier = "SameAccountId";
    	var amount = 100f;

    	var testTransferDto = new MyTransferDto(debtorAccountIdentifier, creditorAccountIdentifier, amount);
    	
    	//Act
    	var response = controller.Transfer(testTransferDto);
    	
    	//Assert
    	response.Should().BeOfType<BadRequestResult>();
    }
}
