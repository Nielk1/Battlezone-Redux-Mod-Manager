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
        // Workshop metadata title, filled in by the name enrichment side-band as soon
        // as WorkshopContext has it (usually instantly from the JSON cache).
        public string WorkshopName { get; internal set; }

        // The [MODMANAGER]::name override from the mod's own inis, when the author set one.
        public virtual string ManagerName { get { return null; } }

        // The name generated from the in-game inis (mission names, workshop ini names, ...).
        public virtual string GeneratedName { get { return UniqueID; } }

        // The raw Steam Workshop ID for this mod, when it has one, for metadata lookups.
        public virtual string GetWorkshopId() { return null; }

        public string Name { get
            {
                // Precedence: the mod's own [MODMANAGER]::name override, then the
                // Workshop metadata title (arrives asynchronously), then the name
                // generated from the in-game inis.
                string manager = ManagerName;
                if (!string.IsNullOrWhiteSpace(manager)) return manager;
                string workshop = WorkshopName;
                if (!string.IsNullOrWhiteSpace(workshop)) return workshop;
                return GeneratedName;
            } }
        public Image LargeIcon { get; set; }
        public Image SmallIcon { get; set; }
        public ListViewItem ListViewItemCache { get; set; }
        public bool HasUpdate { get; internal set; }
        public bool FolderOnlyDetection { get; internal set; }

        public override string ToString()
        {
            return Name;

        }

        public abstract void ToggleGog();
        public abstract void ToggleSteam();

        public abstract bool Delete();
    }
}
