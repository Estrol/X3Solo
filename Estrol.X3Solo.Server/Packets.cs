namespace Estrol.X3Solo.Server {

    /// <summary>
    /// O2-JAM Client TCP opcode, All taken from sniffting through wireshark.
    /// </summary>
    public enum Packets : ushort {
        Disconnect = 0xfff0,
        Login = 0x03ef,
        Connect = 0x03f1,
        PlanetConnect = 0x03f3,
        PlanetLogin = 0x03e8,
        Channel = 0x03ea,
        OJNList = 0x0fbe,
        EnterCH = 0x03ec,
        GetRoom = 0x07d2,
        GetChar = 0x07d0,
        Timest = 0x13A4,
        ClientList = 0x07e8,
        TCPPing = 0x1771,
        LeaveCH = 0x07e5,
        CreateRoom = 0x07d4,
        LeaveRoom = 0x0bbd,
        RoomBGChange = 0x0fa2,
        SetSongID = 0x0fa0,
        RoomPlrColorChange = 0x0fa4,
        LobbyChat = 0x07dc,
        JoinRoom = 0x0bba,
        RoomChat = 0x0bc3,
        RoomListAddRoom = 0x07d5,
        RoomListRemoveRoom = 0x07d7,
        RoomRingChange = 0xfb7,
        GameStart = 0xfaa,
        ApplyRing = 0xfac,
        GameQuit = 0xfb5,
        GamePing = 0xfae,
        ScoreSubmit = 0xfb0,
        RoomNameChange = 0xbb8,
        Tutorial1 = 0x138e,
        Tutorial2 = 0x138f
    }
}
