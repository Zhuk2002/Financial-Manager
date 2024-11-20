using System;
using System.ComponentModel.DataAnnotations;

public class Transaction
{
    [Key]
    public int Id { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [StringLength(100)]
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
