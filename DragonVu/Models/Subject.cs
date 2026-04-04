// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace DragonVu.Models
{
    public class Subject
    {

        [Key]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; } = null!;

        // السنة (1 أو 2 حالياً)

        public int Year { get; set; }
        [ValidateNever]
        public ICollection<Question> questions { get; set; }
        [ValidateNever]
        public ICollection<Result> results { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



    }

}




