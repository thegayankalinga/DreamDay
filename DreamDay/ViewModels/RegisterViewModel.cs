using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Please enter your email address.")]
    public string EmailAddress { get; set; }
    
    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please confirm password.")]
    public string ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "Please enter your first name.")]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Required(ErrorMessage = "Please enter your last name.")]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    // Role selection: "Couple", "Planner", or "Admin"
    [Required(ErrorMessage = "Please select a role.")]
    [Display(Name = "Registering As")]
    public string Role { get; set; }
    
    //Partner Fields
    [Display(Name = "Wedding Date")]
    public DateOnly? WeddingDate { get; set; }
    
    [Display(Name = "Partner Name")]
    public string? PartnerName { get; set; }
}