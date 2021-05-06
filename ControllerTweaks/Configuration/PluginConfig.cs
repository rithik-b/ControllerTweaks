using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ControllerTweaks.Configuration
{
    internal class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public virtual bool ControllerSwapEnabled { get; set; } = false;

        [UseConverter(typeof(ListConverter<OVRInput.Button>))]
        [NonNullable]
        public virtual List<OVRInput.Button> PauseButtons { get; set; } = new List<OVRInput.Button>();
        public virtual bool PauseRemapEnabled { get; set; } = false;

        [UseConverter(typeof(ListConverter<OVRInput.Button>))]
        [NonNullable]
        public virtual List<OVRInput.Button> LeftSelectButtons { get; set; } = new List<OVRInput.Button>();
        public virtual bool LeftSelectRemapEnabled { get; set; } = false;

        [UseConverter(typeof(ListConverter<OVRInput.Button>))]
        [NonNullable]
        public virtual List<OVRInput.Button> RightSelectButtons { get; set; } = new List<OVRInput.Button>();
        public virtual bool RightSelectRemapEnabled { get; set; } = false;

        [UseConverter(typeof(DictionaryConverter<OffsetPreset>))]
        [NonNullable]
        public virtual Dictionary<string, OffsetPreset> OffsetPresets { get; set; } = new Dictionary<string, OffsetPreset>();
        public virtual int SelectedPreset { get; set; } = 0;

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
}