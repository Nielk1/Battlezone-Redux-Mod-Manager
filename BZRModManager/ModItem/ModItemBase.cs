using System.Drawing;
using System.Windows.Forms;

namespace BZRModManager.ModItem
{
    public interface ILinqListViewItemMods
    {
        string IconKey { get; }
        string Name { get; }

        string ModType { get; }
        string[] ModTags { get; }
        string WorkshopIdOutput { get; }
        string ModSource { get; }

        InstallStatus InstalledSteam { get; }
        InstallStatus InstalledGog { get; }

        string FilePath { get; }

        Image LargeIcon { get; }
        Image SmallIcon { get; }
        ListViewItem ListViewItemCache { get; set; }

        void ToggleGog();
        void ToggleSteam();
        bool Delete();
    }

    public abstract class ModItemBase : ILinqListViewItemMods
    {
        public abstract string UniqueID { get; }
        public abstract InstallStatus InstalledSteam { get; }
        public abstract InstallStatus InstalledGog { get; }
        public int AppId { get; protected set; }
        public abstract string ModType { get; }
        public abstract string[] ModTags { get; }
        public abstract string WorkshopIdOutput { get; }
        public abstract string ModSource { get; }

        public abstract string FilePath { get; }

        public string IconKey { get { return UniqueID; } }
        public string Name { get { return ToString(); } }
        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }
        public bool HasUpdate { get; internal set; }
        public bool FolderOnlyDetection { get; internal set; }

        public override string ToString()
        {
            //if (Workshop != null) return Workshop.WorkshopId.ToString();
            return "UNKNOWN MOD";
        }

        public abstract void ToggleGog();
        public abstract void ToggleSteam();

        public abstract bool Delete();
    }
}
