#nullable enable
namespace ClientCore.I18N;

/// <summary>
/// 描述一个基于日期范围加载的 MIX 文件配置。
/// </summary>
/// <param name="EventName">事件名称（如 Christmas、Halloween）。</param>
/// <param name="StartMonth">起始月份（1-12）。</param>
/// <param name="StartDay">起始日期（1-31）。</param>
/// <param name="EndMonth">结束月份（1-12）。</param>
/// <param name="EndDay">结束日期（1-31）。</param>
/// <param name="Source">源文件路径，相对于游戏根目录。</param>
/// <param name="Target">目标文件路径，相对于游戏根目录。</param>
public readonly record struct DateBasedMix(
    string EventName,
    int StartMonth, int StartDay,
    int EndMonth, int EndDay,
    string Source, string Target);