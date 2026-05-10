using System.ComponentModel.DataAnnotations;
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class MovieViewModel
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }


    [Required]
    public string Director { get; set; }

    [Required]
    public string Genre { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }

    [Required]
    [Range(0, 10)]
    public double AvgRating { get; set; }


    

    // Convert ViewModel to a Movie model
    public Movie ToMovie()
    {
        return new Movie
        {
            Id = Id,
            Title = Title,
            Director = Director,
            Genre = Genre,
            ReleaseDate = ReleaseDate,
            AvgRating = AvgRating,
            
        };
    }

    // static method to create a viewmodel from a Movie entity
    public static MovieViewModel FromMovie(Movie m)
    {
        return new MovieViewModel
        {
            Id = m.Id,
            Title = m.Title,
            Director = m.Director,
            Genre = m.Genre,
            ReleaseDate = m.ReleaseDate,
            AvgRating= m.AvgRating, 
        };
    }


}
