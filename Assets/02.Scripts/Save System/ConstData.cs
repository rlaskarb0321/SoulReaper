using System;

public static class ConstData
{
    // ���� ���� �ܰ�
    public enum eBossEncounterPhase { FirstMeet, Induce, Rational, }
    public const int TOTAL_BOSS_ENOCOUNTER_PHASE_COUNT = 3;

    // �� ~ ������ �������� �÷��̾�� �������� ��ġ, ȸ����
    // ��ġ
    public const float FROM_CASTLE_TO_FOREST_POS_X = -49.641f;
    public const float FROM_CASTLE_TO_FOREST_POS_Y = 0.08f;
    public const float FROM_CASTLE_TO_FOREST_POS_Z = -24.25f;
    // ȸ����
    public const float FROM_CASTLE_TO_FOREST_ROT_X = 0.0f;
    public const float FROM_CASTLE_TO_FOREST_ROT_Y = 0.99f;
    public const float FROM_CASTLE_TO_FOREST_ROT_Z = 0.0f;
    public const float FROM_CASTLE_TO_FOREST_ROT_W = -0.063f;

    // �� ~ ������ ������ �÷��̾�� �������� ��ġ, ȸ����
    // ��ġ
    public const float FROM_FOREST_TO_CASTLE_POS_X = -58.37f;
    public const float FROM_FOREST_TO_CASTLE_POS_Y = 0.09f;
    public const float FROM_FOREST_TO_CASTLE_POS_Z = -35.64f;
    // ȸ����
    public const float FROM_FOREST_TO_CASTLE_ROT_X = 0.0f;
    public const float FROM_FOREST_TO_CASTLE_ROT_Y = 0.44f;
    public const float FROM_FOREST_TO_CASTLE_ROT_Z = 0.0f;
    public const float FROM_FOREST_TO_CASTLE_ROT_W = 0.89f;

    // ����� ȥ�㸻, �÷��̾ Ʈ���� �ƿ������� ȥ�㸻�� �� Ȯ��
    public const float VICTIM_TRIGGER_OUT_TALK_MY_SELF_PERCENTAGE = 18.0f;

    // ����� ���� ���� ��Ű�µ� �ʿ��� ��ȥ ��ȭ�� ��
    public const int VICTIM_LIBERATE_SOUL_COUNT = 60;

    // ��翡 ��ġ�� ��ȥ�� ��
    public const int SHRINE_COST = 20;

    // ��ų UI �� �׸� ����
    public const int SKILL_UI_COUNT = 3;
}
