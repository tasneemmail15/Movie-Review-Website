
using SMS.Data.Entities;

namespace SMS.Web.Models;

public class ReviewSearchViewModel
{
    // result set
    public IList<Review> Review { get; set;} = new List<Review>();

    // search options        
    public string Query { get; set; } = string.Empty;
    public ReviewRange Range { get; set; } = ReviewRange.open;
}

