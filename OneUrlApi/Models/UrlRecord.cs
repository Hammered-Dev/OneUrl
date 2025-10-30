using System.ComponentModel.DataAnnotations;

namespace OneUrlApi.Models;

public class UrlRecord
{
    [Key]
    public int Id { get; set; }
    required public string Target { get; set; }
    required public string Location { get; set; }

}