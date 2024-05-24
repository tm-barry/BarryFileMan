namespace BarryFileMan.ViewModels.Rename.Providers
{
    public partial class TMDBMovieRenameProviderViewModel : BaseTMDBRenameProviderViewModel
    {
        public TMDBMovieRenameProviderViewModel() : this(new RenameViewModel()) { }

        public TMDBMovieRenameProviderViewModel(RenameViewModel viewModel) : base(viewModel)
        {
            // TODO - load from preset
            MatchPattern = "(?<movie>[^(?:\\\\|/)]+?)\\W(?:(?<year>\\d{4})(?:\\W(?<resolution>\\d+p)?)|(?<resolution>\\d+p)(?:\\W(?<year>\\d{4}))?)";
            RenamePattern = "";
            Input = "\\ParentFolder\\Movie.2000.1080p";
            SelectedMatchTypeIndex = 1;
        }
    }
}
