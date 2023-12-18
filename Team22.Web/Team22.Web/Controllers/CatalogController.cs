using iTunesSearch.Library.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team22.Web.Services;
using Team22.Web.Data;
using Team22.Web.Enums;
using Team22.Web.Models;

namespace Team22.Web.Controllers;

public class CatalogController : Controller
{

    private readonly ILogger<CatalogController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly CatalogService _catalogService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ProductService _productService;
    private readonly SponsorService _sponsorService;
    private readonly UserService _userService;

    public CatalogController(ILogger<CatalogController> logger, IHttpClientFactory httpClientFactory, CatalogService catalogService, 
        UserManager<AppUser> userManager, ProductService productService, SponsorService sponsorService, UserService userService)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _catalogService = catalogService;
        _userManager = userManager;
        _productService = productService;
        _sponsorService = sponsorService;
        _userService = userService;
    }
    
    // prob delete
    public IActionResult DriverSearch()
    {
        return View();
    }

    /**
     * GET CATALOG FOR DRIVER
     * gets the catalog for a driver
     */
    public async Task<IActionResult> DriverCatalog()
    {
        // get the catalog and products from db
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var bridge = await _userService.GetSponsorUserBridge(user.Id);
        if (bridge.Status == QueryStatus.NotFound)
        {
            return View();
        }
        
        ViewBag.CurrentPoints = bridge.Value.Points != null ? bridge.Value.Points : 0;
        var catalog = await _catalogService.GetCatalogBySponsorId(bridge.Value.SponsorId);
        return View(catalog.Value);
    }

    /*
     * GET SPONSOR CATALOG
     * gets the catalog for a given sponsor
    */
    [HttpGet]
    public async Task<IActionResult> SponsorCatalog()
    { 
       var user = await _userManager.GetUserAsync(HttpContext.User);
       var bridge = await _userService.GetSponsorUserBridge(user.Id);

       return await _catalogService.GetCatalogBySponsorId(bridge.Value.SponsorId) switch
       {
           { Status: QueryStatus.NotFound } => View(),
           { Status: QueryStatus.Success } result => View(result.Value)
       };
       
    }
    
    /*
     * SPONSOR ADD TO CATALOG
     * view for sponsors to search the api and add to their catalog
     */
    public IActionResult SponsorAdd(int catalogId)
    {
        ViewBag.CatalogId = catalogId;
        return View();
    }

    /**
     * Admin view catalog
     *
     * Note: as I am about to commit this, I realized there is no requirement for admin catalog...
     */
    [HttpGet]
    public async Task<IActionResult> AdminCatalog()
    {
        // get a list of sponsors 
        ViewBag.sponsors = await _sponsorService.GetAllSponsors();

        return View();
    }
    
    
    [HttpGet]
    public async Task<IActionResult> AdminGetSponsorCatalog(int sponsorId)
    {
        ViewBag.sponsors = await _sponsorService.GetAllSponsors();
        
        return await _catalogService.GetCatalogBySponsorId(sponsorId) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } result => View("AdminCatalog",result.Value)
        };
    }
    

    #region API Search
    
    /*
     * SEARCH ITUNES API
     * function to search the API and return the results to SponsorAdd view
     */
    
    [HttpGet]
    public async Task<IActionResult> ApiSearch(int catalogId, string search, ProductCategory category)
    {
        // to keep track of which catalog id
        ViewBag.CatalogId = catalogId;
        
        // decide which product category the user is searching for
        switch (category)
        {
            case ProductCategory.Song:
                ViewBag.Category = "Song";
                return View("SponsorAdd", await SearchSong(search));
            case ProductCategory.Album:
                ViewBag.Category = "Album";
                return View("SponsorAdd", await SearchAlbum(search));
            case ProductCategory.TvEpisode:
                ViewBag.Category = "TvEpisode";
                return View("SponsorAdd", await SearchEpisode(search));
            case ProductCategory.TvSeason:
                ViewBag.Category = "TvSeason";
                return View("SponsorAdd", await SearchSeason(search));
        }
        
        // shouldn't get here
        return View("SponsorAdd");
    }

    
    /*
     * SEARCH ITUNES API FOR SONG
     * function to search for a song
     */
    public async Task<ITunesSearch> SearchSong(string search)
    {
        return await _catalogService.GetSong(search) switch
        {
            { Status: QueryStatus.NotFound } => null, // not sure what else to return here
            { Status: QueryStatus.Success } result => result.Value 
        };
    }
    
    /*
     * SEARCH ITUNES API FOR ALBUM
     * function to search for an album
     */
    public async Task<ITunesSearch> SearchAlbum(string search)
    {
        return await _catalogService.GetAlbum(search) switch
        {
            { Status: QueryStatus.NotFound } => null, // not sure what else to return here
            { Status: QueryStatus.Success } result => result.Value 
        };
    }

    /*
     * SEARCH ITUNES API FOR TV EPISODE
     * function to search for an episode 
     */
    public async Task<ITunesSearch> SearchEpisode(string search)
    {
        return await _catalogService.GetTvEpisodes(search) switch
        {
            { Status: QueryStatus.NotFound } => null,
            { Status: QueryStatus.Success } result => result.Value
        };
    }

    /*
     * SEARCH ITUNES API FOR TV SEASON
     * function to search for a season
     */
    public async Task<ITunesSearch> SearchSeason(string search)
    {
        return await _catalogService.GetTvSeasons(search) switch
        {
            { Status: QueryStatus.NotFound } => null,
            { Status: QueryStatus.Success } result => result.Value
        };
    }

    // can prob delete
    [HttpGet]
    public async Task<IActionResult> SearchCatalog(string search)
    {
        var httpClient = _httpClientFactory.CreateClient();
        var URL = $"https://itunes.apple.com/search?term={search}&limit=25";
        var response = await httpClient.GetAsync(URL);
        var returnedResponse = await response.Content.ReadAsStringAsync();

        return View("DriverSearch", returnedResponse);
    }
    #endregion
    
    #region Add to Sponsor Catalog
    
    /*
     * SPONSOR ADD TO CATALOG
     * function to add an item to a sponsor's catalog
     * 
     */
    [HttpPost]
    public async Task<IActionResult> AddSong(int catalogId, Song song)
    {
        // attempt to add song to db
        var result = await _productService.AddSong(new ProductService.AddProductQuery
            { CatalogId = catalogId, Song = song });

        if (result.Status == QueryStatus.NotFound || result.Status == QueryStatus.Conflict)
        {
            return NotFound();
        }
        
        // get updated catalog
        var catalog = await _catalogService.GetCatalogById(catalogId);
        return View("SponsorCatalog", catalog.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAlbum(int catalogId, Album album)
    {
        var result = await _productService.AddAlbum(new ProductService.AddProductQuery
            { CatalogId = catalogId, Album = album });

        if (result.Status == QueryStatus.NotFound || result.Status == QueryStatus.Conflict)
        {
            return NotFound();
        }
        
        // get updated catalog
        var catalog = await _catalogService.GetCatalogById(catalogId);
        return View("SponsorCatalog", catalog.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddEpisode(int catalogId, TVEpisode episode)
    {
        var result = await _productService.AddEpisode(new ProductService.AddProductQuery
            { CatalogId = catalogId, Episode = episode });

        if (result.Status == QueryStatus.NotFound || result.Status == QueryStatus.Conflict)
        {
            return NotFound();
        }
        
        // get updated catalog
        var catalog = await _catalogService.GetCatalogById(catalogId);
        return View("SponsorCatalog", catalog.Value);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddSeason(int catalogId, TVSeason season)
    {
        var result = await _productService.AddSeason(new ProductService.AddProductQuery
            { CatalogId = catalogId, Season = season });

        if (result.Status == QueryStatus.NotFound || result.Status == QueryStatus.Conflict)
        {
            return NotFound();
        }
        
        // get updated catalog
        var catalog = await _catalogService.GetCatalogById(catalogId);
        return View("SponsorCatalog", catalog.Value);
    }
    
    #endregion
    
    
    /*
     * REMOVE PRODUCT FROM SPONSOR CATALOG
     * deletes the product with matching productId 
     */
    public async Task<IActionResult> RemoveFromCatalog(int productId)
    {
        return await _productService.DeleteProduct(productId) switch
        {
            { Status: QueryStatus.NotFound } => NotFound(),
            { Status: QueryStatus.Success } => RedirectToAction("SponsorCatalog")
        };
    }
}