using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DalSoft.RestClient.Examples.Test.System.Models;
using DalSoft.RestClient.Examples.Test.System.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace DalSoft.RestClient.Examples.Test.System
{
    public class JsonPlaceHolderTests
    {
        private static readonly RestClient Client = new RestClient("https://jsonplaceholder.typicode.com", new Config()
            .SetJsonSerializerSettings(new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()}));

        [Fact]
        public async Task Get_UserWithId1_ReturnsOkStatusCode()
        {
            
            await Client
                .Resource<UserResource>(api => api.GetUser(1)).Get()
                .Verify<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_UserWithId1_ReturnsJsonStringWithUsernameBret()
        {
            
            await Client
                .Resource<UserResource>(api => api.GetUser(1)).Get()
                .Verify<string>(s => s.Contains("\"username\": \"Bret\""));
        }

        [Fact]
        public async Task Get_UserWithId1_ReturnsUserModelWithUsernameBret()
        {
            
            await Client
                .Resource<UserResource>(api => api.GetUser(1)).Get()
                .Verify<User>(user => user.Username == "Bret");
        }

        [Fact]
        public async Task Get_UserWithId1_ReturnsUserWithUsernameBretAndOkStatusCode()
        {
            
            await Client
                .Resource<UserResource>(api => api.GetUser(1)).Get()
                .Verify<User>(user => user.Username == "Bret")
                .Verify<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_UserWithId1_ReturnsDynamicWithUsernameBretAndOkStatusCode()
        {
            
            await Client
                .Resource("users/1").Get()
                .Verify(userIsBret => userIsBret.username == "Bret")
                .Verify(httpResponseMessageIsOk => httpResponseMessageIsOk.HttpResponseMessage.StatusCode == HttpStatusCode.OK)
                .Verify<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_UserWithId1Example_TestingUsingOnVerifyFailed()
        {
            var thrownException = await Assert.ThrowsAsync<AggregateException>(async () =>
            {
                await Client
                    .Resource<UserResource>(api => api.GetUser(1)).Get()

                    .Verify<User>(user => user.Username == "Fail")

                    .OnVerifyFailed<HttpResponseMessage>((aggregateException, response) =>
                    {   // Callback invoked if the Verify above fails
                        Assert.Equal(1, aggregateException.InnerExceptions.Count);
                    }, throwOnVerifyFailed: false)

                    .Verify<HttpResponseMessage>(response => response.StatusCode == HttpStatusCode.BadRequest)

                    .OnVerifyFailed<HttpResponseMessage>((aggregateException, response) =>
                    {
                        // Because the previous OnVerifyFailed set throwOnVerifyFailed to false callback only invoked if above Verify failure
                        Assert.Equal(1, aggregateException.InnerExceptions.Count);
                    }, throwOnVerifyFailed: true)
                    .Verify<User>(response => response.Username == "Peter")

                    .OnVerifyFailed<User>((aggregateException, response) =>
                    {
                        // Because the previous OnVerifyFailed set throwOnVerifyFailed to true callback invoked if the above Verify and previous Verify fails
                        // aggregateException contains above Verify and previous Verify failures
                        Assert.Equal(2, aggregateException.InnerExceptions.Count);
                    }, throwOnVerifyFailed: true);
                    
                    // Last OnVerifyFailed set throwOnVerifyFailed to true so an exception will be thrown 
                    // this will contain both the above and previous Verify failures, but not the first failure as throwOnVerifyFailed was set to false 
            });

            Assert.Equal(2, thrownException.InnerExceptions.Count);
        }
    }
}
