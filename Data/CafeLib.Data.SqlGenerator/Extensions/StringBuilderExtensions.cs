﻿using System;
using System.Text;

namespace CafeLib.Data.SqlGenerator.Extensions
{
    public static class StringBuilderExtensions
    {
        public static void AppendLineWithSpace(this StringBuilder sb, string text)
        {
            var lines = text.Split(new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            text = string.Join("\n".PadRight(5), lines);
            sb.AppendLine($"    {text}");
        }
    }
}