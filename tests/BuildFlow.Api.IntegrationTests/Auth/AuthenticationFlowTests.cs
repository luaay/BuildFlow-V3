using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace BuildFlow.Api.IntegrationTests.Auth;

[Collection(nameof(IntegrationTestCollection))]
public class AuthenticationFlowTests
{
    private readonly IntegrationTestFactory _factory;

    public AuthenticationFlowTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FullFlow_RegisterLoginAndAccessProtected_ShouldSucceed()
    {
        var client = _factory.CreateClient();

        // 1) التجهيز: سجّل مستأجراً جديداً
        var registerRequest = new
        {
            tenantName = "Auth Flow Co",
            slug = "auth-flow-co",
            plan = 1,
            ownerEmail = "owner@authflow.com",
            ownerPassword = "P@ssw0rd123",
            ownerFullName = "Auth Owner"
        };
        var registerResponse = await client.PostAsJsonAsync(
            "/api/tenants/register", registerRequest);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // 2) الفعل: سجّل الدخول وخذ الرمز
        var loginRequest = new
        {
            slug = "auth-flow-co",
            email = "owner@authflow.com",
            password = "P@ssw0rd123"
        };
        var loginResponse = await client.PostAsJsonAsync(
            "/api/auth/login", loginRequest);

        // التوكيد: الدخول نجح وأرجع رمزاً
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
       
       var loginResult = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponseDto>();
        loginResult.Should().NotBeNull();
        var token = loginResult!.AccessToken;
        token.Should().NotBeNullOrWhiteSpace();

        // 3) استعمل الرمز في طلب محميّ
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        var protectedResponse = await client.GetAsync("/api/projects");

         // مؤقّت: اطبع جسم الخطأ لتشخيصه
        // اقرأ جسم الاستجابة، وضُمّه في رسالة الفشل إن فشل التوكيد
        var errorBody = await protectedResponse.Content.ReadAsStringAsync();

        protectedResponse.StatusCode.Should().Be(
            HttpStatusCode.OK,
            because: $"but the server returned: {errorBody}");


        // التوكيد النهائيّ: الطلب المحميّ نجح بالرمز
        protectedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // نوع يطابق حقل الرمز من استجابة الدخول
    private sealed record LoginResponseDto(string AccessToken);
}