using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RtgsGlobal.TechTest.Api;
using RtgsGlobal.TechTest.Api.Controllers;
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
}
