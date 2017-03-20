@echo building infrastructure packages
dotnet restore ../../Riganti.Utils.Infrastructure.Core
dotnet build ../../Riganti.Utils.Infrastructure.Core

dotnet restore ../../Riganti.Utils.Infrastructure.EntityFramework
dotnet build ../../Riganti.Utils.Infrastructure.EntityFramework

dotnet restore ../../Riganti.Utils.Infrastructure.EntityFrameworkCore
dotnet build ../../Riganti.Utils.Infrastructure.EntityFrameworkCore

dotnet restore ../../Riganti.Utils.Infrastructure.Azure.TableStorage
dotnet build ../../Riganti.Utils.Infrastructure.Azure.TableStorage

dotnet restore ../../Riganti.Utils.Infrastructure.Configuration
dotnet build ../../Riganti.Utils.Infrastructure.Configuration

dotnet restore ../../Riganti.Utils.Infrastructure.Services
dotnet build ../../Riganti.Utils.Infrastructure.Services

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Azure
dotnet build ../../Riganti.Utils.Infrastructure.Services.Azure

dotnet restore ../../Riganti.Utils.Infrastructure.Services.SendGrid
dotnet build ../../Riganti.Utils.Infrastructure.Services.SendGrid

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Amazon.SES
dotnet build ../../Riganti.Utils.Infrastructure.Services.Amazon.SES

dotnet restore ../../Riganti.Utils.Infrastructure.Services.Smtp
dotnet build ../../Riganti.Utils.Infrastructure.Services.Smtp

dotnet restore ../../Riganti.Utils.Infrastructure.DotVVM
dotnet build ../../Riganti.Utils.Infrastructure.DotVVM

dotnet restore ../../Riganti.Utils.Infrastructure.AutoMapper
dotnet build ../../Riganti.Utils.Infrastructure.AutoMapper

dotnet restore ../../Riganti.Utils.Infrastructure.AspNetCore
dotnet build ../../Riganti.Utils.Infrastructure.AspNetCore

dotnet restore ../../Riganti.Utils.Infrastructure.SystemWeb
dotnet build ../../Riganti.Utils.Infrastructure.SystemWeb

