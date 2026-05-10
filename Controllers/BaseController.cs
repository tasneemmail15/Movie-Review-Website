using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;

namespace SMS.Web.Controllers;
public enum AlertType { success, danger, warning, info }
public class BaseController : Controller
{
    // Store Alert in TempData Storage thus only accessible in next Request
    public void Alert(string message, AlertType type = AlertType.info)
    {
        TempData["Alert.Message"] = message;
        TempData["Alert.Type"] = type.ToString();
    }

    public void SendAlert(bool condition, string success, string failure)
    {
        if (condition)
        {
            Alert(success, AlertType.success);
        }
        else
        {
            Alert(failure, AlertType.danger);
        }
    }

    public static T GetApi<T>(string url)
    {
        using(var http = new HttpClient()) { 
            try {
                var result = http.GetStringAsync(url).Result;
                return JsonSerializer.Deserialize<T>(result);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return default;
            }
        }
    }

}
