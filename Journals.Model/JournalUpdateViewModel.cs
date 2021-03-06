﻿using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Journals.Model
{
    public class JournalUpdateViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public int UserId { get; set; }
    }
}