using System.Diagnostics;
using ShadowverseArtOverlay.RemoteMemoryObjects;

namespace ShadowverseArtOverlay.Framework
{
    public class Shadowverse : RemoteMemoryObject
    {
        private static Shadowverse _instance;
        private static int _instanceId;

        public static Shadowverse Instance
        {
            get
            {
                var proc = FindShadowverseProcess();

                if (proc == null)
                    return null;

                if (_instance == null || _instanceId != proc.Id)
                {
                    var mem = new Memory(proc.Id);
                    _instance = new Shadowverse(mem);
                    _instanceId = proc.Id;
                }

                return _instance;
            }
        }

        private static Process FindShadowverseProcess()
        {
            var client = Process.GetProcessesByName("Shadowverse");
            if (client.Length == 0)
            {
                return null;
            }

            return client[0];
        }
        public Shadowverse(Memory m)
        {
            M = m;
            Address = m.BaseAddress;
            Game = this;
        }

        public Memory Memory => M;
        public SingleBattleMgr BattleManagerSingleton => GetObject<SingleBattleMgr>(M.ReadInt(M.MonoBaseAddress + 0x1F46AC, 0x18, 0xDA0) + 0x0);

        public bool IsCardSelected =>
            BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerPanel.NameLabel.Active ||
            BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerEvoPanel.NameLabel.Active ||
            BattleManagerSingleton.DetailManager.DetailPanelControl.NonFollowerPanel.NameLabel.Active;

        public string SelectedCard =>
            BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerPanel.NameLabel.Active
                ? BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerPanel.NameLabel.Text
                : BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerEvoPanel.NameLabel.Active
                    ? BattleManagerSingleton.DetailManager.DetailPanelControl.FollowerEvoPanel.NameLabel.Text
                    : BattleManagerSingleton.DetailManager.DetailPanelControl.NonFollowerPanel.NameLabel.Active
                        ? BattleManagerSingleton.DetailManager.DetailPanelControl.NonFollowerPanel.NameLabel.Text
                        : string.Empty;

        public string LocateOffsetWithSelectedCardName(string cardName)
        {

            for (int i = 0xFFFFF0; i >= 0; i -= 4)
            {
                var battleManagerDebug = GetObject<SingleBattleMgr>(M.ReadInt(M.BaseAddress + i, 0x1CC, 0x734) + 0xA40);

                var followerPanel = battleManagerDebug.DetailManager.DetailPanelControl.FollowerPanel;
                var followerEvoPanel = battleManagerDebug.DetailManager.DetailPanelControl.FollowerEvoPanel;
                var nonFollowerPanel = battleManagerDebug.DetailManager.DetailPanelControl.NonFollowerPanel;

                if (followerPanel.NameLabel.Active && followerPanel.NameLabel.Text.ToLower() == cardName.ToLower())
                {
                    var ret = i.ToString("x");
                    Debugger.Break();
                    return ret;
                }

                if (followerEvoPanel.NameLabel.Active && followerEvoPanel.NameLabel.Text.ToLower() == cardName.ToLower())
                {
                    var ret = i.ToString("x");
                    Debugger.Break();
                    return ret;
                }

                if (nonFollowerPanel.NameLabel.Active && nonFollowerPanel.NameLabel.Text.ToLower() == cardName.ToLower())
                {
                    var ret = i.ToString("x");
                    Debugger.Break();
                    return ret;
                }
            }

            return "Not Found";
        }
    }
}