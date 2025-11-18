using System.Text.RegularExpressions;

public static class InputSanitizer
{
    // 禁止任何會破壞 Header / Cookie 的字元
    public static string SanitizeForCookie(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        // 移除 CRLF, 空白控制字元
        string sanitized = Regex.Replace(input, @"[\r\n\t\f\v]", "");

        // 避免注入 ; = 等 cookie 結構字元
        sanitized = Regex.Replace(sanitized, @"[;]", "");

        return sanitized;
    }
}
