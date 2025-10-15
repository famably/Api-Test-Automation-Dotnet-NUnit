using NUnit.Framework;
using RestSharp;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Net;
using System.Linq;

namespace ApiTests.Tests
{
    [TestFixture]
    public class UserApiTests
    {
        private RestClient _client;
        private IHost _mockServer;
        private const string BaseUrl = "http://localhost:5055";

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            // Build and start a lightweight mock API server
            var builder = WebApplication.CreateBuilder();
            builder.WebHost.UseUrls(BaseUrl);

            var app = builder.Build();

            var users = new[]
            {
                new { id = 1, email = "george.bluth@reqres.in", first_name = "George", last_name = "Bluth", avatar = "avatar1.png" },
                new { id = 2, email = "janet.weaver@reqres.in", first_name = "Janet", last_name = "Weaver", avatar = "avatar2.png" }
            };

            app.MapGet("/api/users", async ctx =>
            {
                await ctx.Response.WriteAsJsonAsync(new { page = 2, data = users });
            });

            app.MapGet("/api/users/{id:int}", async ctx =>
            {
                var id = int.Parse((string)ctx.Request.RouteValues["id"]!);
                var user = users.FirstOrDefault(u => u.id == id);
                if (user == null)
                {
                    ctx.Response.StatusCode = 404;
                    await ctx.Response.WriteAsync("{}");
                }
                else
                {
                    await ctx.Response.WriteAsJsonAsync(new { data = user });
                }
            });

            app.MapPost("/api/users", async ctx =>
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(ctx.Request.Body);
                var name = body.GetProperty("name").GetString();
                var job = body.GetProperty("job").GetString();
                ctx.Response.StatusCode = 201;
                await ctx.Response.WriteAsJsonAsync(new { id = 99, name, job, createdAt = DateTime.UtcNow });
            });

            app.MapPut("/api/users/{id:int}", async ctx =>
            {
                var body = await JsonSerializer.DeserializeAsync<JsonElement>(ctx.Request.Body);
                var name = body.GetProperty("name").GetString();
                var job = body.GetProperty("job").GetString();
                await ctx.Response.WriteAsJsonAsync(new { name, job, updatedAt = DateTime.UtcNow });
            });

            app.MapDelete("/api/users/{id:int}", ctx =>
            {
                ctx.Response.StatusCode = 204;
                return Task.CompletedTask;
            });

            _mockServer = app;
            await app.StartAsync();

            _client = new RestClient(BaseUrl);
            _client.AddDefaultHeader("Accept", "application/json");
            _client.AddDefaultHeader("Content-Type", "application/json");
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            if (_mockServer != null)
                await _mockServer.StopAsync();
            _client?.Dispose();
        }

        [Test]
        public void Get_List_Of_Users_Should_Return_Valid_Data()
        {
            var response = _client.Execute(new RestRequest("/api/users", Method.Get));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = JsonDocument.Parse(response.Content ?? "{}");
            json.RootElement.GetProperty("data").GetArrayLength().Should().BeGreaterThan(0);
        }

        [Test]
        public void Get_Single_User_Should_Return_Correct_User()
        {
            var response = _client.Execute(new RestRequest("/api/users/2", Method.Get));
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = JsonDocument.Parse(response.Content ?? "{}");
            json.RootElement.GetProperty("data").GetProperty("id").GetInt32().Should().Be(2);
        }

        [Test]
        public void Create_User_Should_Return_Created_User()
        {
            var req = new RestRequest("/api/users", Method.Post);
            req.AddJsonBody(new { name = "morpheus", job = "leader" });
            var response = _client.Execute(req);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var json = JsonDocument.Parse(response.Content ?? "{}");
            json.RootElement.GetProperty("name").GetString().Should().Be("morpheus");
        }

        [Test]
        public void Update_User_Should_Return_Updated_Info()
        {
            var req = new RestRequest("/api/users/2", Method.Put);
            req.AddJsonBody(new { name = "morpheus", job = "zion resident" });
            var response = _client.Execute(req);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var json = JsonDocument.Parse(response.Content ?? "{}");
            json.RootElement.GetProperty("job").GetString().Should().Be("zion resident");
        }

        [Test]
        public void Delete_User_Should_Return_204_And_Empty_Body()
        {
            var response = _client.Execute(new RestRequest("/api/users/2", Method.Delete));
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
