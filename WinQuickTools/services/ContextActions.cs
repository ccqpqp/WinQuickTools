using WinQuickTools.Features;

namespace WinQuickTools
{
    public static class ContextActions
    {
        public static void Execute(string cmd, string? path)
        {
            if (path == null) return;

            switch (cmd)
            {
                case "exportlist":
                    FileFeatures.ExportList(path);
                    break;

                case "renamebatch":
                    FileFeatures.RenameBatch(path);
                    break;

                case "renumber":
                    FileFeatures.Renumber(path);
                    break;

                case "fullpathcopy":
                    FileFeatures.FullPathCopy(path);
                    break;
            }
        }
    }
}