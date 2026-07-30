using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SIMS.Application.Interfaces.Repositories;
using SIMS.Application.Interfaces.Services;
using SIMS.Application.Services;
using SIMS.Application.Settings;
using SIMS.Infrastructure.Persistence;
using SIMS.Infrastructure.Security;
using SIMS.Infrastructure.Settings;

namespace SIMS.Infrastructure;

/// <summary>
/// Registers all Infrastructure and Application services into the DI container.
/// Call builder.Services.AddInfrastructure(builder.Configuration, builder.Environment.ContentRootPath) in Program.cs.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string contentRootPath)
    {
        // ── Settings ──────────────────────────────────────────────────── //
        // Bind from config first, then resolve any relative BasePath against
        // contentRootPath (the project directory) rather than AppContext.BaseDirectory
        // (the build output directory) so CSV files land in SIMS-BackEnd/Data/,
        // not bin/Debug/net8.0/Data/.
        services.Configure<DataStoreSettings>(settings =>
        {
            configuration.GetSection(DataStoreSettings.SectionName).Bind(settings);
            if (!Path.IsPathRooted(settings.BasePath))
                settings.BasePath = Path.Combine(contentRootPath, settings.BasePath);
        });

        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // Application-layer view of the same "Jwt" section — AuthService needs the
        // refresh window but cannot reference Infrastructure's JwtSettings.
        services.Configure<TokenPolicy>(
            configuration.GetSection(JwtSettings.SectionName));

        // ── Persistence ────────────────────────────────────────────────── //
        // Concrete registration first (PermissionRepository injects the type directly),
        // then the interface aliased onto that same instance so role_permissions.csv is
        // guarded by exactly one semaphore — see CsvRepositoryBase.
        services.AddSingleton<RolePermissionRepository>();
        services.AddSingleton<IRolePermissionRepository>(
            sp => sp.GetRequiredService<RolePermissionRepository>());
        services.AddSingleton<IUserRepository,          UserRepository>();
        services.AddSingleton<IRoleRepository,          RoleRepository>();
        services.AddSingleton<IPermissionRepository,    PermissionRepository>();
        services.AddSingleton<IRevokedTokenRepository,  RevokedTokenRepository>();
        services.AddSingleton<IStudentRepository,       StudentRepository>();
        services.AddSingleton<IInstructorRepository,    InstructorRepository>();
        services.AddSingleton<ICourseRepository,        CourseRepository>();
        services.AddSingleton<ISubjectRepository,       SubjectRepository>();
        services.AddSingleton<IClassRepository,         ClassRepository>();
        services.AddSingleton<IEnrollmentRepository,    EnrollmentRepository>();
        services.AddSingleton<IGradeRepository,         GradeRepository>();

        // ── Security ────────────────────────────────────────────────────── //
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenService,   JwtTokenService>();

        // ── Application services ─────────────────────────────────────────── //
        services.AddScoped<IAuthService,       AuthService>();
        services.AddScoped<IUserService,       UserService>();
        services.AddScoped<IStudentService,    StudentService>();
        services.AddScoped<IInstructorService, InstructorService>();
        services.AddScoped<ICourseService,     CourseService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ISubjectService,    SubjectService>();
        services.AddScoped<IClassService,      ClassService>();
        services.AddScoped<IGradeService,      GradeService>();

        return services;
    }
}
