namespace BarryFileMan.Models.Config
{
    public class UserFlattenConfig
    {
        public string? DefaultRegexFileFilter { get; set; }

        public bool DeleteExcludedFilesDefault { get; set; }

        public bool DeleteEmptyFoldersDefault { get; set; }

        public UserFlattenConfig() : this(null, false, true) { }
        public UserFlattenConfig(string? defaultRegexFileFilter, bool deleteExcludedFilesDefault, bool deleteEmptyFoldersDefault)
        {
            DefaultRegexFileFilter = defaultRegexFileFilter;
            DeleteExcludedFilesDefault = deleteExcludedFilesDefault;
            DeleteEmptyFoldersDefault = deleteEmptyFoldersDefault;
        }
    }
}
