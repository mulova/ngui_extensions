using UnityEngine;
using System.Text;
using System;
using System.Collections.Generic;
using commons;
using comunity;
using ngui.ex;


public static class UILabelEx
{
    public static Loggerx log = LogManager.GetLogger(typeof(UILabel));

    public static void SetSimpleTimeAgo(this UILabel label, TimeSpan t)
    {
        double days = t.TotalDays;
        if (days >= 1)
        {
            label.SetText("{0}d", (int)days);
            return;
        }
        double hours = t.TotalHours;
        if (hours >= 1)
        {
            label.SetText("{0}h", (int)hours);
            return;
        }
        double min = t.TotalMinutes;
        if (min >= 1)
        {
            label.SetText("{0}m", (int)min);
            return;
        }
        double sec = t.TotalSeconds;
        if (sec >= 1)
        {
            label.SetText("{0}s", (int)sec);
            return;
        }
        label.SetText("0s");
    }

    public static void SetPlainNumber(this UILabel label, long n)
    {
        if (label == null)
        {
            return;
        }
        label.SetNumber(n, null, false, null);
    }

    public static void SetPlainNumber(this UILabel label, long n, string suffix, bool sign)
    {
        label.SetNumber(n, suffix, sign, null);
    }

    public static void Clear(this UILabel label)
    {
        if (label == null)
        {
            return;
        }
        label.text = string.Empty;
    }


    public static long GetNumber(this UILabel label)
    {
        if (label == null)
        {
            return 0;
        }
        string text = label.text;
        long l = 0;
        if (text.IsNotEmpty())
        {
            long.TryParse(text, out l);
        }
        return l;
    }

    public static void SetNumber(this UILabel label, long n)
    {
        if (label == null)
        {
            return;
        }
        label.SetNumber(n, null, false);
    }

    public static void SetNaturalNumber(this UILabel label, long n)
    {
        if (label == null)
        {
            return;
        }
        if (n > 0)
        {
            label.SetNumber(n, null, false);
        } else
        {
            label.SetText("-");
        }
    }

    public static void SetNumber(this UILabel label, long n, string suffix, bool sign, string numberFormat = "N0")
    {
        if (label == null)
        {
            return;
        }
        if (label is UIText)
        {
            (label as UIText).textKey = null;
        }
        StringBuilder str = new StringBuilder();
        if (n == 0)
        {
            str.Append("0");
        } else
        {
            if (sign)
            {
                if (n > 0)
                {
                    str.Append("+");
                }
            }
            if (numberFormat.IsNotEmpty())
            {
                str.Append(n.ToString(numberFormat));
            } else
            {
                str.Append(n.ToString());
            }
        }
        if (!string.IsNullOrEmpty(suffix))
        {
            str.Append(" ");
            str.Append(suffix);
        }
        label.SetText(str.ToString());
    }

    public static void SetLexicon(this UILabel label, string format, params object[] param)
    {
        if (label == null)
        {
            return;
        }
        label.SetText(Lexicon.Get(format, param));
    }

    public static void SetText(this UILabel label, string format, params object[] param)
    {
        if (label == null)
        {
            return;
        }
        if (label is UIText)
        {
            (label as UIText).textKey = null;
        }
        if (format != null)
        {
            if (param.IsNotEmpty())
            {
                try
                {
                    label.text = string.Format(format, param);
                } catch (Exception ex)
                {
                    log.Error(ex);
                    label.text = format;
                }
            } else
            {
                label.text = format;
            }
        } else
        {
            label.text = null;
        }
    }

    public static void SetCurrencyLong(this UILabel label, long value, bool unit = true)
    {
        label.SetText(unit? "{0:C0}": "{0:N0}", value);
    }

    public static void SetCurrency(this UILabel label, long value, bool unit = true)
    {
        label.SetText(GetCurrency(value, unit));
    }

    public static string GetCurrency(long value, bool unit = true)
	{
		string cur = "";
		long temp = value;
		if (value >= 1000000000) {
			temp = value % 1000000000;
            if (temp < 100000000) cur = string.Format(unit? "{0:c0}B": "{0:d0}B", value/1000000000);
            else cur = string.Format(unit? "{0:c1}B": "{0:n1}B", value/1000000000f);
		}
		else if (value >= 1000000)
		{
			temp = value % 1000000;
            if (temp < 100000) cur = string.Format(unit? "{0:c0}M": "{0:d0}M", value/1000000);
            else cur = string.Format(unit? "{0:c1}M": "{0:n1}M", value/1000000f);
		} else if (value >= 1000)
		{
			temp = value % 1000;
            if (temp < 100) cur = string.Format(unit? "{0:c0}K": "{0:d0}K", value/1000);
            else cur = string.Format(unit? "{0:c1}K": "{0:n1}K", value/1000f);
		} else
		{
            cur = string.Format(unit? "{0:c0}": "{0:d0}", value);
		}
		return cur;
	}
}
