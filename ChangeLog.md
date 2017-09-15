# Change Log

## vNext (2.1.0)
- Namespaces refactoring ([PR #26](https://github.com/riganti/infrastructure/pull/26))
  - *Riganti.Utils.Infrastructure.Services.Smtp.Mailing* namespace has been removed, all types moved to *Riganti.Utils.Infrastructure.Services.Mailing* namespace.
  - *Riganti.Utils.Infrastructure.Services.Smtp.Mailing* NuGet no longer exists.
  - All *\*.Services.Logging.\** namespaces renamed to *\*.Logging.\** (*Service* part was removed).
- Logging extended ([edeab27](https://github.com/riganti/infrastructure/commit/edeab27e25e669ebc8588d13a1c35f599f3e878a))
  - Added methods to [`ILogger`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/ILogger.cs)
  interface and [`LoggerBase`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/LoggerBase.cs)
  class: `LogVerbose(string message)`, `LogInfo(string message)`, etc.
  - Added [`ConsoleLogger`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/ConsoleLogger.cs).
  - Added [`EmptyLogger`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/EmptyLogger.cs).
  - Added `IMessageFormatter` and default [`DefaultLogFormatter`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultLogFormatter.cs).
  - Modified [`DefaultExceptionFormatter`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DefaultExceptionFormatter.cs):
    - Added displaying of `Data` property.
    - Added displaying of reflected properties (with possibility to modify `IgnoredReflectionProperties` collection).
    - Added `ExceptionAdapters` property as collection of `IExceptionAdapter` with default [`DbEntityValidationExceptionAdapter`](https://github.com/riganti/infrastructure/blob/edeab27e25e669ebc8588d13a1c35f599f3e878a/src/Infrastructure/Riganti.Utils.Infrastructure.Logging/DbEntityValidationExceptionAdapter.cs), that allows to format specific exception.

## 2.0.12
- Fixed `EntityFrameworkRepository.Delete(TKey id)` method - attach fake entity only when not found in local context. ([PR #24](https://github.com/riganti/infrastructure/pull/24))
- `ISortableQuery<TQueryableResult>` interface moved from `QueryBase` to `IQuery`. ([PR #25](https://github.com/riganti/infrastructure/pull/25))

## 2.0.11
- Fixed AutoMapper extension methods for Entity Framework. ([39e0dac](https://github.com/riganti/infrastructure/commit/39e0dac10ee5a3317eb84124302dae801b0b1227))

## 2.0.10
- Added CRUD facade permitions validation. ([ba46bd6](https://github.com/riganti/infrastructure/commit/ba46bd6eeef08df2c322539da701287bcb905748))

## 2.0.9
- Add mapping from DTO to Entity to `EntityDTOMapper`. ([PR #22](https://github.com/riganti/infrastructure/pull/22))