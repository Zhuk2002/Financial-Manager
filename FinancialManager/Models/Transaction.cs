using System;
using System.ComponentModel.DataAnnotations;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Range(-10000000, 10000000, ErrorMessage = "Let's be realistic")]
    public decimal Amount { get; set; }

    [StringLength(50)]
    public string? Description { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [Display(Name = "Income")]
    public bool IsIncome { get; set; }

    [Required]
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}
