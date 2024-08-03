namespace Network
{
	public enum EConnectResult
	{
		Success,
		TimeOut,
		Disconnect,
		Error
	}

	public enum EMessageType
	{
		MT_NOTING = 0,
		MT_MATCHQ_JOIN,
		MT_SET_NAME,
		MT_MESSEGE,
		MT_ACTIVE_USER,
		MT_ROOM_CREATED,
		MT_GAME_RESULT,
		MT_USER_ACTION
	}
}