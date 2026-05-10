using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class ReviewCreateViewModel
{
    // selectlist of movies (id, title)       
    public SelectList Movies { set; get; }

    // Collecting MovieId and Title in Form
    [Required(ErrorMessage = "Please select a Movie")]
    [Display(Name = "Find movie")]
    public int MovieId { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 5)]
    public string Title { get; set; } 
    
    [StringLength(500)]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    [Range(0,10)]
    public int Rating { get; set; }

}
