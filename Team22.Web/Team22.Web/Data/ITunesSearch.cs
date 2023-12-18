using iTunesSearch.Library.Models;
using Team22.Web.Enums;

namespace Team22.Web.Data;

/*
 * Class to encapsulate the iTunesSearch library classes so we can display the results
 */

public class ITunesSearch
{
    public ProductCategory ProductCategory { get; set; }
    
    public AlbumResult AlbumResult { get; set; } = null!;

    public PodcastListResult PodcastListResult { get; set; } = null!; 

    public SongResult SongResult { get; set; } = null!;

    public SongArtistResult SongArtistResult { get; set; } = null!;

    public TVEpisodeListResult TvEpisodeListResult { get; set; } = null!;

    public TVSeasonListResult TvSeasonListResult { get; set; } = null!;

}