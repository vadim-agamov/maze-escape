namespace Modules.CheatService.Controls
{
    public abstract class CheatControl
    {
        private readonly ICheatService _cheatService;

        protected CheatControl(ICheatService cheatService)
        {
            _cheatService = cheatService;
        }
        protected void HideCheats() => _cheatService.Hide();
    }
}