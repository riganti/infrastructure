# Change Log

## vNext (2.1.0)

- Namespaces refactoring ([PR #26](https://github.com/riganti/infrastructure/pull/26))
  - *Riganti.Utils.Infrastructure.Services.Smtp.Mailing* namespace has been removed, all types moved to *Riganti.Utils.Infrastructure.Services.Mailing* namespace.
  - *Riganti.Utils.Infrastructure.Services.Smtp.Mailing* NuGet no longer exists.
  - All *\*.Services.Logging.\** namespaces renamed to *\*.Logging.\** (*Service* part was removed).

## 2.0.12
- ISortableQuery&lt;TQueryableResult&gt; interface moved from QueryBase to IQuery. ([PR #25](https://github.com/riganti/infrastructure/pull/25))
- Fixed `EntityFrameworkRepository.Delete(TKey id)` method - attach fake entity only when not found in local context. ([PR #24](https://github.com/riganti/infrastructure/pull/24))

## 2.0.11
- Fixed AutoMapper extension methods for Entity Framework. ([39e0dac](https://github.com/riganti/infrastructure/commit/39e0dac10ee5a3317eb84124302dae801b0b1227))

## 2.0.10
- Added CRUD facade permitions validation. ([ba46bd6](https://github.com/riganti/infrastructure/commit/ba46bd6eeef08df2c322539da701287bcb905748))

## 2.0.9
- Add mapping from DTO to Entity to `EntityDTOMapper`. ([PR #22](https://github.com/riganti/infrastructure/pull/22))