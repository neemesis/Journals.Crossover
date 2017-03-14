using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Journals.Model {
    public class Issue {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public DateTime ModifiedDate { get; set; }

        [ForeignKey("UserId")]
        public UserProfile User { get; set; }

        public int UserId { get; set; }

        [ForeignKey("JournalId")]
        public Journal Journal { get; set; }
        public int JournalId { get; set; }
    }
}
