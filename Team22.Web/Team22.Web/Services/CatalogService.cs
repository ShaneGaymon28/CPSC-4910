using System.Text;
using iTunesSearch.Library;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Team22.Web.Contexts;
using Team22.Web.Data;
using Team22.Web.Enums;
using Team22.Web.Models;
using Team22.Web.Utilities;

namespace Team22.Web.Services;

public class CatalogService
{
    // to access info from iTunes APi, use this searchManager
    private iTunesSearchManager searchManager;
    private Team22Context _context;
    
    public CatalogService(Team22Context context)
    {
        searchManager = new iTunesSearchManager();
        _context = context;
    }

    #region GET iTunes API Search
    
    // searches for a song by songName
    public async Task<QueryResult<ITunesSearch>> GetSong(string songName, int resultLimit = 100)
    {
        var items = await searchManager.GetSongsAsync(songName);
        if (items is null)
        {
            return QueryResult<ITunesSearch>.NotFound();
        }
        
        return QueryResult<ITunesSearch>.Success(new ITunesSearch
        {
            SongResult = items,
            ProductCategory = ProductCategory.Song
        });
    }

    // searches for an album by albumName
    public async Task<QueryResult<ITunesSearch>> GetAlbum(string albumName, int resultLimit = 100)
    {
        var items = await searchManager.GetAlbumsAsync(albumName);
        if (items is null)
        {
            return QueryResult<ITunesSearch>.NotFound();
        }
        
        return QueryResult<ITunesSearch>.Success(new ITunesSearch
        {
            AlbumResult = items,
            ProductCategory = ProductCategory.Song
        });
    }
    
    // searches for TV episodes by showName
    public async Task<QueryResult<ITunesSearch>> GetTvEpisodes(string showName, int resultLimit = 100)
    {
        var items = await searchManager.GetTVEpisodesForShow(showName);
        if (items is null)
        {
            return QueryResult<ITunesSearch>.NotFound();
        }
        
        return QueryResult<ITunesSearch>.Success(new ITunesSearch
        {
            TvEpisodeListResult = items,
            ProductCategory = ProductCategory.Song
        });
    }

    public async Task<QueryResult<ITunesSearch>> GetTvSeasons(string showName, int resultLimit = 100)
    {
        var items = await searchManager.GetTVSeasonsForShow(showName);
        if (items is null)
        {
            return QueryResult<ITunesSearch>.NotFound();
        }

        return QueryResult<ITunesSearch>.Success(new ITunesSearch
        {
            TvSeasonListResult = items,
            ProductCategory = ProductCategory.TvSeason
        });
    }
    
    
    
    #endregion
    
    #region Create Catalog

    // create a catalog for sponsor with sponsor ID
     
    public async Task<QueryStatus> CreateCatalog(int sponsorId)
    {
        // check that the sponsor exists
        var sponsor = _context.Sponsors.FirstOrDefault(s => s.Id == sponsorId);
        if (sponsor is null)
        {
            return QueryStatus.NotFound;
        }

        // check that the sponsor doesn't already have a catalog
        var catalog = _context.Catalog.FirstOrDefault(c => c.SponsorId == sponsorId);
        if (catalog is not null)
        {
            return QueryStatus.Conflict;
        }

        Catalog newCatalog = new Catalog
        {
            Sponsor = sponsor,
            SponsorId = sponsor.Id
            
        };

        _context.Catalog.Add(newCatalog);
        
        await _context.SaveChangesAsync();

        return QueryStatus.Success;
    }

    #endregion
    
    #region GET Catalog
    
     public async Task<QueryResult<Catalog>> GetCatalogBySponsorId(int sponsorId)
    {
        // get the catalog
        var catalog = _context.Catalog.FirstOrDefault(c => c.SponsorId == sponsorId);
        if (catalog is null)
        {
            return QueryResult<Catalog>.NotFound();
        }
        
        // get products with matching catalog id
        var catalogProducts = await _context.Products.Where(p => p.CatalogId == catalog.Id).ToListAsync();
        if (catalogProducts is not null)
        {
            catalog.Products = catalogProducts;
        }
        
        return QueryResult<Catalog>.Success(catalog);
    }

     public async Task<QueryResult<Catalog>> GetCatalogById(int catalogId)
     {
         var catalog = _context.Catalog.FirstOrDefault(c => c.Id == catalogId);
         if (catalog is null)
         {
             return QueryResult<Catalog>.NotFound();
         }
        
         // get products with matching catalog id
         var catalogProducts = await _context.Products.Where(p => p.CatalogId == catalog.Id).ToListAsync();
         if (catalogProducts is not null)
         {
             catalog.Products = catalogProducts;
         }
         
         return QueryResult<Catalog>.Success(catalog);
     }

    #endregion

    #region Search Catalog

    public async Task<QueryResult<string>> SearchCatalog(string search)
    {
        var httpClient = new HttpClient();
        var URL = $"https://itunes.apple.com/search?term={search}&limit=25";
        var response = await httpClient.GetAsync(URL);
        var returnedResponse = await response.Content.ReadAsStringAsync();

        return QueryResult<string>.Success(returnedResponse);
    }

    #endregion

    #region Catalog String To CSV
    private class CatalogEntry
    {
        public string ArtistName { get; set; } = null!;
        public string TrackName { get; set; } = null!;
        public string CollectionName { get; set; } = null!;
        public string TrackPrice { get; set; } = null!;
        public string CollectionPrice { get; set; } = null!;
        public string TrackViewUrl { get; set; } = null!;
        public string CollectionViewUrl { get; set; } = null!;
        public string ArtworkUrl100 { get; set; } = null!;
        public string ReleaseDate { get; set; } = null!;
        public string PrimaryGenreName { get; set; } = null!;
    }

    public QueryStatus CatalogStringToCSV(string catalogString)
    {
        var catalog = JsonConvert.DeserializeObject<List<CatalogEntry>>(catalogString);
        var csv = new StringBuilder();

        csv.AppendLine("Artist,Album,Track,Price,Genre,Release Date,Preview URL,Artwork URL");

        for (int i = 0; i < catalog.Count; i++)
        {
            csv.AppendLine(
                $"{catalog[i].ArtistName},{catalog[i].CollectionName},{catalog[i].TrackName},{catalog[i].TrackPrice},{catalog[i].PrimaryGenreName},{catalog[i].ReleaseDate},{catalog[i].TrackViewUrl},{catalog[i].ArtworkUrl100}");
        }

        // build unique file name
        var filename = $"Catalog_{DateTime.Now:yyyyMMddHHmmss}.csv";

        // ensure file is in CSV Files folder
        filename = Path.Combine("CSV Files", filename);

        // write to file
        File.WriteAllText(filename, csv.ToString());

        return QueryStatus.Success;
    }

    #endregion
    
}