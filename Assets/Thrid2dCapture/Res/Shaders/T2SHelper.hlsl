#ifndef T2SHELPER_INCLUDED
#define T2SHELPER_INCLUDED

// 打包函数
void PackAlpha_float(float AlphaBit, float ExtraData, out float Out)
{
    // 使用 round 确保浮点数精度准确，然后显式转为 uint
    uint a = uint(round(AlphaBit * 255.0f));
    uint e = uint(round(ExtraData * 127.0f));

    // 再次进行位掩码操作，并合并
    // 强制截断，防止输入值过大污染其他位
    uint combined = ((e & 0x7FU) << 1) | (a & 1U);

    // 输出到 0-1 范围
    Out = combined / 255.0f;
}

// 解包函数
void UnpackAlpha_float(float AlphaIn, out float AlphaBit, out float ExtraData)
{
    // 1. 还原为 0-255 的整数，注意加上 0.5 的偏移或使用 round 增加鲁棒性
    uint val = uint(round(AlphaIn * 255.0f));

    // 2. 提取位信息
    // 使用 U 后缀明确表示无符号常量
    AlphaBit = float(val & 1U);
    ExtraData = float((val >> 1) & 0x7FU) / 127.0f;
}
#endif