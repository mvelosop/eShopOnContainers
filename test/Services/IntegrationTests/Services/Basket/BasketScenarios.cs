using IntegrationTests.Services.Extensions;
using Microsoft.eShopOnContainers.Services.Basket.API.Model;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebMVC.Models;
using Xunit;

namespace IntegrationTests.Services.Basket
{
    public class BasketScenarios
        : BasketScenarioBase
    {
        [Fact]
        public async Task Post_basket_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var content = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateClient()
                   .PostAsync(Post.Basket, content);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Get_basket_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var response = await server.CreateClient()
                   .GetAsync(Get.GetBasket(1));

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Send_Checkout_basket_and_response_ok_status_code()
        {
            using (var server = CreateServer())
            {
                var contentBasket = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
                await server.CreateClient()
                   .PostAsync(Post.Basket, contentBasket);

                var contentCheckout = new StringContent(BuildCheckout(), UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateIdempotentClient()
                   .PostAsync(Post.CheckoutOrder, contentCheckout);

                response.EnsureSuccessStatusCode();
            }
        }

        [Fact]
        public async Task Log_Validation_Errors_When_Invalid_Data()
        {
            //----------------------------------------------------------------------------------------------------------
            // This test creates an invalid checkout basket but the test should complete because:
            // the POSTING of the basket DTO to Basket.API -> BasketController.Checkout 
            // should complete without problems if the basket.api container is running.
            // BUT..
            // the validation exception will be raised in a while, because:
            // the controller creates an UserCheckoutAcceptedIntegrationEvent...
            // that gets queued in RabbitMQ and eventually...
            // gets handled by the UserCheckoutAcceptedIntegrationEventHandler
            // where it's converted to a CreateOrderCommand and 
            // THEN MediatR's validation behavior is the one that raises the ValidationException
            //
            // So to see the failure, you have to run ALL integration tests so the processing of the CreateOrderCommand
            // occurs before the test run finishes and it aborts because of the ValidationException
            //----------------------------------------------------------------------------------------------------------

            using (var server = CreateServer())
            {
                var contentBasket = new StringContent(BuildBasket(), UTF8Encoding.UTF8, "application/json");
                await server.CreateClient()
                    .PostAsync(Post.Basket, contentBasket);

                var invalidBasket = BuildCheckout(b =>
                {
                    b.CardNumber = "123456";
                    b.CardSecurityNumber = "1234";
                });

                var contentCheckout = new StringContent(invalidBasket, UTF8Encoding.UTF8, "application/json");
                var response = await server.CreateIdempotentClient()
                    .PostAsync(Post.CheckoutOrder, contentCheckout);

                response.EnsureSuccessStatusCode();
            }
        }

        string BuildBasket(Action<CustomerBasket> makeInvalidaAction = null)
        {
            var order = new CustomerBasket("1234");

            order.Items.Add(new BasketItem
            {
                ProductId = "1",
                ProductName = ".NET Bot Black Hoodie",
                UnitPrice = 10,
                Quantity = 1
            });

            makeInvalidaAction?.Invoke(order);

            return JsonConvert.SerializeObject(order);
        }

        string BuildCheckout(Action<BasketDTO> makeInvalidaAction = null)
        {
            var checkoutBasket = new BasketDTO()
            {
                City = "city",
                Street = "street",
                State = "state",
                Country = "coutry",
                ZipCode = "zipcode",
                CardNumber = "1234567890123456",
                CardHolderName = "CardHolderName",
                CardExpiration = DateTime.UtcNow.AddDays(1),
                CardSecurityNumber = "123",
                CardTypeId = 1,
                Buyer = "Buyer",
                RequestId = Guid.NewGuid()
            };

            makeInvalidaAction?.Invoke(checkoutBasket);

            return JsonConvert.SerializeObject(checkoutBasket);
        }
    }
}
