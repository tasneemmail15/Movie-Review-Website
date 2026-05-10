using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

using SMS.Web.Models;
using SMS.Data.Services;
using SMS.Data.Entities;

namespace SMS.Web.Controllers;

public class ReviewController : BaseController
{
    private readonly IMovieService svc;
    private readonly ILogger<HomeController> logger;
   
    public ReviewController(IMovieService _svc, ILogger<HomeController> _logger)
    {
        svc = _svc;
        logger = _logger;
    }

    // GET /review/index
    [Authorize(Roles="admin,support")]
    public IActionResult Index(ReviewSearchViewModel search)
    {                  
        // set the viewmodel review property by calling service method 
        // using the range and query values from the viewmodel 
        search.Review = svc.SearchReviews(range: search.Range, query: search.Query);
         
        // check if request is from HTMX and return review table partial view
        if (Request.Headers.ContainsKey("HX-Request"))
        {
            logger.LogInformation($"HX Request: query={search.Query} range={search.Range}");
            return PartialView("_Review", search);
        }

        // standard request so return full page
        return View("Index", search);
    } 

    // GET/reveiw/{id}
    public IActionResult Details(int id)
    {
        var reveiw = svc.GetReview(id);
        if (reveiw == null)
        {
            Alert("Review Not Found", AlertType.warning);  
            return RedirectToAction(nameof(Index));             
        }

        return View("Details",reveiw);
    }

    // public IActionResult Delete([Bind("Id")] Review r)
    // {
    //     // delete review via service
    //     var review = svc.DeleteReview(r.Id);
    //     // if (review is null)
    //     // {
    //     //     Alert("Review not found", AlertType.warning);
    //     //     return RedirectToAction(nameof(Index));
    //     // }
        
    //     Alert($"Review {r.Id } closed", AlertType.info);  
    //     return RedirectToAction(nameof(Details), new { Id = r.Id });       
    // }

    // GET /review/create
    [Authorize(Roles="admin,support")]
    public IActionResult Create(int page=1, int size=10, string query=null)
    {       
        var rvm = new ReviewCreateViewModel();

        // check if request is from HTMX
        if (Request.Headers.ContainsKey("HX-Request"))
        {
            // check if there is a query string
            if (!string.IsNullOrEmpty(query))
            {  
                rvm.Movies = new SelectList(svc.FindMovies(query), "Id", "Title");
            }
            logger.LogInformation($"HX Request: query={query}");
            return PartialView("_Movies", rvm);
        }

        // standard request
        return View(rvm);
    }

    // POST /review/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="admin,support")]
    public IActionResult Create(ReviewCreateViewModel rvm)   
    {
        if (ModelState.IsValid)
        {
            var review = svc.CreateReview(rvm.MovieId, rvm.Title, rvm.Content, rvm.Rating);           
            if (review is null)
            {
                Alert("Review not created", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            
            Alert("Review Created", AlertType.success);
            return RedirectToAction(nameof(Details), new { Id = review.Id });           
        }

        // before sending viewmodel back (due to validation issues) repopulate the select list            
        rvm.Movies = new SelectList(svc.FindMovies(), "Id", "Title");

        return View(rvm);
    }

    // GET /review/create
    [Authorize(Roles="admin,support")]
    public IActionResult Edit(int page=1, int size=10, string query=null)
    {       
        var rvm = new ReviewCreateViewModel();

        // check if request is from HTMX
        if (Request.Headers.ContainsKey("HX-Request"))
        {
            // check if there is a query string
            if (!string.IsNullOrEmpty(query))
            {  
                rvm.Movies = new SelectList(svc.FindMovies(query), "Id", "Title");
            }
            logger.LogInformation($"HX Request: query={query}");
            return PartialView("_Movies", rvm);
        }

        // standard request
        return View(rvm);
    }

    // POST /review/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="admin,support")]
    public IActionResult Edit(ReviewCreateViewModel rvm)   
    {
        if (ModelState.IsValid)
        {
            var review = svc.CreateReview(rvm.MovieId, rvm.Title, rvm.Content, rvm.Rating);           
            if (review is null)
            {
                Alert("Review not Updated", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }
            
            Alert("Review Updated", AlertType.success);
            return RedirectToAction(nameof(Details), new { Id = review.Id });           
        }

        // before sending viewmodel back (due to validation issues) repopulate the select list            
        rvm.Movies = new SelectList(svc.FindMovies(), "Id", "Title");

        return View(rvm);
    }
}
