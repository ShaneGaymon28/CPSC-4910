using static Team22.Web.Services.ApplicationService;
using Team22.Web.Enums;
using Team22.Web.Contexts;
using Team22.Web.Models;
using Team22.Web.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Team22.Web.Services
{
    public class AuditService
    {
        private readonly Team22Context _context;

        public AuditService(Team22Context context)
        {
            _context = context;
        }

        #region Get all audits

        public async Task<QueryResult<List<Audit>>> GetAllAudits()
        {
            return QueryResult<List<Audit>>.Success(await _context.Audits.ToListAsync());
        }

        #endregion

        #region Get audits by subject

        public async Task<QueryResult<List<Audit>>> GetAuditsBySubject(int userId)
        {
            return QueryResult<List<Audit>>.Success(await _context.Audits.Where(a => a.SubjectId == userId).ToListAsync());
        }

        #endregion

        #region Get audits by author

        public async Task<QueryResult<List<Audit>>> GetAuditsByAuthor(int authorId)
        {
            return QueryResult<List<Audit>>.Success(await _context.Audits.Where(a => a.AuthorId == authorId).ToListAsync());
        }

        #endregion

        #region Audit list to CSV

        public QueryResult<string> AuditListToCsv(List<Audit> list)
        {
            // generate unique filename
            var fileName = $"AuditList_{DateTime.Now:yyyyMMddHHmmssfff}.csv";

            // create csv file
            var csv = new StringBuilder();

            // ensure file is in CSV Files folder
            fileName = Path.Combine("CSV Files", fileName);

            // get properties of object
            var properties = typeof(Audit).GetProperties();

            // add headers
            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            // add data
            foreach (var item in list)
            {
                csv.AppendLine(string.Join(",", properties.Select(p => p.GetValue(item, null))));
            }

            // write to file
            File.WriteAllText(fileName, csv.ToString());

            // return filename to caller
            return QueryResult<string>.Success(fileName);
        }

        #endregion
    }
}
