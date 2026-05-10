
namespace SMS.Web.Models;

public class AboutViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Formed { get; set; } = DateTime.Now;
    public string FormedString => Formed.ToLongDateString();
    public int Days => (DateTime.Now - Formed).Days;
}