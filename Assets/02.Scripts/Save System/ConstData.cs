using System;

public static class ConstData
{
    // 보스 조우 단계
    public enum eBossEncounterPhase { FirstMeet, Induce, Rational, }
    public const int TOTAL_BOSS_ENOCOUNTER_PHASE_COUNT = 3;

    // 성 ~ 숲으로 나왔을때 플레이어에게 지정해줄 위치, 회전값
    // 위치
    public const float FROM_CASTLE_TO_FOREST_POS_X = -49.641f;
    public const float FROM_CASTLE_TO_FOREST_POS_Y = 0.08f;
    public const float FROM_CASTLE_TO_FOREST_POS_Z = -24.25f;
    // 회전값
    public const float FROM_CASTLE_TO_FOREST_ROT_X = 0.0f;
    public const float FROM_CASTLE_TO_FOREST_ROT_Y = 0.99f;
    public const float FROM_CASTLE_TO_FOREST_ROT_Z = 0.0f;
    public const float FROM_CASTLE_TO_FOREST_ROT_W = -0.063f;

    // 숲 ~ 성으로 들어갔을때 플레이어에게 지정해줄 위치, 회전값
    // 위치
    public const float FROM_FOREST_TO_CASTLE_POS_X = -58.37f;
    public const float FROM_FOREST_TO_CASTLE_POS_Y = 0.09f;
    public const float FROM_FOREST_TO_CASTLE_POS_Z = -35.64f;
    // 회전값
    public const float FROM_FOREST_TO_CASTLE_ROT_X = 0.0f;
    public const float FROM_FOREST_TO_CASTLE_ROT_Y = 0.44f;
    public const float FROM_FOREST_TO_CASTLE_ROT_Z = 0.0f;
    public const float FROM_FOREST_TO_CASTLE_ROT_W = 0.89f;

    // 희생자 혼잣말, 플레이어가 트리거 아웃됐을때 혼잣말을 할 확률
    public const float VICTIM_TRIGGER_OUT_TALK_MY_SELF_PERCENTAGE = 18.0f;

    // 희생자 봉인 해제 시키는데 필요한 영혼 재화의 수
    public const int VICTIM_LIBERATE_SOUL_COUNT = 60;

    // 사당에 바치는 영혼의 수
    public const int SHRINE_COST = 20;

    // 공 버프
    public const string DAMAGE_BUFF_NAME = "분노";
    public const float DAMAGE_BUFF_DURATION = 10.0f;
    public const float DAMAGE_BUFF_AMOUNT = 20.0f;

    // 체력, 마나 뻥튀기 버프
    public const string HPMP_BUFF_NAME = "강인함";
    public const float HPMP_BUFF_DURATION = 10.0f;
    public const float HP_BUFF_AMOUNT = 50.0f;
    public const float MP_BUFF_AMOUNT = 50.0f;
}
