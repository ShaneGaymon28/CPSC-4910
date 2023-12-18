using iTunesSearch.Library.Models;
using Microsoft.EntityFrameworkCore;
using Team22.Web.Contexts;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

/*
 * Create functions to add each type of product to the product table
 */

public class ProductService
{
    private readonly Team22Context _context;
    
    public ProductService(Team22Context context)
    {
        _context = context;
    }
    
    #region Add Products

    public class AddProductQuery
    {
        public int CatalogId { get; set; }
        public Song? Song { get; set; }
        public Album? Album { get; set; }
        public TVEpisode? Episode { get; set; }
        public TVSeason? Season { get; set; }
    }

    public async Task<QueryResult<QueryStatus>> AddSong(AddProductQuery request)
    {
        var catalog = await _context.Catalog.Include(s => s.Sponsor).FirstOrDefaultAsync(c => c.Id == request.CatalogId);
        if (catalog is null)
        {
            return QueryResult<QueryStatus>.NotFound();
        }
        
        // check if a song already exists
        if (_context.Products.Any(p => p.TrackName == request.Song.TrackName 
                                    && p.CatalogId == request.CatalogId
                                    && p.ProductCategory == ProductCategory.Song))
        {
            return QueryResult<QueryStatus>.Conflict();
        }

        // calculate point-price value
        var point = (request.Song.TrackPrice / catalog.Sponsor.PointDollarRatio);
        
        var newProduct = new Product
        {
            TrackId = Convert.ToInt32(request.Song.TrackId),
            ArtistId = Convert.ToInt32(request.Song.ArtistId),
            TrackName = request.Song.TrackName,
            ArtistName = request.Song.ArtistName,
            CollectionName = request.Song.CollectionName,
            CollectionId = Convert.ToInt32(request.Song.CollectionId),
            ArtworkURL = request.Song.ArtworkUrl100,
            DollarPrice = request.Song.TrackPrice,
            PointPrice = point,
            CatalogId = request.CatalogId,
            Name = request.Song.TrackName,
            ProductCategory = ProductCategory.Song
        };
        
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();
        
        return QueryResult<QueryStatus>.Success();
    }

    public async Task<QueryResult<QueryStatus>> AddAlbum(AddProductQuery request)
    {
        // get the catalog we're updating
        var catalog = await _context.Catalog.Include(s => s.Sponsor).FirstOrDefaultAsync(c => c.Id == request.CatalogId);
        if (catalog is null)
        {
            return QueryResult<QueryStatus>.NotFound();
        }
        
        // check for duplicates
        if (_context.Products.Any(p => p.CollectionName == request.Album.CollectionName 
                                       && p.CatalogId == request.CatalogId
                                       && p.ProductCategory == ProductCategory.Album))
        {
            return QueryResult<QueryStatus>.Conflict();
        }

        // calculate point-price value
        var point = (request.Album.CollectionPrice / catalog.Sponsor.PointDollarRatio);
        
        var newProduct = new Product
        {
            ArtistId = Convert.ToInt32(request.Album.ArtistId),
            ArtistName = request.Album.ArtistName,
            CollectionName = request.Album.CollectionName,
            CollectionId = Convert.ToInt32(request.Album.CollectionId),
            ArtworkURL = request.Album.ArtworkUrl100,
            DollarPrice = request.Album.CollectionPrice,
            PointPrice = point,
            CatalogId = request.CatalogId,
            Name = request.Album.CollectionName,
            ProductCategory = ProductCategory.Album
        };
        
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();
        
        return QueryResult<QueryStatus>.Success();
    }
    
    public async Task<QueryResult<QueryStatus>> AddEpisode(AddProductQuery request)
    {
        var catalog = await _context.Catalog.Include(s => s.Sponsor).FirstOrDefaultAsync(c => c.Id == request.CatalogId);
        if (catalog is null)
        {
            return QueryResult<QueryStatus>.NotFound();
        }
        
        // check if an episode already exists
        if (_context.Products.Any(p => p.Name == request.Episode.ShowName 
                                       && p.CatalogId == request.CatalogId
                                       && p.ProductCategory == ProductCategory.TvEpisode))
        {
            return QueryResult<QueryStatus>.Conflict();
        }

        // calculate point-price value
        var point = (decimal.ToDouble(request.Episode.Price) / catalog.Sponsor.PointDollarRatio);
        
        var newProduct = new Product
        {
            TrackId = Convert.ToInt32(request.Episode.EpisodeId),
            TrackName = request.Episode.Name,
            ArtistId = Convert.ToInt32(request.Episode.ShowId),
            ArtistName = request.Episode.ShowName,
            CollectionName = request.Episode.SeasonName,
            ArtworkURL = request.Episode.ArtworkUrl,
            DollarPrice = decimal.ToDouble(request.Episode.Price),
            PointPrice = point,
            CatalogId = request.CatalogId,
            Name = request.Episode.Name,
            ProductCategory = ProductCategory.TvEpisode
        };
        
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();
        
        return QueryResult<QueryStatus>.Success();
    }
    
    public async Task<QueryResult<QueryStatus>> AddSeason(AddProductQuery request)
    {
        var catalog = await _context.Catalog.Include(s => s.Sponsor).FirstOrDefaultAsync(c => c.Id == request.CatalogId);
        if (catalog is null)
        {
            return QueryResult<QueryStatus>.NotFound();
        }
        
        // check for duplicates
        if (_context.Products.Any(p => p.CollectionName == request.Season.SeasonName 
                                       && p.CatalogId == request.CatalogId
                                       && p.ProductCategory == ProductCategory.TvSeason))
        {
            return QueryResult<QueryStatus>.Conflict();
        }

        // calculate point-price value
        var point = (decimal.ToDouble(request.Season.SeasonPrice) / catalog.Sponsor.PointDollarRatio);
        
        var newProduct = new Product
        {
            ArtistId = Convert.ToInt32(request.Season.ShowId),
            ArtistName = request.Season.ShowName,
            CollectionId = Convert.ToInt32(request.Season.SeasonId),
            CollectionName = request.Season.SeasonName,
            ArtworkURL = request.Season.ArtworkUrl,
            DollarPrice = decimal.ToDouble(request.Season.SeasonPrice),
            PointPrice = point,
            CatalogId = request.CatalogId,
            Name = request.Season.SeasonName,
            ProductCategory = ProductCategory.TvSeason
        };
        
        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();
        
        return QueryResult<QueryStatus>.Success();
    }
    
    #endregion

    #region Delete Products

    public async Task<QueryResult<QueryStatus>> DeleteProduct(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product is null)
        {
            return QueryResult<QueryStatus>.NotFound();
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return QueryResult<QueryStatus>.Success();
    }

    #endregion
    
}