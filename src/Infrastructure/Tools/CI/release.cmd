@echo building infrastructure packages in release mode
dotnet restore ../../Riganti.Utils.Infrastructure.Core
dotnet build ../../Riganti.Utils.Infrastructure.Core -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.EntityFramework
dotnet build ../../Riganti.Utils.Infrastructure.EntityFramework -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.EntityFrameworkCore
dotnet build ../../Riganti.Utils.Infrastructure.EntityFrameworkCore -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Azure.TableStorage
dotnet build ../../Riganti.Utils.Infrastructure.Azure.TableStorage -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Configuration
dotnet build ../../Riganti.Utils.Infrastructure.Configuration -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Services
dotnet build ../../Riganti.Utils.Infrastructure.Services -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Azure
dotnet build ../../Riganti.Utils.Infrastructure.Services.Azure -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Services.SendGrid
dotnet build ../../Riganti.Utils.Infrastructure.Services.SendGrid -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Amazon.SES
dotnet build ../../Riganti.Utils.Infrastructure.Services.Amazon.SES -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Smtp
dotnet build ../../Riganti.Utils.Infrastructure.Services.Smtp -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.DotVVM
dotnet build ../../Riganti.Utils.Infrastructure.DotVVM -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.AutoMapper
dotnet build ../../Riganti.Utils.Infrastructure.AutoMapper -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.AspNetCore
dotnet build ../../Riganti.Utils.Infrastructure.AspNetCore -c Release

dotnet restore ../../Riganti.Utils.Infrastructure.SystemWeb
dotnet build ../../Riganti.Utils.Infrastructure.SystemWeb -c Release
