using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using Riganti.Utils.Infrastructure.Core;
using Xunit;

namespace Riganti.Utils.Infrastructure.EntityFramework.Tests.Repository
{
  public class EntityFrameworkRepositoryTests
  {
    private static readonly EpisodeEntity[] episodesSeriesOne =
    {
            new EpisodeEntity {Id = 1, Series = 1, Title = "Open Government"},
            new EpisodeEntity {Id = 2, Series = 1, Title = "The Official Visit"},
            new EpisodeEntity {Id = 3, Series = 1, Title = "The Economy Drive"},
            new EpisodeEntity {Id = 4, Series = 1, Title = "Big Brother"},
            new EpisodeEntity {Id = 5, Series = 1, Title = "The Writing on the Wall"},
            new EpisodeEntity {Id = 6, Series = 1, Title = "The Right to Know"},
            new EpisodeEntity {Id = 7, Series = 1, Title = "Jobs for the Boys"}
    };

    private static readonly QuoteEntity[] quotes =
    {
      new QuoteEntity {Id = 1,  Text = " It is not for a humble mortal such as I to speculate on the complex and elevated deliberations of the mighty."},
    };

    private readonly Mock<YesMinisterDbContext> dbContextMock;
    private readonly EntityFrameworkRepository<EpisodeEntity, int> episodeRepositorySUT;
    private readonly EntityFrameworkRepository<QuoteEntity, int> quoteRepositorySUT;
    private readonly Mock<IDbSet<EpisodeEntity>> episodesDbSetMock;
    private readonly Mock<IDbSet<QuoteEntity>> quotesDbSetMock;

    public EntityFrameworkRepositoryTests()
    {
      var dbContextMockFactory = new DbContextMockFactory();
      dbContextMock = dbContextMockFactory.CreateDbContextMock<YesMinisterDbContext>();
      episodesDbSetMock = dbContextMock.SetupDbSet<YesMinisterDbContext, EpisodeEntity, int>(episodesSeriesOne, context => context.Episodes);
      quotesDbSetMock = dbContextMock.SetupDbSet<YesMinisterDbContext, QuoteEntity, int>(quotes, context => context.Quotes);

      episodeRepositorySUT = CreateEntityFrameworkRepository<EpisodeEntity>();
      quoteRepositorySUT = CreateEntityFrameworkRepository<QuoteEntity>();
    }

    [Fact]
    public void InitializeNew_ReturnsNotNullItem()
    {
      var newEpisode = episodeRepositorySUT.InitializeNew();

      Assert.NotNull(newEpisode);
    }

    [Fact]
    public void Insert_OneItem_CallDbSetAddMethod()
    {
      var newEpisode = new EpisodeEntity { Id = 10, Title = "Inserted item" };

      episodeRepositorySUT.Insert(newEpisode);

      episodesDbSetMock.Verify(set => set.Add(newEpisode), Times.Once);
    }

