using System.Security.Policy;

namespace webapi.Services
{
    public class PaginationFilter
    {
        public int pageNumber { get; set; }

        public int pageSize { get; set; }

        // default setup values for pagination
        public PaginationFilter()
        {
            this.pageNumber = 1;
            this.pageSize = 50;
        }

        // provide values for pageNumber and pageSize
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.pageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.pageSize = pageSize < 50 ? 50 : pageSize;
        }
    }
}
