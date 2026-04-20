using DogtoraliaMVC.Controllers;
using DogtoraliaMVC.Data;
using DogtoraliaMVC.Services;
using DogtoraliaMVC.Tests.Helpers;
using DogtoraliaMVC.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace DogtoraliaMVC.Tests.Controllers;

public class AccountControllerTests
{
    private static DogtoraliaDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<DogtoraliaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DogtoraliaDbContext(options);
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static Mock<SignInManager<IdentityUser>> CreateMockSignInManager(
        Mock<UserManager<IdentityUser>> mockUm)
    {
        return new Mock<SignInManager<IdentityUser>>(
            mockUm.Object,
            new Mock<IHttpContextAccessor>().Object,
            new Mock<IUserClaimsPrincipalFactory<IdentityUser>>().Object,
            null, null, null, null);
    }

    private static (AccountController, Mock<UserManager<IdentityUser>>, Mock<SignInManager<IdentityUser>>)
        CreateController(DogtoraliaDbContext ctx)
    {
        var mockUm = ControllerTestHelpers.CreateMockUserManager();
        var mockSm = CreateMockSignInManager(mockUm);
        var mockEmail = new Mock<IEmailService>();
        mockEmail.Setup(e => e.SendWelcomeEmailAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        var mockLogger = new Mock<ILogger<AccountController>>();
        var controller = new AccountController(mockUm.Object, mockSm.Object, ctx, mockEmail.Object, mockLogger.Object);
        // Anonymous user context (not authenticated)
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return (controller, mockUm, mockSm);
    }

    // ── Login GET ─────────────────────────────────────────────────────────────

    [Fact]
    public void Login_Get_ReturnsView()
    {
        using var ctx = CreateContext();
        var (controller, _, _) = CreateController(ctx);

        var result = controller.Login((string?)null);

        Assert.IsType<ViewResult>(result);
    }

    // ── Login POST ────────────────────────────────────────────────────────────

    [Fact]
    public async Task Login_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var (controller, _, _) = CreateController(ctx);
        controller.ModelState.AddModelError("Email", "Required");

        var result = await controller.Login(new LoginViewModel());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Login_Post_ValidCredentials_RedirectsToHome()
    {
        using var ctx = CreateContext();
        var (controller, mockUm, mockSm) = CreateController(ctx);

        mockSm.Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        var vm = new LoginViewModel { Email = "admin@dogtoralia.com", Password = "Admin" };
        var result = await controller.Login(vm) as RedirectToActionResult;

        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Home", result.ControllerName);
    }

    [Fact]
    public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
    {
        using var ctx = CreateContext();
        var (controller, _, mockSm) = CreateController(ctx);

        mockSm.Setup(s => s.PasswordSignInAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        var vm = new LoginViewModel { Email = "bad@test.com", Password = "wrong" };
        var result = await controller.Login(vm) as ViewResult;

        Assert.NotNull(result);
        Assert.False(controller.ModelState.IsValid);
    }

    // ── Register GET ──────────────────────────────────────────────────────────

    [Fact]
    public void Register_Get_ReturnsView()
    {
        using var ctx = CreateContext();
        var (controller, _, _) = CreateController(ctx);

        var result = controller.Register();

        Assert.IsType<ViewResult>(result);
    }

    // ── Register POST ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Register_Post_InvalidModel_ReturnsView()
    {
        using var ctx = CreateContext();
        var (controller, _, _) = CreateController(ctx);
        controller.ModelState.AddModelError("Name", "Required");

        var result = await controller.Register(new RegisterViewModel());

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Register_Post_DuplicateEmail_ReturnsViewWithError()
    {
        using var ctx = CreateContext();
        var (controller, _, _) = CreateController(ctx);

        var vm = new RegisterViewModel
        {
            Name = "Jorge",
            Email = "jorge.sanchez@gmail.com", // already in seed
            Phone = "+52-55-0000-0001",
            Password = "pass",
            ConfirmPassword = "pass"
        };

        var result = await controller.Register(vm) as ViewResult;

        Assert.NotNull(result);
        Assert.False(controller.ModelState.IsValid);
    }

    [Fact]
    public async Task Register_Post_ValidModel_CreatesOwnerAndRedirects()
    {
        using var ctx = CreateContext();
        var (controller, mockUm, mockSm) = CreateController(ctx);

        var newUser = new IdentityUser { Id = "new-user-id", UserName = "new@test.com", Email = "new@test.com" };

        mockUm.Setup(u => u.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .Callback<IdentityUser, string>((u, _) => u.Id = newUser.Id)
            .ReturnsAsync(IdentityResult.Success);

        mockUm.Setup(u => u.AddToRoleAsync(It.IsAny<IdentityUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        mockSm.Setup(s => s.SignInAsync(It.IsAny<IdentityUser>(), It.IsAny<bool>(), null))
            .Returns(Task.CompletedTask);

        var vm = new RegisterViewModel
        {
            Name = "New User",
            Email = "new@test.com",
            Phone = "+52-55-9999-0000",
            Password = "pass",
            ConfirmPassword = "pass"
        };

        var result = await controller.Register(vm) as RedirectToActionResult;

        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Home", result.ControllerName);
        Assert.Equal(11, ctx.PetOwners.Count());
    }

    // ── Logout ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Logout_Post_SignsOutAndRedirects()
    {
        using var ctx = CreateContext();
        var (controller, _, mockSm) = CreateController(ctx);
        mockSm.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        var result = await controller.Logout() as RedirectToActionResult;

        mockSm.Verify(s => s.SignOutAsync(), Times.Once);
        Assert.Equal("Index", result!.ActionName);
        Assert.Equal("Home", result.ControllerName);
    }
}
