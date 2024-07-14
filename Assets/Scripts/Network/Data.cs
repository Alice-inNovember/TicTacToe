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
		MT_USER_ACTION,
	};
}
// 1. 계정, 점수 제도 (후순위)
// 2. player 종류 알려주는 창 (X, O)
// 3. Turn 바뀔때마다 누구 턴인지 알려주는 uI
// 4. 작은 칸에 마우스 올리면, 어느 큰칸에 햘당되는지 알려주는 UI
// 5. 끝날때 어느 수로 끝났는지 보이게
// 6. 전턴에 어디다가 놨는지 알려주는 UI
// 7. O, X 색깔 구분
// 8. Turn Timer (한플레이어당 일정 시간 햘당, 스피드 모드는 한 턴 당 일정 시간 햘당)
// 9. 첫턴 플레이어 정가운데 작은칸 못 먹게 (플레이하면서 생각)
// 10. 글로벌 UI (인게임 세팅)
// 11. 런칭 플랫폼 선정 (웹, 맥, 윈도우 등)
// 1, 7, 8, 9(보류), 10, 11
// 12. Match Stop
