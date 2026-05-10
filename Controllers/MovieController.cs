
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SMS.Data.Entities;
using SMS.Data.Services;
using SMS.Web.Models;

namespace SMS.Web.Controllers;

[Authorize]
public class MovieController : BaseController
{
    private IMovieService svc;

    public MovieController(IMovieService _svc)
    {
        svc = _svc;            
    }

    // GET /movie
    public IActionResult Index(int page=1, int size=10, string orderBy="id", string direction="asc")
    {
        // load movies using service and pass to view
        var data = svc.GetMovies(page,size,orderBy, direction);
        
        return View(data);
    }

    // GET /Movies/details/{id}
    public IActionResult Details(int id)
    {
        var movie = svc.GetMovie(id);
      
        if (movie is null) {
            Alert($"Movie {id} Not Found..", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }
        return View(movie);
    }

    // GET: /movie/create
    [Authorize(Roles="admin")]
    public IActionResult Create()
    {
        // display blank form to create a movie
        return View();
    }

    // POST /movie/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="admin")]
    public IActionResult Create(MovieViewModel m)
    {   
        // validate title as unique
        if (svc.GetMovieByTitle(m.Title) != null)
        {
            ModelState.AddModelError(nameof(m.Title), "Title already exixts");
        }

        // complete POST action to add movie
        if (ModelState.IsValid)
        {
            // call service AddMovie method using data in viewmodel s converted to a movie model
            var movie = svc.AddMovie(m.ToMovie()) ;
            if (movie is null) 
            {
                Alert($"Movie could not be created..", AlertType.warning);
                return RedirectToAction(nameof(Index));
            }else
            {
                Alert("Movie created successfully", AlertType.success);
            }

            return RedirectToAction(nameof(Details), new { Id = movie.Id});   
        }
        
        // redisplay the form for editing as there are validation errors
        return View(m);
    }

    // GET /movie/edit/{id}
    [Authorize(Roles="admin,support")]
    public IActionResult Edit(int id)
    {
        // load the movie using the service
        var movie = svc.GetMovie(id);

        // check if movie is null
        if (movie is null)
        {
             Alert($"Movie {id} Not Found..", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }  

        // pass movieviewmodel to view for editing
        return View(MovieViewModel.FromMovie(movie));
    }

    // POST /movie/edit/{id}
    [ValidateAntiForgeryToken]
    [HttpPost]
    [Authorize(Roles="admin,support")]
    public IActionResult Edit(int id, MovieViewModel m)
    {
        // add validation error if email exists and is not owned by movie being edited 
        var existing = svc.GetMovieByTitle(m.Title);
        if (existing != null && m.Id != existing.Id) 
        {
            ModelState.AddModelError(nameof(m.Title), "The title is already in use");
        } 

        // complete POST action to save movie changes
        if (ModelState.IsValid)
        {            
            var movie = svc.UpdateMovie(m.ToMovie());
            if (movie is null) 
            {
                Alert($"Movie could not be updated..", AlertType.warning);
                return RedirectToAction(nameof(Details), new { Id = id });
            }
            else
            {
                Alert("Movie updated successfully", AlertType.success);
            }

            // redirect back to view the movie details
            return RedirectToAction(nameof(Details), new { Id = id });
        }

        // redisplay the form for editing as validation errors
        return View(m);
    }

    // GET / movie/delete/{id}
    [Authorize(Roles="admin")]
    public IActionResult Delete(int id)
    {
        // load the movie using the service
        var movie = svc.GetMovie(id);
        // check the returned movie is not null 
        if (movie == null)
        {
            Alert($"Movie {id} could not be deleted..", AlertType.danger);
            return RedirectToAction(nameof(Index));          
        }     
        
        // pass movie to view for deletion confirmation
        return View(movie);
    }

    // POST /movie/delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles="admin")]
    public IActionResult DeleteConfirm(int id)
    {
        // delete movie via service
        var deleted = svc.DeleteMovie(id);
        if (deleted)
        {
            Alert("Movie Deleted", AlertType.success);
        }
        else
        {
            Alert("Movie could not be deleted", AlertType.warning);
        }
        
        // redirect to the index view
        return RedirectToAction(nameof(Index));
    }

     // ============== Movie Review Management ==============

    // GET /movie/reviewcreate/{id}
    [HttpGet]
    public IActionResult ReviewCreate(int id)
    {
        var movie = svc.GetMovie(id);
        if (movie == null)
        {
            Alert("Movie does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }

        // create a review view model and set foreign key
        var review = new Review { MovieId = id }; 
        // render blank form
        return View( review );
    }

    // POST /movie/reviewcreate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReviewCreate([Bind("MovieId, Title, Content, Ratings")] Review r)
    {
        if (ModelState.IsValid)
        {
            var review = svc.CreateReview(r.MovieId, r.Title, r.Content, r.Ratings); 
            if (review is not null)
            {
                Alert("Review Created Successfully", AlertType.success);
            }
            else
            {
                Alert("Review could not be created", AlertType.warning);
            }

            // redirect to display movie - note how Id is passed
            return RedirectToAction(
                nameof(Details), new { Id = review.MovieId }
            );
        }
        // redisplay the form for editing
        return View(r);
    }

     // GET /movie/reviewedit/{id}
     [HttpGet]
    public IActionResult ReviewEdit(int id)
    {
        var review = svc.GetReview(id);
        if (review == null)
        {
            Alert("Review does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }
                
        return View( review );
    }

    // POST /movie/reviewedit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReviewEdit(int id,  [Bind("MovieId, Title, Ratings, Content")] Review r)
    {
        if (ModelState.IsValid)
        {        
            var review = svc.UpdateReview(id, r.Title, r.Content, r.Ratings);
            if (review is not null)
            {
                Alert("Review updated Successfully", AlertType.success);
            }
            else
            {
                Alert("Review could not be updated", AlertType.warning);
            }

            // redirect to display movie - note how Id is passed
            return RedirectToAction(
                nameof(Details), new { Id = review.MovieId }
            );
        }
        // redisplay the form for editing
        return View(r);
    }

    // GET /movie/reviewdelete/{id}
    [HttpGet]
    public IActionResult DeleteReview(int id)
    {
        // load the review using the service
        var review = svc.GetReview(id);
        // check the returned Review is not null
        if (review == null)
        {
            Alert("Review does not exist", AlertType.warning);
            return RedirectToAction(nameof(Index));
        }     
        
        // pass review to view for deletion confirmation
        return View(review);
    }

    // POST /movie/deleteconfirm/{id}
    [HttpPost]
    public IActionResult ReveiwDeleteConfirm(int id, int movieId)
    {
        var deleted = svc.DeleteReview(id);

        if (deleted)
            {
                Alert("Review deleted Successfully", AlertType.success);
            }
            else
            {
                Alert("Review could not be deleted", AlertType.warning);
            }

        // redirect to the movie details view
        return RedirectToAction(nameof(Details), new { Id = movieId });
    }

}