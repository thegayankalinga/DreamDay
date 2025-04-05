using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels;

public class LoginViewModel
{
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Please enter your email address")]
    public required string UserName { get; set; }
    
    [Required(ErrorMessage = "Please enter your password")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}