    [Fact]
    public void Insert_MultipleItems_NewItemShouldBeInDbSetLocal()
    {
      var newEpisode1 = new EpisodeEntity { Id = 10, Title = "Inserted item 1" };
      var newEpisode2 = new EpisodeEntity { Id = 11, Title = "Inserted item 2" };
      var newEpisode3 = new EpisodeEntity { Id = 12, Title = "Inserted item 3" };

      var newEpisodes = new[] { newEpisode1, newEpisode2, newEpisode3 };
      episodeRepositorySUT.Insert(newEpisodes);

      Assert.Contains(newEpisode1, episodesDbSetMock.Object.Local);
      Assert.Contains(newEpisode2, episodesDbSetMock.Object.Local);
      Assert.Contains(newEpisode3, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void Delete_OneItem_SetEntityStateToModified()
    {
      var deletedEpisode = episodesSeriesOne[1];

      episodeRepositorySUT.Delete(deletedEpisode);


      Assert.DoesNotContain(deletedEpisode, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void Delete_MultipleItems_SetEntityStateToModified()
    {
      var deletedEpisode1 = episodesSeriesOne[1];
      var deletedEpisode2 = episodesSeriesOne[2];
      var deletedEpisode3 = episodesSeriesOne[3];
      var deletedEpisodes = new[] { deletedEpisode1, deletedEpisode2, deletedEpisode3 };

      episodeRepositorySUT.Delete(deletedEpisodes);

      Assert.DoesNotContain(deletedEpisode1, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode2, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode3, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void DeleteById_OneItem_ShouldDeleteItemFromDbSetLocal()
    {
      var deletedEpisode = episodesSeriesOne[1];

      episodeRepositorySUT.Delete(deletedEpisode.Id);

      Assert.DoesNotContain(deletedEpisode, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void DeleteById_OneItem_ShouldCallAttach()
    {
      var deletedEpisode = episodesSeriesOne[1];

      episodeRepositorySUT.Delete(deletedEpisode.Id);

      episodesDbSetMock.Verify(set => set.Attach(It.Is<EpisodeEntity>(entity => entity.Id == deletedEpisode.Id)), Times.Once);
    }

    [Fact]
    public void DeleteById_MultipleItems_SetEntityStateToModified()
    {
      var deletedEpisode1 = episodesSeriesOne[1];
      var deletedEpisode2 = episodesSeriesOne[2];
      var deletedEpisode3 = episodesSeriesOne[3];
      var deletedEpisodesIds = new[] { deletedEpisode1.Id, deletedEpisode2.Id, deletedEpisode3.Id };

      episodeRepositorySUT.Delete(deletedEpisodesIds);

      Assert.DoesNotContain(deletedEpisode1, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode2, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode3, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void SoftDelete_OneItem_SetDeletedDate()
    {
      var deletedQuote = quotes[0];

      quoteRepositorySUT.Delete(deletedQuote);
      var quoteById = quoteRepositorySUT.GetById(deletedQuote.Id);


      Assert.NotNull(quoteById.DeletedDate);
    }

    [Fact]
    public void SoftDelete_MultipleItems_SetEntityStateToModified()
    {
      var deletedEpisode1 = episodesSeriesOne[1];
      var deletedEpisode2 = episodesSeriesOne[2];
      var deletedEpisode3 = episodesSeriesOne[3];
      var deletedEpisodes = new[] { deletedEpisode1, deletedEpisode2, deletedEpisode3 };

      episodeRepositorySUT.Delete(deletedEpisodes);

      Assert.DoesNotContain(deletedEpisode1, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode2, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode3, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void SoftDeleteById_OneItem_ShouldDeleteItemFromDbSetLocal()
    {
      var deletedEpisode = episodesSeriesOne[1];

      episodeRepositorySUT.Delete(deletedEpisode.Id);

      Assert.DoesNotContain(deletedEpisode, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void SoftDeleteById_OneItem_ShouldCallAttach()
    {
      var deletedEpisode = episodesSeriesOne[1];

      episodeRepositorySUT.Delete(deletedEpisode.Id);

      episodesDbSetMock.Verify(set => set.Attach(It.Is<EpisodeEntity>(entity => entity.Id == deletedEpisode.Id)), Times.Once);
    }

    [Fact]
    public void SoftDeleteById_MultipleItems_SetEntityStateToModified()
    {
      var deletedEpisode1 = episodesSeriesOne[1];
      var deletedEpisode2 = episodesSeriesOne[2];
      var deletedEpisode3 = episodesSeriesOne[3];
      var deletedEpisodesIds = new[] { deletedEpisode1.Id, deletedEpisode2.Id, deletedEpisode3.Id };

      episodeRepositorySUT.Delete(deletedEpisodesIds);

      Assert.DoesNotContain(deletedEpisode1, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode2, episodesDbSetMock.Object.Local);
      Assert.DoesNotContain(deletedEpisode3, episodesDbSetMock.Object.Local);
    }

    [Fact]
    public void GetById_OneItem_ReturnsCorrectItem()
    {
      var id = 1;
      var expectedEpisode = episodesSeriesOne.Single(e => e.Id == id);

      var episode = episodeRepositorySUT.GetById(id);

      Assert.Equal(expectedEpisode, episode);
    }

    [Fact]
    public void GetByIds_MultipleItems_ReturnsCorrectItem()
    {
      var id1 = 1;
      var id2 = 2;
      var id3 = 3;
      var expectedEpisode1 = episodesSeriesOne.Single(e => e.Id == id1);
      var expectedEpisode2 = episodesSeriesOne.Single(e => e.Id == id2);
      var expectedEpisode3 = episodesSeriesOne.Single(e => e.Id == id3);

      var episodes = episodeRepositorySUT.GetByIds(new[] { 1, 2, 3 });

      Assert.Contains(expectedEpisode1, episodes);
      Assert.Contains(expectedEpisode2, episodes);
      Assert.Contains(expectedEpisode3, episodes);
    }

    private EntityFrameworkRepository<TEntity, int> CreateEntityFrameworkRepository<TEntity>() where TEntity : class, IEntity<int>, new()
    {
      var unitOfWorkRegistryStub = new ThreadLocalUnitOfWorkRegistry();
      var unitOfWorkProvider = new EntityFrameworkUnitOfWorkProvider(unitOfWorkRegistryStub,
          CreateYesMinisterDbContext);
      IDateTimeProvider dateTimeProvider = new UtcDateTimeProvider();

      var unitOfWork = new EntityFrameworkUnitOfWork(unitOfWorkProvider, CreateYesMinisterDbContext,
          DbContextOptions.ReuseParentContext);
      unitOfWorkRegistryStub.RegisterUnitOfWork(unitOfWork);

      return new EntityFrameworkRepository<TEntity, int>(unitOfWorkProvider, dateTimeProvider);
    }

    private YesMinisterDbContext CreateYesMinisterDbContext()
    {
      return dbContextMock.Object;
    }
  }
}