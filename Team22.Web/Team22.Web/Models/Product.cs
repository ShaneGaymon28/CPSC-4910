using Team22.Web.Enums;
using System.Collections.Generic;

namespace Team22.Web.Models;

public class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double PointPrice { get; set; } // price of item in points
    
    public ProductCategory ProductCategory { get; set; } // category of product

    public int? TrackId { get; set; } // ID of song from ITunes API
    
    public string? TrackName { get; set; } // name of song
    
    public string? ArtworkURL { get; set; } // url to get artwork of song/album
    
    public string? ArtistName { get; set; } // name of artist
    
    public int? ArtistId { get; set; } // ID of artist from ITunes API
    
    public string? CollectionName { get; set; } // name of album
    
    public int? CollectionId { get; set; } // ID of album from ITunes API

    public double DollarPrice { get; set; } 
    
    public int CatalogId { get; set; }
    
    public Catalog Catalog { get; set; }

    // nullable because not every product will have an order
    public int? OrderId { get; set; }
    public Order? Order { get; set; }
}
