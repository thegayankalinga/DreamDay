using System.ComponentModel.DataAnnotations;

namespace DreamDay.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Please enter your email address.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")] 
    public required string EmailAddress { get; set; }

    [Required(ErrorMessage = "Please enter your password.")]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)] // Added StringLength attribute
    public required string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Please confirm password.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] // Added Compare attribute
    public required string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "Please enter your first name.")]
    [Display(Name = "First Name")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Please enter your last name.")]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Please select a role.")]
    [Display(Name = "Registering As")]
    public required string Role { get; set; }

    [Display(Name = "Wedding Date")]
    [DataType(DataType.Date)]
    public DateOnly? WeddingDate { get; set; }

    [Display(Name = "Partner Name")]
    public string? PartnerName { get; set; }
}