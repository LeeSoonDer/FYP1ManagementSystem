using FYP1ManagementSystem.Models;
using FYP1ManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 🔌 注册数据库
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🔐 注册身份验证
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 👨‍🏫 MVC 控制器 + Razor 视图
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ⚠️ 中间件配置（顺序很重要！）
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // ✅ 推荐加上这行

app.UseRouting();

app.UseAuthentication(); // ✅ 你漏掉的认证环节
app.UseAuthorization();

// 路由配置
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ✅ 添加管理员初始化逻辑（放在 app.Run() 之前）
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // 创建角色（如不存在）
    string[] roles = { "Admin", "Student", "Supervisor" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 创建默认 Admin 用户
    string adminEmail = "admin@fyp.com";
    string adminPassword = "Admin123!";

    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = adminEmail,
            FullName = "System Administrator"
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

app.Run();
