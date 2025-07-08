namespace IMDB_DB.Builders
{
    internal abstract class BaseBuilder
    {
        internal string _fileName;
        internal string _outputDir;
        internal string _inputDataBaseDir;
        internal string _insertHeader;

        public BaseBuilder( string outputDir, string inputDataFile, string fileName )
        {
            _outputDir = outputDir;
            _inputDataBaseDir = inputDataFile;
            _fileName = fileName;
        }

        public abstract void CreateRatingInsertFiles();
    }
}