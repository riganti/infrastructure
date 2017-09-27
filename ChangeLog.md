# Change Log

## vNext (2.1.2)
- Fixed default timestamp format in [DefaultMessageFormatter](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultMessageFormatter.cs)
  ([1ebe6d6](https://github.com/riganti/infrastructure/commit/1ebe6d60163700a9d91afae1dbb4f18502727443)).
- Fix - directory is created if not exists in [PickupFolderMailSender](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Services.Mailing/PickupFolderMailSender.cs)
  ([PR #28](https://github.com/riganti/infrastructure/pull/28)).
- Added optional parametr keepRemovedItemsInDestinationCollection(default value is true) for method SyncCollectionByKey([a55350b](https://github.com/riganti/infrastructure/commit/a55350be61064c0dada06b71bb7965ea541d7371)).

## 2.1.1
- Refactored Message Formater in logging - removed from
  [ConsoleLogger](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/ConsoleLogger.cs)
  and [TextFileLogger](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/TextFileLogger.cs)
  and added to [LoggerBase](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/LoggerBase.cs).
  Default value set to [DefaultMessageFormatter](https://github.com/riganti/infrastructure/blob/master/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultMessageFormatter.cs)
  ([8578ef4](https://github.com/riganti/infrastructure/commit/8578ef4068c4ecf4b20b25b74c0b74062961e6d7)).

## 2.1.0
- Namespaces refactoring ([PR #26](https://github.com/riganti/infrastructure/pull/26))
  - *Riganti.Utils.Infrastructure.Services.Smtp.Mailing* namespace has been removed, all types moved to *Riganti.Utils.Infrastructure.Services.Mailing* namespace.
  - All *\*.Services.Logging.\** namespaces renamed to *\*.Logging.\** (*Service* part was removed).
- Logging extended ([edeab27](https://github.com/riganti/infrastructure/commit/edeab27e25e669ebc8588d13a1c35f599f3e878a))
  - Added methods to [ILogger](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/ILogger.cs)
  interface and [LoggerBase](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/LoggerBase.cs)
  class: `LogVerbose(string message)`, `LogInfo(string message)`, etc.
  - Added [ConsoleLogger](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/ConsoleLogger.cs).
  - Added [EmptyLogger](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/EmptyLogger.cs).
  - Added `IMessageFormatter` and default [DefaultLogFormatter](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultLogFormatter.cs).
  - Modified [DefaultExceptionFormatter](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultExceptionFormatter.cs):
    - Added displaying of `Data` property.
    - Added displaying of reflected properties (with possibility to modify `IgnoredReflectionProperties` collection).
    - Added `ExceptionAdapters` property as collection of `IExceptionAdapter` with default [DbEntityValidationExceptionAdapter](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DbEntityValidationExceptionAdapter.cs), that allows to format specific exception.
- [ILogger](https://github.com/riganti/infrastructure/blob/c4567ad51b1b7b095399dec5c6d9e8a65273934b/src/Infrastructure/Riganti.Utils.Infrastructure.Core/Logging/ILogger.cs)
  and [Severity](https://github.com/riganti/infrastructure/blob/c4567ad51b1b7b095399dec5c6d9e8a65273934b/src/Infrastructure/Riganti.Utils.Infrastructure.Core/Logging/Severity.cs)
  moved to *Riganti.Utils.Infrastructure.Core*.
- Added [XUnitOutputLogger](https://github.com/riganti/infrastructure/blob/e612747531e83efd622dbd7389c5ed83ecaa53d1/src/Infrastructure/Riganti.Utils.Infrastructure.Testing/Riganti.Utils.Infrastructure.Testing.csproj)
  and NuGet package *Riganti.Utils.Infrastructure.Testing*.

#### These NuGet packages no longer exist:
- *Riganti.Utils.Infrastructure.Services.Logging*
- *Riganti.Utils.Infrastructure.Services.Logging.ApplicationInsights*
- *Riganti.Utils.Infrastructure.Services.Logging.Email*
- *Riganti.Utils.Infrastructure.Services.Smtp*

## 2.0.12
- Fixed `EntityFrameworkRepository.Delete(TKey id)` method - attach fake entity only when not found in local context. ([PR #24](https://github.com/riganti/infrastructure/pull/24))
- `ISortableQuery<TQueryableResult>` interface moved from `QueryBase` to `IQuery`. ([PR #25](https://github.com/riganti/infrastructure/pull/25))

## 2.0.11
- Fixed AutoMapper extension methods for Entity Framework. ([39e0dac](https://github.com/riganti/infrastructure/commit/39e0dac10ee5a3317eb84124302dae801b0b1227))

## 2.0.10
- Added CRUD facade permitions validation. ([ba46bd6](https://github.com/riganti/infrastructure/commit/ba46bd6eeef08df2c322539da701287bcb905748))

## 2.0.9
- Add mapping from DTO to Entity to `EntityDTOMapper`. ([PR #22](https://github.com/riganti/infrastructure/pull/22))