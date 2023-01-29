namespace SimonUtils.DbUtil
{
    public static class Database
    {
        private static DatabaseContext _context = new DatabaseContext();

        public static void Add<T>(T obj)
        {
            using (_context)
            {

            }
        }

        public static void Get<T>(List<T> objList)
        {

        }

        public static void DeleteFrom<T>(List<T> objList)
        {

        }

        public static void Delete<T>(T obj)
        {

        }
    }
}
