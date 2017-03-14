using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Journals.Model {
    public class IssueViewModel {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }

        public int JournalId { get; set; }

        [Required, ValidateFile2]
        public HttpPostedFileBase File { get; set; }

        public int UserId { get; set; }
    }

    public class ValidateFile2Attribute : ValidationAttribute {
        public override bool IsValid(object value) {
            double maxContentLength = 1024 * 1024 * 3.5; //3.5 MB
            string AllowedFileExtensions = ".pdf";

            var file = value as HttpPostedFileBase;

            if (file == null)
                return true;
            if (!AllowedFileExtensions.Equals(file.FileName.Substring(file.FileName.LastIndexOf('.')), StringComparison.InvariantCultureIgnoreCase)) {
                ErrorMessage = "Please upload journal in pdf format";
                return false;
            } else if (file.ContentLength > maxContentLength) {
                ErrorMessage = "Journal is too large, maximum allowed size is : " + ((maxContentLength / 1024) / 1024).ToString() + "MB";
                return false;
            } else
                return true;
        }
    }
}
