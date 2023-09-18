using Microsoft.EntityFrameworkCore;
using MvcApp.Models;
using Tests.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<Aplicationdb>(options => options.UseSqlServer(connection));

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Auth/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    });
builder.Services.AddAuthorization();

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<SubjectCreateService>();

builder.Services.AddDbContext<Aplicationdb>(options => options.UseSqlServer(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "Login",
    pattern: "SignIn", // Измените значение на желаемое
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "ManageUsers",
    pattern: "Manage",
    defaults: new { controller = "Auth", action = "ManageUsers" });

app.MapControllerRoute(
    name: "CreateUserAdmin",
    pattern: "CreateUserAdmin",
    defaults: new { controller = "Auth", action = "CreateUserAdmin" });

app.MapControllerRoute(
    name: "CreateTest",
    pattern: "CreateTest",
    defaults: new { controller = "TestCreate", action = "Create" });

app.MapControllerRoute(
    name: "CreateQuestion",
    pattern: "CreateQuestion/{testId}",
    defaults: new { controller = "TestCreate", action = "CreateQuestion" });


app.MapControllerRoute(
    name: "deleteConfirmed",
    pattern: "delete-confirmed/{testId}",
    defaults: new { controller = "TestCreate", action = "DeleteConfirmed" });

app.MapControllerRoute(
    name: "Subjects",
    pattern: "Subjects",
    defaults: new { controller = "Testing", action = "Subjects" });

app.MapControllerRoute(
    name: "Tests",
    pattern: "Tests/{subjectId:int}",
    defaults: new { controller = "Testing", action = "Tests" });


app.MapControllerRoute(
    name: "StartTest",
    pattern: "StartTest/{testId:int}",
    defaults: new { controller = "Testing", action = "StartTest" });

app.MapControllerRoute(
    name: "Testing",
    pattern: "Testing/{testId:int}/{fullName}/{uniqueTestId}", // Добавление параметра uniqueTestId в URL
    defaults: new { controller = "Testing", action = "Testing" });

app.MapControllerRoute(
    name: "Submit",
    pattern: "Submit",
    defaults: new { controller = "Testing", action = "Submit" });

app.MapControllerRoute(
     name: "Create-Subject",
        pattern: "CreateSubject",
        defaults: new { controller = "TestCreate", action = "CreateSubject" });
app.MapControllerRoute(
    name: "editTest",
    pattern: "edit-test/{testId}",
    defaults: new { controller = "TestCreate", action = "Edit" });

app.MapControllerRoute(
    name: "EditQuestionAnswers",
    pattern: "EditQuestionAnswers/{questionId}/{answerId}",
    defaults: new { controller = "TestCreate", action = "EditAnswer" });

app.MapControllerRoute(
    name: "EditQuestion",
    pattern: "TestCreate/EditQuestion/{testId}/{questionId}",
    defaults: new { controller = "TestCreate", action = "EditQuestion" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "deleteSubject",
    pattern: "delete-subject/{subjectId}",
    defaults: new { controller = "TestCreate", action = "DeleteSubject" });
app.MapControllerRoute(
    name: "DeleteAnswer",
    pattern: "delete-subject/{subjectId}",
    defaults: new { controller = "TestCreate", action = "DeleteAnswer" });

app.Run();