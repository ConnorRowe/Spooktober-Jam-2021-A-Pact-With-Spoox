using Godot;

namespace Spooktober
{
    public class Settings : VBoxContainer
    {
        private HSlider masterSlider;
        private HSlider musicSlider;
        private HSlider sfxSlider;
        private CheckButton fullscreenToggle;

        private int masterBusId;
        private int musicBusId;
        private int sfxBusId;

        public override void _Ready()
        {
            base._Ready();

            masterSlider = GetNode<HSlider>("MasterSlider");
            musicSlider = GetNode<HSlider>("MusicSlider");
            sfxSlider = GetNode<HSlider>("SFXSlider");
            fullscreenToggle = GetNode<CheckButton>("FullscreenToggle");

            masterBusId = AudioServer.GetBusIndex("Master");
            musicBusId = AudioServer.GetBusIndex("Music");
            sfxBusId = AudioServer.GetBusIndex("SFX");

            masterSlider.Value = GetBusVol(masterBusId);
            musicSlider.Value = GetBusVol(musicBusId);
            sfxSlider.Value = GetBusVol(sfxBusId);
            fullscreenToggle.Pressed = OS.WindowFullscreen;

            masterSlider.Connect("value_changed", this, nameof(MasterChanged));
            musicSlider.Connect("value_changed", this, nameof(MusicChanged));
            sfxSlider.Connect("value_changed", this, nameof(SFXChanged));
            fullscreenToggle.Connect("toggled", this, nameof(FullscreenToggled));
        }

        private float GetBusVol(int busId)
        {
            return GD.Db2Linear(AudioServer.GetBusVolumeDb(busId));
        }

        private void SetBusVol(int busId, float vol)
        {
            AudioServer.SetBusVolumeDb(busId, GD.Linear2Db(vol));
        }

        private void MasterChanged(float vol)
        {
            SetBusVol(masterBusId, vol);
        }

        private void MusicChanged(float vol)
        {
            SetBusVol(musicBusId, vol);
        }

        private void SFXChanged(float vol)
        {
            SetBusVol(sfxBusId, vol);
        }

        private void FullscreenToggled(bool toggle)
        {
            OS.WindowFullscreen = toggle;
        }
    }
}