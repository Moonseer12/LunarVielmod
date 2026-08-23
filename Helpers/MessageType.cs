namespace Stellamod.Helpers
{
    public enum MessageType : byte
    {
        None = 0,
        ProjectileData,
        Dodge,
        Dash,
        BossSpawnFromClient,
        SpawnExplosiveBarrel,
        BoonData,
        CompleteZuiQuest,
        CreatePortal,
        DashPlayerSync,
        ResetColosseum,
        StartColosseum,
        HandleDoor,
        ScarecrowPlayerSync,

        PlaceRibbon,
        BreakRibbon,

        PlaceDecoration,
        BreakDecoration,
        AggroSync,
        RecoilPlayerSync,
        RomanceDodge,
        BossDowned,
        CauldronSync,
        WaypointActivate,
        SpawnNPC,
        ZTileSync,
        ChangeNPCAI,
        LevelingPlayerSync,
        ClassReworkPlayerSync
    }
}