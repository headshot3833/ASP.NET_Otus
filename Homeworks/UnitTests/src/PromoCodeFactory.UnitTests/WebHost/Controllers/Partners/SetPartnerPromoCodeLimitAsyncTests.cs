using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Controllers;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PromoCodeFactory.UnitTests.WebHost.Controllers.Partners
{
    public class SetPartnerPromoCodeLimitAsyncTests
    {
        private readonly PartnersController _partnersController;
        private readonly Mock<IRepository<Partner>> _partnersRepositoryMock;

        public SetPartnerPromoCodeLimitAsyncTests()
        {
            _partnersRepositoryMock = new Mock<IRepository<Partner>>();
            _partnersController = new PartnersController(_partnersRepositoryMock.Object);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerNotFound_Returns404()
        {
            Fixture AutoFixture = new Fixture();
            var request = AutoFixture.Create<SetPartnerPromoCodeLimitRequest>();
            var id = Guid.NewGuid();

            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync((Partner)null);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(id, request);

            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>()
                .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_PartnerNotFound_Return400()
        {
            var fixture = new Fixture();
            fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            var request = fixture.Create<SetPartnerPromoCodeLimitRequest>();
            var id = Guid.NewGuid();
            var partner = fixture.Build<Partner>()
                .With(p => p.Id,id)
                .With(p => p.IsActive, false)
                .Create();
            _partnersRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(partner);

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(id, request);

            result.Should().NotBeNull();

            var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequest.Value.Should().Be("Данный партнер не активен");

        }

        [Fact]
        public async Task SetPartnerPromoCodeLimit_ValidRequest_SavesNewLimitAndResetsPromoCodes()
        {
            var fixture = new Fixture();
            var id = Guid.NewGuid();

            var request = new SetPartnerPromoCodeLimitRequest
            {
                Limit = 10,
                EndDate = DateTime.UtcNow.AddDays(30)
            };

            var activeLimit = new PartnerPromoCodeLimit
            {
                Id = Guid.NewGuid(),
                Limit = 5,
                CreateDate = DateTime.UtcNow.AddDays(-10),
                PartnerId = id
            };

            var partner = new Partner
            {
                Id = id,
                IsActive = true,
                NumberIssuedPromoCodes = 15,
                PartnerLimits = new List<PartnerPromoCodeLimit> { activeLimit }
            };

            _partnersRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(partner);

            _partnersRepositoryMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<Partner>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var result = await _partnersController.SetPartnerPromoCodeLimitAsync(id, request);


            result.Should().BeOfType<CreatedAtActionResult>();


            partner.NumberIssuedPromoCodes.Should().Be(0);


            activeLimit.CancelDate.Should().NotBeNull();


            partner.PartnerLimits.Should().ContainSingle(l => l.Limit == 10);

            var newLimit = partner.PartnerLimits.First(l => l.Limit == 10);
            newLimit.CreateDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(2));
            newLimit.EndDate.Should().BeCloseTo(request.EndDate, TimeSpan.FromSeconds(1));
            newLimit.PartnerId.Should().Be(partner.Id);


            _partnersRepositoryMock.Verify(repo => repo.UpdateAsync(partner), Times.Once);
        }
    }
}