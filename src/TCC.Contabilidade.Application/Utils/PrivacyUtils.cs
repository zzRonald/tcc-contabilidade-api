using System.Text.RegularExpressions;

namespace TCC.Contabilidade.Application.Utils;

public static class PrivacyUtils
{
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            return email;

        var parts = email.Split('@');
        var name = parts[0];
        var domain = parts[1];

        if (name.Length <= 2)
            return $"{name[0]}***@{domain}";

        return $"{name[0]}***{name[^1]}@{domain}";
    }

    public static string MaskIp(string ip)
    {
        if (string.IsNullOrEmpty(ip))
            return ip;

        // IPv4
        if (ip.Contains('.'))
        {
            var parts = ip.Split('.');
            if (parts.Length == 4)
            {
                return $"{parts[0]}.{parts[1]}.***.***";
            }
        }

        // IPv6
        if (ip.Contains(':'))
        {
            var parts = ip.Split(':');
            if (parts.Length >= 2)
            {
                return $"{parts[0]}:{parts[1]}:***:***";
            }
        }

        return "***.***.***.***";
    }

    public static string MaskCpf(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return cpf;

        var numbers = Regex.Replace(cpf, @"[^\d]", "");
        if (numbers.Length != 11)
            return "***.***.***-**";

        return $"***.***.***-{numbers.Substring(9)}";
    }

    public static string MaskCnpj(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return cnpj;

        var numbers = Regex.Replace(cnpj, @"[^\d]", "");
        if (numbers.Length != 14)
            return "**..***.***.****-**";

        return $"{numbers.Substring(0, 2)}.***.***/****-{numbers.Substring(12)}";
    }
}
