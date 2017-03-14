using Journals.Model;
using System.Web;

namespace Journals.Web.Helpers
{
    public static class JiHelper
    {
        public static void PopulateFileJournal(HttpPostedFileBase file, Journal journal)
        {
            if (file != null && file.ContentLength > 0)
            {
                journal.FileName = System.IO.Path.GetFileName(file.FileName);
                journal.ContentType = file.ContentType;

                using (var reader = new System.IO.BinaryReader(file.InputStream))
                {
                    journal.Content = reader.ReadBytes(file.ContentLength);
                }
            }
        }

        public static void PopulateFileIssue(HttpPostedFileBase file, Issue journal) {
            if (file != null && file.ContentLength > 0) {
                journal.FileName = System.IO.Path.GetFileName(file.FileName);
                journal.ContentType = file.ContentType;

                using (var reader = new System.IO.BinaryReader(file.InputStream)) {
                    journal.Content = reader.ReadBytes(file.ContentLength);
                }
            }
        }
    }
}